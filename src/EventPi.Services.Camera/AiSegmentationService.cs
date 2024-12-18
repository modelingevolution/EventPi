using System.Collections.Concurrent;
using System.Diagnostics;
using CliWrap;
using Microsoft.Extensions.Configuration;
using ModelingEvolution_VideoStreaming.Yolo;
using ModelingEvolution.VideoStreaming;
using ModelingEvolution.VideoStreaming.VectorGraphics;
using Rectangle = System.Drawing.Rectangle;
using System.IO;
using System.Net;
using System.Threading.Channels;
using ModelingEvolution.VideoStreaming.Buffers;
using Docker.DotNet;
using EventPi.Abstractions;
using EventPi.Threading;
using Microsoft.Extensions.Logging;
using Emgu.CV.Dnn;
using static ModelingEvolution.VideoStreaming.VectorGraphics.ProtoStreamClient;

namespace EventPi.Services.Camera;

static class CommandExtension
{
    public static async Task<string> GetImageName(this DockerClient client, string containerName)
    {
        try
        {
            // Inspect the container to get its details
            var container = await client.Containers.InspectContainerAsync(containerName);
            string imageId = container.Image;
            // Inspect the image to get its details
            var image = await client.Images.InspectImageAsync(imageId);
            // Retrieve the image name with tag
            string imageNameWithTag = image.RepoTags?[0] ?? "Unknown";
            return imageNameWithTag;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving image name: {ex.Message}");
            return "Error";
        }
    }
    public static async Task<ContainerStatus> Check(this DockerClient client, string containerNameOrId)
    {
        try
        {
            var container = await client.Containers.InspectContainerAsync(containerNameOrId);
            if (container == null)
            {
                return ContainerStatus.DoesNotExist;
            }

            return container.State.Status == "running" ? ContainerStatus.Running : ContainerStatus.Stopped;
        }
        catch
        {
            return ContainerStatus.DoesNotExist;
        }
    }
    public static Command WithArgumentsIf(this Command cmd, bool condition, string command) => condition ? cmd.WithArguments(command) : cmd;
}




public class AiSegmentationService : IPartialYuvFrameHandler, IDisposable
{
    private readonly AiCameraConfigurationProvider _aiConfig;
    private readonly RemoteCanvasStreamPool _pool;
    private readonly IAsyncSegmentationModelRunner<ISegmentation> _runner;
    private readonly float _threshold;
    private Rectangle _interestRegion;
    private ICanvas _canvas;
    private AiModelConfiguration _configuration;
    private volatile int _parallelCount = 0;
    //private const int MaxParallelCount = 1;
    public bool Should(ulong seq)
    {
        var sampling = seq % (ulong)Every == 0ul;
        return true;
    }

    public AiSegmentationService(IConfiguration configuration,
        AiCameraConfigurationProvider aiConfig, ILogger<AiSegmentationService> logger,
        RemoteCanvasStreamPool pool, ModelFactory modelFactory)
    {
        _aiConfig = aiConfig;
        _pool = pool;
        var modelPath = configuration.GetModelAiPath();
        this._threshold = configuration.GetAiConfidenceThreshold();
        this._logger = logger;
        logger.LogInformation($"Loading {modelPath}");
        this._runner = (IAsyncSegmentationModelRunner<ISegmentation>)modelFactory.LoadSegmentationModel(modelPath);
        this._runner.FrameSegmentationPerformed += OnResult;
        logger.LogInformation($"{modelPath} loaded.");
    }

    private readonly PeriodicConsoleWriter _w1 = new(TimeSpan.FromSeconds(2));
    private readonly PeriodicConsoleWriter _w2 = new(TimeSpan.FromSeconds(2));
    private readonly ILogger<AiSegmentationService> _logger;

    private void OnResult(object? sender, ISegmentationResult<ISegmentation> results)
    {
        //_w2.WriteLine($"Cam/Frame: {results.Id}, in {_interestRegion}, results: {results}");
        try
        {
            using (var c = _canvas.BeginScope(results.Id.FrameId, 2))
            {
                c.DrawRectangle(_interestRegion, RgbColor.Blue);
                c.DrawText($"{results.Id.FrameId}:{_interestRegion.ToStringShort()}", (ushort)_interestRegion.X,
                    (ushort)_interestRegion.Y, 12, RgbColor.Blue);
            }

            if (results.Count > 0)
                using (var c = _canvas.BeginScope(results.Id.FrameId, 3))
                {
                    // We use for, so that we won't allocate memory for the iterator.
                    for (var index = 0; index < results.Count; index++)
                    {
                        var i = results[index];
                        if (i.Polygon == null) continue;

                        var p = i.Polygon;
                        p.TransformBy(_interestRegion);

                        c.DrawPolygon(p.Polygon.Points, RgbColor.Red);

                        var p1 = i.Polygon.Polygon[0];
                        c.DrawText(i.Name.Name, p1.X, p1.Y, color: RgbColor.Green);
                        //Debug.WriteLine($"Draw polygon: {sPol.Polygon.ToAnnotationString()}");
                    }
                }

            //Debug.WriteLine($"AI metrics, allocated bytes: {(Bytes)ManagedArray<VectorU16>.ALLOCATED_BYTES} {_runner.Performance}");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing AI results.");
        }
        finally
        {
            results.Dispose();
        }
    }

    public int Every { get; } = 1;

    private Channel<ulong> tmp = Channel.CreateUnbounded<ulong>();
    //private PrecisePeriodicTimer _t;
    private void Testing()
    {
        while (true)
        {
            //_t.WaitForNextIteration();
            if (tmp.Reader.TryRead(out var i))
            {
                using var c = _canvas.BeginScope(i,3);
                
                //using var c = new DrawingBatchScope(_canvas, 3, i);

                c.DrawRectangle(_interestRegion, RgbColor.Blue);
                c.DrawText($"{i}:{_interestRegion.ToStringShort()}", (ushort)_interestRegion.X,
                    (ushort)_interestRegion.Y, 12, RgbColor.Blue);
            }
            
        }
    }

    private bool _isRunning = false;
    
    public unsafe void Handle(YuvFrame frame, YuvFrame? prv, ulong seq, CancellationToken token, object st)
    {
        if (!_isRunning)
        {
            _isRunning = true;
            _runner.StartAsync();
            return;
        }
        //_w1.WriteLine($"Ai segmentation service frame: {seq}/{frame.Metadata.FrameNumber}, region: {_interestRegion.ToStringShort()}");
        var r = _interestRegion;
        _runner.AsyncProcess(&frame, r, r.Size, _threshold);
        tmp.Writer.TryWrite(seq);
    }

    public void Init(VideoAddress va)
    {
        _interestRegion = new Rectangle(1920/2 - 640/2, 1080/2-640/2, 640, 640);
        _canvas = _pool.GetCanvas(va);
        _configuration = _aiConfig.Get(va);
        //Thread t = new Thread(Testing);
        //_t = new PrecisePeriodicTimer(TimeSpan.FromSeconds(1d / 30));
        //t.Start();
        //_runner.StartAsync();
    }

    public void Dispose()
    {
        _runner.Dispose();
    }
}