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
using ModelingEvolution.VideoStreaming.Buffers;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

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
        RemoteCanvasStreamPool pool)
    {
        _aiConfig = aiConfig;
        _pool = pool;
        var modelPath = configuration.GetModelAiPath();
        this._threshold = configuration.GetAiConfidenceThreshold();
        logger.LogInformation($"Loading {modelPath}");
        this._runner = (IAsyncSegmentationModelRunner<ISegmentation>)ModelFactory.LoadSegmentationModel(modelPath);
        this._runner.FrameSegmentationPerformed += OnResult;
        logger.LogInformation($"{modelPath} loaded.");
    }

    private readonly PeriodicConsoleWriter _writter = new(TimeSpan.FromSeconds(2));
    private void OnResult(object? sender, ISegmentationResult<ISegmentation> results)
    {
        _writter.WriteLine($"Cam/Frame: {results.Id}, in {_interestRegion}, results: {results}");
        _canvas.Begin(results.Id.FrameId, 2);
        _canvas.DrawRectangle(_interestRegion, RgbColor.Green, 2);
        _canvas.End(2);

        _canvas.Begin(results.Id.FrameId, 3);
        foreach (var i in results.Where(x => x.Polygon != null))
        {
            var p = i.Polygon;
            p.TransformBy(_interestRegion);

            _canvas.DrawPolygon(p.Polygon.Points, RgbColor.Red, 3);

            var p1 = i.Polygon.Polygon[0];
            _canvas.DrawText(i.Name.Name, p1.X, p1.Y, color: RgbColor.Green, layerId: 3);
            //Debug.WriteLine($"Draw polygon: {sPol.Polygon.ToAnnotationString()}");
        }
        
        //Debug.WriteLine($"AI metrics, allocated bytes: {(Bytes)ManagedArray<VectorU16>.ALLOCATED_BYTES} {_runner.Performance}");
        _canvas.End(3);

        Interlocked.Decrement(ref _parallelCount);
    }

    public int Every { get; } = 1;
    public unsafe void Handle(YuvFrame frame, YuvFrame? prv, ulong seq, CancellationToken token, object st)
    {
        var r = _interestRegion = _configuration.ConfigurationState.InterestRegion;
        _runner.AsyncProcess(&frame, r, r.Size, _threshold);

    }

    public void Init(VideoAddress va)
    {
        _interestRegion = new Rectangle(200, 1080/2-300, 640, 640);
        _canvas = _pool.GetCanvas(va);
        _configuration = _aiConfig.Get(va);
    }

    public void Dispose()
    {
        _runner.Dispose();
    }
}