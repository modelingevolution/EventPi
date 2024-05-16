using EventPi.Pid;
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
    public double DetectWeldingBound { get; set; }
    public double WeldingBrightPixelsTarget { get; set; }
    public double CurrentAppliedShutter { get; set; }
    public double DetectNonWeldingBound { get; set; }
    public double KP { get; set; }
    public double KD { get; set; }
    public double KI { get; set; }
    public double OutputLowerLimit { get; set; }
    public double OutputUpperLimit { get; set; }
    public bool TryDetect { get; set; }
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
    }

    private void OnDetectWelding(object? sender, FrameFeaturesRecord e)
    {
        if (!TryDetect) return;

        if (e.TotalBrightPixels > DetectWeldingBound && !IsWelding)
        {
            IsWelding = true;
            _logger.LogInformation("Welding detected");
            _cameraParameters = new SetCameraParameters();
            _cameraParameters.CopyFrom(WeldingProfile);
            _proxy.ProcessAsync(_cameraParameters);
            CurrentAppliedShutter = _cameraParameters.Shutter;
        }
        else
        {
            if (e.TotalDarkPixels > DetectNonWeldingBound && IsWelding)
            {
                _logger.LogInformation("Welding not detected");
                IsWelding = false;
                _cameraParameters = new SetCameraParameters();
                _cameraParameters.CopyFrom(DefaultProfile);
                _proxy.ProcessAsync(_cameraParameters);
                CurrentAppliedShutter = _cameraParameters.Shutter;
            }
        }

        if (IsWelding)
        {
            PidController pid = new PidController(KP, KD, KI, OutputUpperLimit, OutputLowerLimit);
            var result =pid.CalculateOutput(WeldingBrightPixelsTarget,e.TotalBrightPixels, TimeSpan.FromSeconds(1));

            _cameraParameters.Shutter += (int)result;
            _proxy.ProcessAsync(_cameraParameters);
            CurrentAppliedShutter = _cameraParameters.Shutter;
        }
      
    }
}