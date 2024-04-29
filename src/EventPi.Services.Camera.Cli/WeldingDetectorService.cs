using EventPi.Abstractions;
using MicroPlumberd.Services;
using ModelingEvolution.Plumberd.StateTransitioning;
using ProtoBuf;
using System.Collections.Concurrent;

namespace EventPi.Services.Camera.Cli;

public class WeldingDetectorService
{
    private readonly ILogger<WeldingDetectorService> _logger;
    private readonly GrpcFrameFeaturesService _frameFeatures;
    private readonly GrpcCppCameraProxy _proxy;
   
    private readonly CameraWeldingConfigurationModel _model;
    public bool IsWelding { get; private set; }
   


    public WeldingDetectorService(ILogger<WeldingDetectorService> logger, GrpcFrameFeaturesService frameFeatures, GrpcCppCameraProxy proxy, CameraWeldingConfigurationModel model)
    {
        _logger = logger;
        _proxy = proxy;
        _model = model;
        _proxy.InitProxy();
    
        _frameFeatures = frameFeatures;
        _frameFeatures.OnFrameFeaturesAppeared += DetectWelding;
    }

    public void DetectWelding(object? sender, FrameFeaturesRecord frameFeaturesRecord)
    {
        _model.UpdateBuffers(frameFeaturesRecord);
        
        if (_model.GetBrightPixels() > _model.WeldingBorder && !IsWelding)
        {
            _proxy.ProcessAsync(new CameraState()
            {
                AnalogueGain = 0.0f,
                BlueGain = -1.0f,
                Brightness = 0.0f,
                Contrast = 0.0f,
                DigitalGain = 0.0f,
                RedGain = -1.0f,
                Sharpness = 0.0f,
                Shutter = 1000
            });
            IsWelding = true;
            return;
        }

        if (_model.GetDarkPixels() > _model.NonWeldingBorder && IsWelding)
        {
            _proxy.ProcessAsync(new CameraState()
            {
                AnalogueGain = 0.0f,
                BlueGain = 0.0f,
                Brightness = 0.0f,
                Contrast = 0.0f,
                DigitalGain = 0.0f,
                RedGain = 0.0f,
                Sharpness = 0.0f,
                Shutter = 40000
            });
            IsWelding = false;
            return;
        }
    }
}