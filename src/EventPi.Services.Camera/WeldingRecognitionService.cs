using EventPi.Abstractions;
using EventPi.Pid;
using EventPi.Services.Camera.Contract;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using ModelingEvolution.VideoStreaming;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Net;
using System.Text;
using MicroPlumberd;

namespace EventPi.Services.Camera;

public class WeldingRecognitionService : IPartialYuvFrameHandler, IDisposable
{
    private readonly ILogger<WeldingRecognitionService> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IEnvironment _env;
    private readonly CancellationTokenSource _cts;
    private readonly IPlumber _plumber;
    private ICameraManager _manager;
    private readonly Channel<SetCameraParameters> _channel;
    private readonly CircularBuffer<int> _bufferBrightPixels;
    private readonly CircularBuffer<int> _bufferDarkPixels;

    private readonly WeldingRecognitionModel _model;
    private readonly WeldingRecognitionProvider _profileProvider;

    public bool IsWelding { get; private set; }

    public int BrightOffset { get; set; }
    public int DarkOffset { get; set; }

    public ICameraParametersReadOnly CurrentAppliedProfile { get; private set; }

    public int Every => 1;

    public WeldingRecognitionService(
        WeldingRecognitionProvider profileProvider,
        IPlumber plumber,
        IEnvironment env,
        ILogger<WeldingRecognitionService> logger,
        ILoggerFactory loggerFactory,
        CameraManager manager)
    {
        CurrentAppliedProfile = new CameraProfile();
        _cts = new CancellationTokenSource();
        _plumber = plumber;
        _logger = logger;
        _loggerFactory = loggerFactory;
        _env = env;
        _profileProvider = profileProvider;
        _model = new WeldingRecognitionModel();
        _manager = manager;

        _bufferBrightPixels = new CircularBuffer<int>(3);
        _bufferDarkPixels = new CircularBuffer<int>(3);
        _channel = Channel.CreateBounded<SetCameraParameters>(new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropOldest });

    }
    private VideoAddress _address;
    public void Init(VideoAddress va)
    {
        _address = va;
        if (va.VideoSource == VideoSource.File)
            _manager = new ConsoleCameraManager(_loggerFactory.CreateLogger<ConsoleCameraManager>());

        var stream = $"WeldingRecognition-{_env.HostName}/{va.CameraNumber ?? 0}";
        _logger.LogInformation($"Welding recognition service initialzied. Subscrbing to: {stream}");
        _plumber.SubscribeStateEventHandler(this._model,
            stream,
            FromRelativeStreamPosition.End - 1,
            false);

        _ = Task.Factory.StartNew(OnSendCommand, TaskCreationOptions.LongRunning);
    }
    private async Task OnSendCommand()
    {
        try
        {
            while (!_cts.IsCancellationRequested)
            {
                var cmd = await _channel.Reader.ReadAsync(_cts.Token);
                await _manager.ProcessAsync(cmd, _address.CameraNumber ?? 0);
            }
        }
        catch (OperationCanceledException) { }
    }


    private DarkBrightPixels _count0;
    private DarkBrightPixels _count1;
    private readonly Rectangle _area;
    private volatile int isRunning = 0;
    private long _skippedFrames = 0;

    //private List<DarkBrightPixels> _tmp = new List<DarkBrightPixels>();
    public void Handle(YuvFrame frame, YuvFrame? prv, ulong seq, CancellationToken token, object st)
    {
        // Making sure we are not processing in parallel.
        if (Interlocked.Increment(ref isRunning) > 1)
        {
            Interlocked.Decrement(ref isRunning);
            Interlocked.Increment(ref _skippedFrames);
            return;
        }

        Rectangle r = new Rectangle(0, 0, frame.Info.Width, frame.Info.Height);
        r.Inflate(-400, -400);

        var count = frame.CountPixelsOutsideRange(20, 200, r);
        //_tmp.Add(count);

        if (prv == null)
        {
            _count0 = count;
            Interlocked.Decrement(ref isRunning);
            //Console.Write("Exit prv");
            return;
        }

        if ((frame.Metadata.FrameNumber & 0x1UL) == 1UL)
            _count1 = count; // it this is frame nr. 1,3,5,7 ...
        else _count0 = count; // for frame nr. 2,4,6, ...

        var px = DarkBrightPixels.Min(_count0, _count1);
        //Debug.WriteLine(px);

        _bufferBrightPixels.AddLast(px.BrightPixels);
        _bufferDarkPixels.AddLast(px.DarkPixels);

        if (!_model.DetectionEnabled)
        {
            Interlocked.Decrement(ref isRunning);
            //Console.WriteLine("D");
            return;
        }

        var areaSizeInPixels = r.Area();
        if (!IsWelding)
        {
            var avg = _bufferBrightPixels.Average();
            if (avg > _model.WeldingBound * 0.01 * areaSizeInPixels)
            {
                _logger.LogInformation("Welding detected");
                IsWelding = true;

                var camParams = new SetCameraParameters();
                camParams.CopyFrom(_profileProvider.Welding.Profile);
                CurrentAppliedProfile = camParams;
                _channel.Writer.TryWrite(camParams);

                //Console.WriteLine("Welding detected");
            }
            else
            {
                //Console.WriteLine($"avg ({avg}) < _model.WeldingBound*0.01*areaSizeInPixels");
            }
        }
        else
        {
            // It must be welding
            var avg = _bufferDarkPixels.Average();
            if (avg > _model.NonWeldingBound * 0.01 * areaSizeInPixels)
            {
                _logger.LogInformation("Welding not detected");
                _logger.LogInformation($"OnDetectWelding: {px}");
                IsWelding = false;

                var camParams = new SetCameraParameters();
                camParams.CopyFrom(_profileProvider.Default.Profile);
                CurrentAppliedProfile = camParams;
                _channel.Writer.TryWrite(camParams);
                //Console.WriteLine("Welding not detected");
            }
            else
            {
                //Console.WriteLine($"avg ({avg}) > _model.NonWeldingBound*0.01*areaSizeInPixels");
            }
        }
        Interlocked.Decrement(ref isRunning);
        //Console.WriteLine("Done");
        //if ((seq % 30) == 0)
        //{
        //    StringBuilder sb = new StringBuilder($"Camera: {_address.CameraNumber ?? 0}\nSkipped frames: {_skippedFrames}\n");
        //    DarkBrightPixels tmp = new DarkBrightPixels();
        //    var c = _tmp.Count;
        //    for (int i = c - 1; i >= 0; i--)
        //        tmp += _tmp[i];
            
        //    _tmp.Clear();
        //    sb.AppendLine($"[{c}]: {tmp}");
        //    Console.WriteLine(sb.ToString());
        //}
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
public static class YuvFrameWeldingRecognitionUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe DarkBrightPixels CountPixelsOutsideRange(this YuvFrame frame, int low, int high)
    {
        int darkPixels = 0;
        int brightPixels = 0;
        var end = frame.Data + frame.Info.Width * frame.Info.Height;
        for (byte* p = frame.Data; p != end; p += 1)
        {
            var value = *p;
            if (value > high)
                brightPixels++;

            if (value < low)
                darkPixels++;
        }
        return new DarkBrightPixels(darkPixels, brightPixels);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe DarkBrightPixels CountPixelsOutsideRange(this YuvFrame frame, int low, int high, Rectangle r)
    {
        int darkPixels = 0;
        int brightPixels = 0;

        for (int ix = 0; ix < r.Width; ix++)
            for (int iy = 0; iy < r.Height; iy++)
            {
                var value = frame.Y(ix + r.X, iy + r.Y);

                if (value > high)
                    brightPixels++;

                if (value < low)
                    darkPixels++;
            }
        return new DarkBrightPixels(darkPixels, brightPixels);
    }
}
public readonly record struct DarkBrightPixels(int DarkPixels, int BrightPixels)
{

    public static DarkBrightPixels operator +(DarkBrightPixels a, DarkBrightPixels b)
    {
        return new DarkBrightPixels(a.DarkPixels + b.DarkPixels, a.BrightPixels + b.BrightPixels);
    }

    public static DarkBrightPixels operator -(DarkBrightPixels a, DarkBrightPixels b)
    {
        return new DarkBrightPixels(a.DarkPixels - b.DarkPixels, a.BrightPixels - b.BrightPixels);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DarkBrightPixels Min(DarkBrightPixels a, DarkBrightPixels b)
    {
        return new DarkBrightPixels(Math.Min(a.DarkPixels, b.DarkPixels), Math.Min(a.BrightPixels, b.BrightPixels));
    }
}