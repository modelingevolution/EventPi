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

    public bool IsWelding { get; private set; }
    public bool DetectionEnabled { get;  set; }
    public int WeldingBound { get;  set; }
    public int NonWeldingBound { get;  set; }
    public ICameraParametersReadOnly CurrentAppliedProfile { get; private set; }

    public ICameraParametersReadOnly WeldingProfile { get; set; }
    public ICameraParametersReadOnly NonWeldingProfile { get; set; }


public WeldingRecognitionService(ILogger<WeldingRecognitionService> logger,GrpcCppCameraProxy proxy, GrpcFrameFeaturesService gprc, WeldingRecognitionModel model, CameraProfileConfigurationModel cameraModel)
    {
        CurrentAppliedProfile = new CameraProfile();
        _logger = logger;
        _grpcService = gprc;
        _grpcService.OnFrameFeaturesAppeared += OnDetectWelding;
        _proxy = proxy;
     
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
        if (!DetectionEnabled) return;

        if (_bufferBrightPixels.Average() > WeldingBound && !IsWelding)
        {
            IsWelding = true;
            _logger.LogInformation("Welding detected");
            var camParams = new SetCameraParameters();
           // camParams.CopyFrom(_cameraModel.WeldingProfile);
            camParams.CopyFrom(WeldingProfile);
            _channel.Writer.WriteAsync(camParams);
            CurrentAppliedProfile = camParams;
        }
        else
        {
            if (_bufferDarkPixels.Average() > NonWeldingBound && IsWelding)
            {
                _logger.LogInformation("Welding not detected");
                IsWelding = false;
                var camParams = new SetCameraParameters();
                // camParams.CopyFrom(_cameraModel.DefaultProfile);
                camParams.CopyFrom(NonWeldingProfile);
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