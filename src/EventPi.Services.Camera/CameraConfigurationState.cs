using System.Diagnostics.CodeAnalysis;
using EventPi.Services.Camera.Contract;

namespace EventPi.Services.Camera;

public record CameraConfigurationProfile : ICameraParametersReadOnly
{
    public CameraConfigurationProfile()
    {
        
    }
    public CameraConfigurationProfile(CameraProfile ev)
    {
        
        Shutter = ev.Shutter;
        AnalogueGain = ev.AnalogueGain;
        DigitalGain = ev.DigitalGain;
        Brightness = ev.Brightness;
        Contrast = ev.Contrast;
        Sharpness = ev.Sharpness;
        CameraId = ev.CameraId;
        HdrMode = ev.HdrMode;
        ExposureLevel = ev.ExposureLevel;
        BlueGain = ev.BlueGain;
        RedGain = ev.RedGain;
        AutoHistogramEnabled = ev.AutoHistogramEnabled;

        
    }

    public HdrModeEnum HdrMode { get; set; }
    public bool AutoHistogramEnabled { get; set; }
    public int Shutter { get; set; }
    public float ExposureLevel { get; set; }
    public float DigitalGain { get; set; }
    public float Brightness { get; set; }
    public float Contrast { get; set; }
    public float Sharpness { get; set; }
    public int CameraId { get; set; }
    public float BlueGain { get; set; }
    public float RedGain { get; set; }
    public float AnalogueGain { get; set; }


}