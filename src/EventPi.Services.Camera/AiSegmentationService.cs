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

namespace EventPi.Services.Camera;

static class CommandExtension
{

    public static Command WithArgumentsIf(this Command cmd, bool condition, string command) => condition ? cmd.WithArguments(command) : cmd;
}




public class AiSegmentationService : IPartialYuvFrameHandler, IDisposable
{
    private readonly AiCameraConfigurationProvider _aiConfig;
    private readonly RemoteCanvasStreamPool _pool;
    private readonly IYoloModelRunner<Segmentation> _runner;
    private readonly float _threshold;
    private Rectangle _interestRegion;
    private ICanvas _canvas;
    private AiModelConfiguration _configuration;
    private volatile int _parallelCount = 0;
    private const int MaxParallelCount = 1;
    public bool Should(ulong seq)
    {
        var sampling = seq % (ulong)Every == 0ul;
        if (sampling)
        {
            if (Interlocked.Increment(ref _parallelCount) <= MaxParallelCount)
                return true;

            Interlocked.Decrement(ref _parallelCount);
            return false;
        }

        return false;
    }

    public AiSegmentationService(IConfiguration configuration,
        AiCameraConfigurationProvider aiConfig,
        RemoteCanvasStreamPool pool)
    {
        _aiConfig = aiConfig;
        _pool = pool;
        var modelPath = configuration.GetOnnxModel();
        this._threshold = configuration.GetAiConfidenceThreshold();
        this._runner =  YoloModelFactory.LoadSegmentationModel(modelPath);
    }
    public int Every { get; } = 1;
    public unsafe void Handle(YuvFrame frame, YuvFrame? prv, ulong seq, CancellationToken token, object st)
    {
        var r = _interestRegion = _configuration.ConfigurationState.InterestRegion;
        using var results = _runner.Process(&frame, &r, _threshold);

        _canvas.Begin(seq, 2);
        _canvas.DrawRectangle(_interestRegion, RgbColor.Green, 2);
        _canvas.End(2);
        
        _canvas.Begin(seq, 3);
        foreach (var i in results.Where(x=>x.Polygon != null))
        {
            var p = i.Polygon;
            p.TransformBy(_interestRegion);
            
            _canvas.DrawPolygon(p.Polygon.Points, RgbColor.Red,3);
            
            var p1 = i.Polygon.Polygon[0];
            _canvas.DrawText(i.Name.Name, p1.X, p1.Y,  color: RgbColor.Green, layerId:3);
            //Debug.WriteLine($"Draw polygon: {sPol.Polygon.ToAnnotationString()}");
        }
        
        Debug.WriteLine($"AI metrics, allocated bytes: {(Bytes)ManagedArray<VectorU16>.ALLOCATED_BYTES} {_runner.Performance}");
        _canvas.End(3);
        
        Interlocked.Decrement(ref _parallelCount);
    }

    public void Init(VideoAddress va)
    {
        _interestRegion = new Rectangle(200, 1080/2-300, 640, 640);
        _canvas = _pool.GetCanvas(va);
        _configuration = _aiConfig.Get(va);
    }

    public void Dispose()
    {
        
    }
}