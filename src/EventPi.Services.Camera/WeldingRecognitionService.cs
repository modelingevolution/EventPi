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
    WeldingRecognitionVm _model;
    public bool IsWelding { get; private set; }

    public int BrightOffset { get; set; }
    public int DarkOffset { get;  set; }
    public ICameraParametersReadOnly CurrentAppliedProfile { get; private set; }

    public ICameraParametersReadOnly WeldingProfile { get; set; }
    public ICameraParametersReadOnly NonWeldingProfile { get; set; }
    private CancellationToken token = new CancellationToken();

    public WeldingRecognitionService(ILogger<WeldingRecognitionService> logger, GrpcCppCameraProxy proxy, WeldingRecognitionVm model, CameraProfileConfigurationModel cameraModel)
    {
        CurrentAppliedProfile = new CameraProfile();
        _logger = logger;
        // _grpcService = gprc;
        FrameProcessingHandlers.OnFrameMerged += OnDetectWelding;
        _proxy = proxy;
        _model =model;
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
        var middle = e.Length/2;
        var offset = 80000;
        for (int i= middle - offset; i<middle+offset; i++) 
        {
            if (e[i] >200)
            {
                totalBrightPixels++;
            }
            if (e[i]< 20)
            {
                totalDarkPixels++;
            }
        }
       
        _bufferBrightPixels.AddLast(totalBrightPixels);
        _bufferDarkPixels.AddLast(totalDarkPixels);
       // if (!_model.SetWeldingRecognitionConfiguration.DetectionEnabled) return;

        if (_bufferBrightPixels.Average() > (offset*2 * (BrightOffset/100.0))&& !IsWelding)
        {
            IsWelding = true;
            _logger.LogInformation("Welding detected");
            _logger.LogInformation($"OnDetectWelding: BrightPixels:{totalBrightPixels}, DarkPixels:{totalDarkPixels}");
            var camParams = new SetCameraParameters();
            // camParams.CopyFrom(_cameraModel.WeldingProfile);

            camParams.CopyFrom(WeldingProfile);

            _proxy.ProcessAsync(camParams);
            _channel.Writer.WriteAsync(camParams);
            CurrentAppliedProfile = camParams;
        }

        else
        {
            if (_bufferDarkPixels.Average() > (offset*2 * (DarkOffset/100.0)) && IsWelding)
            {
                _logger.LogInformation("Welding not detected");
                _logger.LogInformation($"OnDetectWelding: BrightPixels:{totalBrightPixels}, DarkPixels:{totalDarkPixels}");
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