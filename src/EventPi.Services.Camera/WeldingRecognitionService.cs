using EventPi.Abstractions;
using EventPi.Pid;
using EventPi.Services.Camera.Contract;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using WeldingAutomation.CameraAutoShutter;

namespace EventPi.Services.Camera;

public class WeldingRecognitionService
{
    private readonly ILogger<WeldingRecognitionService> _logger;
    private readonly GrpcFrameFeaturesService _grpcService;

    public bool IsWelding { get; set; }
    private readonly GrpcCppCameraProxy _proxy;
    private SetCameraParameters _cameraParameters = new SetCameraParameters();
    private readonly Channel<SetCameraParameters> _channel;
    public ICameraParametersReadOnly DefaultProfile { get; set; }
    public ICameraParametersReadOnly WeldingProfile { get; set; }
    private CircularBuffer<int> _bufferBrightPixels;
    private CircularBuffer<int> _bufferDarkPixels;
    public double DetectWeldingBound { get; set; }
    public double WeldingBrightPixelsTarget { get; set; }
    public double CurrentAppliedShutter { get; set; }
    public double DetectNonWeldingBound { get; set; }
    public double KP { get; set; }
    public double KD { get; set; }
    public double KI { get; set; }
    public double OutputLowerLimit { get; set; }
    public double OutputUpperLimit { get; set; }

    public int BrightBorder
    {
        get => _brightBorder;
        set
        {
            if (_brightBorder != value)
            {
                _brightBorder = value;
                _proxy.ProcessAsync(new CameraAutoShutterRequest()
                {
                    LowerBound = _darkBorder,
                    UpperBound = _brightBorder
                });
            }
        }
    }

    public int DarkBorder
    {
        get => _darkBorder;
        set
        {
            if (_darkBorder != value)
            {
                _darkBorder = value;
                _proxy.ProcessAsync(new CameraAutoShutterRequest()
                    { LowerBound = _darkBorder,
                        UpperBound = _brightBorder });
            }
        }
    }

    public bool TryDetect { get; set; }
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private int _darkBorder;
    private int _brightBorder;
    private int _shutterOffset;

    public WeldingRecognitionService(ILogger<WeldingRecognitionService> logger,GrpcCppCameraProxy proxy, GrpcFrameFeaturesService gprc)
    {
        _logger = logger;
        _grpcService = gprc;
        _grpcService.OnFrameFeaturesAppeared += OnDetectWelding;
        _proxy = proxy;
        WeldingProfile = new CameraProfile();
        DefaultProfile = new CameraProfile();
        DetectWeldingBound = 400 * 400 * 0.8;
        DetectNonWeldingBound = 400 * 400 * 0.8;
        KP = 0.01;
        OutputLowerLimit = -100;
        OutputUpperLimit = 100;
        _darkBorder = 20;
        _brightBorder = 200;
        _bufferBrightPixels = new CircularBuffer<int>(3);
        _bufferDarkPixels = new CircularBuffer<int>(3);
        _channel = Channel.CreateBounded<SetCameraParameters>(new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropOldest });
        Task.Factory.StartNew(OnSendCommand, TaskCreationOptions.LongRunning);
    }

    private async Task OnSendCommand()
    {
        try
        {
            while (!_cts.IsCancellationRequested)
            {
                var cmd = await _channel.Reader.ReadAsync(_cts.Token);
                await _proxy.ProcessAsync(cmd);
            }
        }
        catch (OperationCanceledException) { }
    }

    private void OnDetectWelding(object? sender, FrameFeaturesRecord e)
    {
        _bufferBrightPixels.AddLast(e.TotalBrightPixels);
        _bufferDarkPixels.AddLast(e.TotalDarkPixels);
        if (!TryDetect) return;

        if (_bufferBrightPixels.Average() > DetectWeldingBound && !IsWelding)
        {
            IsWelding = true;
            _logger.LogInformation("Welding detected");
            var camParams = new SetCameraParameters();
            camParams.CopyFrom(WeldingProfile);
            _channel.Writer.WriteAsync(camParams);
            _shutterOffset = 0;
            CurrentAppliedShutter = camParams.Shutter;
        }
        else
        {
            if (_bufferDarkPixels.Average() > DetectNonWeldingBound && IsWelding)
            {
                _logger.LogInformation("Welding not detected");
                IsWelding = false;
                var camParams = new SetCameraParameters();
                camParams.CopyFrom(DefaultProfile);
                _channel.Writer.WriteAsync(camParams);
                _shutterOffset = 0;
                CurrentAppliedShutter = camParams.Shutter;
            }
        }

        if (IsWelding)
        {
            PidController pid = new PidController(KP, KD, KI, OutputUpperLimit, OutputLowerLimit);
            var result =pid.CalculateOutput(WeldingBrightPixelsTarget,e.TotalBrightPixels, TimeSpan.FromSeconds(1));

            _shutterOffset += (int)result;
          
            var camParams = new SetCameraParameters();
            camParams.CopyFrom(WeldingProfile);
            camParams.Shutter += _shutterOffset;
            _channel.Writer.WriteAsync(camParams);
            CurrentAppliedShutter = _cameraParameters.Shutter;
        }
      
    }
}