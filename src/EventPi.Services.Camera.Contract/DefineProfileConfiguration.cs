using ModelingEvolution.Drawing;
using System.Diagnostics.CodeAnalysis;

namespace EventPi.Services.Camera.Contract;

public record SetCameraHistogramFilter
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public byte[] Values { get; set; }
}
public record DefineProfileCameraHistogramFilter
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Point<double>[] Points { get; set; }
}


public class DefineProfileCameraParameters : ICameraParameters
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int Shutter { get; set; }
    public HdrModeEnum HdrMode { get; set; }
    public float ExposureLevel { get; set; }
    public float DigitalGain { get; set; }
    public float Brightness { get; set; }
    public float BlueGain { get; set; }
    public float RedGain { get; set; }
    public float AnalogueGain { get; set; }
    public float Contrast { get; set; }
    public float Sharpness { get; set; }
    public int CameraId { get; set; }

    
    public DefineProfileCameraParameters CopyFrom(ICameraParametersReadOnly src)
    {
        Shutter = src.Shutter;
        ExposureLevel = src.ExposureLevel;
        HdrMode = src.HdrMode;
        DigitalGain = src.DigitalGain;
        Brightness = src.Brightness;
        BlueGain = src.BlueGain;
        RedGain = src.RedGain;
        AnalogueGain = src.AnalogueGain;
        Contrast = src.Contrast;
        Sharpness = src.Sharpness;

        return this;
    }
}