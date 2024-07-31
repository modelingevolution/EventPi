using EventPi.Abstractions;
using EventPi.Pid;
using EventPi.Services.Camera.Contract;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using ModelingEvolution.VideoStreaming;

namespace EventPi.Services.Camera;

public class WeldingRecognitionService : BackgroundService
{
    private readonly ILogger<WeldingRecognitionService> _logger;
 
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
    private CancellationToken token = new CancellationToken();

public WeldingRecognitionService(ILogger<WeldingRecognitionService> logger,GrpcCppCameraProxy proxy,  WeldingRecognitionModel model, CameraProfileConfigurationModel cameraModel)
    {
        CurrentAppliedProfile = new CameraProfile();
        _logger = logger;
       // _grpcService = gprc;
       FrameProcessingHandlers.OnFrameMerged += OnDetectWelding;
        _proxy = proxy;
     
        _bufferBrightPixels = new CircularBuffer<int>(3);
        _bufferDarkPixels = new CircularBuffer<int>(3);
        _channel = Channel.CreateBounded<SetCameraParameters>(new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropOldest });
       
    }

    private async Task OnSendCommand()
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                var cmd = await _channel.Reader.ReadAsync(token);
               // await _proxy.ProcessAsync(cmd);
            }
        }
        catch (OperationCanceledException) { }
    }

    private void OnDetectWelding(object? sender, byte[] e)
    {
        var totalBrightPixels = 0;
        var totalDarkPixels = 0;
        foreach (var value in e)
        {
            if(value >200)
            {
                totalBrightPixels++;
            }
            if(value< 20)
            {
                totalDarkPixels++;
            }
        }
        _bufferBrightPixels.AddLast(totalBrightPixels);
        _bufferDarkPixels.AddLast(totalDarkPixels);
        if (!DetectionEnabled) return;

        if (_bufferBrightPixels.Average() > WeldingBound && !IsWelding)
        {
            IsWelding = true;
            _logger.LogInformation("Welding detected");
            var camParams = new SetCameraParameters();
           // camParams.CopyFrom(_cameraModel.WeldingProfile);
            camParams.CopyFrom(WeldingProfile);
             _proxy.ProcessAsync(camParams);
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
                _proxy.ProcessAsync(camParams);
                _channel.Writer.WriteAsync(camParams);
                CurrentAppliedProfile = camParams;
            }
        }
      
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Factory.StartNew(OnSendCommand, TaskCreationOptions.LongRunning);
      
    }
}