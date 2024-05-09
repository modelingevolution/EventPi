using System.Diagnostics.CodeAnalysis;
using EventPi.Services.Camera.Contract;

namespace EventPi.Services.Camera;

public record CameraConfigurationProfile{
    public CameraConfigurationProfile()
    {
        
    }
    public CameraConfigurationProfile(CameraProfile ev)
    {
        
        Shutter = ev.Shutter;
        AnalogGain = ev.AnalogueGain;
        DigitalGain = ev.DigitalGain;
        Brightness = ev.Brightness;
        Contrast = ev.Contrast;
        Sharpness = ev.Sharpness;
        CameraId = ev.CameraId;
        
    }
    
    public int Shutter { get; set; }
    public double AnalogGain { get; set; }
    public double DigitalGain { get; set; }
    public double Brightness { get; set; }
    public double Contrast { get; set; }
    public double Sharpness { get; set; }
    public int CameraId { get; set; }
    

}