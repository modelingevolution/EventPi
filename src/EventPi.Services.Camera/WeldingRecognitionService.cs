using EventPi.Abstractions;
using EventPi.Pid;
using EventPi.Services.Camera.Contract;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace EventPi.Services.Camera;

public class WeldingRecognitionService
{
    private readonly ILogger<WeldingRecognitionService> _logger;
    private readonly GrpcFrameFeaturesService _grpcService;

    public bool IsWelding { get; private set; }
    private readonly GrpcCppCameraProxy _proxy;
    private SetCameraParameters _cameraParameters = new SetCameraParameters();
    private readonly Channel<SetCameraParameters> _channel;
    public ICameraParametersReadOnly DefaultProfile { get; set; }
    public ICameraParametersReadOnly WeldingProfile { get; set; }
    private CircularBuffer<int> _bufferBrightPixels;
    private CircularBuffer<int> _bufferDarkPixels;

    private WeldingRecognitionModel _recognitionModel;
  
    public double WeldingBrightPixelsTarget { get; set; }
    public ICameraParametersReadOnly CurrentAppliedProfile { get; set; }

    public double KP { get; set; }
    public double KD { get; set; }
    public double KI { get; set; }
    public double OutputLowerLimit { get; set; }
    public double OutputUpperLimit { get; set; }

    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
   
    public WeldingRecognitionService(ILogger<WeldingRecognitionService> logger,GrpcCppCameraProxy proxy, GrpcFrameFeaturesService gprc)
    {
        CurrentAppliedProfile = new CameraProfile();
        _logger = logger;
        _grpcService = gprc;
        _grpcService.OnFrameFeaturesAppeared += OnDetectWelding;
        _proxy = proxy;
        WeldingProfile = new CameraProfile();
        DefaultProfile = new CameraProfile();
        KP = 0.01;
        OutputLowerLimit = -100;
        OutputUpperLimit = 100;
    
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
        if (!_recognitionModel.DetectionEnabled) return;

        if (_bufferBrightPixels.Average() > _recognitionModel.WeldingBound && !IsWelding)
        {
            IsWelding = true;
            _logger.LogInformation("Welding detected");
            var camParams = new SetCameraParameters();
            camParams.CopyFrom(WeldingProfile);
            _channel.Writer.WriteAsync(camParams);
            CurrentAppliedProfile = camParams;
        }
        else
        {
            if (_bufferDarkPixels.Average() > _recognitionModel.NonWeldingBound && IsWelding)
            {
                _logger.LogInformation("Welding not detected");
                IsWelding = false;
                var camParams = new SetCameraParameters();
                camParams.CopyFrom(DefaultProfile);
                _channel.Writer.WriteAsync(camParams);
                CurrentAppliedProfile = camParams;
            }
        }
      
    }

   
}