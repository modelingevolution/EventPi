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
    private readonly GrpcCppCameraProxy _proxy;
    private readonly Channel<SetCameraParameters> _channel;
    private readonly CircularBuffer<int> _bufferBrightPixels;
    private readonly CircularBuffer<int> _bufferDarkPixels;
    private readonly WeldingRecognitionModel _recognitionModel;
    private readonly CameraProfileConfigurationModel _cameraModel;

    public bool IsWelding { get; private set; }
    public ICameraParametersReadOnly CurrentAppliedProfile { get; private set; }

    public double KP { get; set; }
    public double KD { get; set; }
    public double KI { get; set; }
    public double OutputLowerLimit { get; set; }
    public double OutputUpperLimit { get; set; }

    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
   
    public WeldingRecognitionService(ILogger<WeldingRecognitionService> logger,GrpcCppCameraProxy proxy, GrpcFrameFeaturesService gprc, WeldingRecognitionModel model, CameraProfileConfigurationModel cameraModel)
    {
        CurrentAppliedProfile = new CameraProfile();
        _logger = logger;
        _grpcService = gprc;
        _grpcService.OnFrameFeaturesAppeared += OnDetectWelding;
        _proxy = proxy;
        _recognitionModel = model;
        _cameraModel = cameraModel;
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
            camParams.CopyFrom(_cameraModel.WeldingProfile);
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
                camParams.CopyFrom(_cameraModel.DefaultProfile);
                _channel.Writer.WriteAsync(camParams);
                CurrentAppliedProfile = camParams;
            }
        }
      
    }

   
}