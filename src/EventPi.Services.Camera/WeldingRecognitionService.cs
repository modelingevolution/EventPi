using EventPi.Services.Camera.Contract;
using Microsoft.Extensions.Logging;

namespace EventPi.Services.Camera;

public class WeldingRecognitionService
{
    private readonly ILogger<WeldingRecognitionService> _logger;
    private readonly GrpcFrameFeaturesService _grpcService;

    public bool IsWelding { get; set; }
    private readonly GrpcCppCameraProxy _proxy;
    private SetCameraParameters _cameraParameters = new SetCameraParameters();
    public ICameraParametersReadOnly DefaultProfile { get; set; }
    public ICameraParametersReadOnly WeldingProfile { get; set; }
    public bool TryDetect { get; set; }
    public WeldingRecognitionService(ILogger<WeldingRecognitionService> logger,GrpcCppCameraProxy proxy, GrpcFrameFeaturesService gprc)
    {
        _logger = logger;
        _grpcService = gprc;
        _grpcService.OnFrameFeaturesAppeared += OnDetectWelding;
        _proxy = proxy;
        WeldingProfile = new CameraProfile();
        DefaultProfile = new CameraProfile();
    }

    private void OnDetectWelding(object? sender, FrameFeaturesRecord e)
    {
        if (!TryDetect) return;

        if (e.TotalBrightPixels > 400 * 400 * 0.8 && !IsWelding)
        {
            IsWelding = true;
            _logger.LogInformation("Welding detected");
            _proxy.ProcessAsync(_cameraParameters.CopyFrom(WeldingProfile));
          
        }
        else
        {
            if (e.TotalDarkPixels > 400 * 400 * 0.8 && IsWelding)
            {
                _logger.LogInformation("Welding not detected");
                IsWelding = false;
                _proxy.ProcessAsync(_cameraParameters.CopyFrom(DefaultProfile));
            }
        }
      
    }
}