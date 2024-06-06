using EventPi.Abstractions;
using EventPi.Pid;
using EventPi.Services.Camera.Contract;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;

namespace EventPi.Services.Camera;

public class WeldingRecognitionService : BackgroundService
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

  
   
    public WeldingRecognitionService(ILogger<WeldingRecognitionService> logger,GrpcCppCameraProxy proxy, GrpcFrameFeaturesService gprc, WeldingRecognitionModel model, CameraProfileConfigurationModel cameraModel)
    {
        CurrentAppliedProfile = new CameraProfile();
        _logger = logger;
        _grpcService = gprc;
        _grpcService.OnFrameFeaturesAppeared += OnDetectWelding;
        _proxy = proxy;
        _recognitionModel = model;
        _cameraModel = cameraModel;
        _bufferBrightPixels = new CircularBuffer<int>(3);
        _bufferDarkPixels = new CircularBuffer<int>(3);
        _channel = Channel.CreateBounded<SetCameraParameters>(new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropOldest });
       
    }

    private async Task OnSendCommand(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                var cmd = await _channel.Reader.ReadAsync(token);
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


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Factory.StartNew(()=>OnSendCommand(stoppingToken), TaskCreationOptions.LongRunning);
        return Task.CompletedTask;
    }
}