using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using EventPi.Services.Camera.Contract;
using MicroPlumberd;


namespace EventPi.Services.Camera;


public class DefineProfileConfiguration : ICameraParameters
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int Shutter { get; set; }
    public float DigitalGain { get; set; }
    public float Brightness { get; set; }
    public float BlueGain { get; set; }
    public float RedGain { get; set; }
    public float AnalogueGain { get; set; }
    public float Contrast { get; set; }
    public float Sharpness { get; set; }
    public int CameraId { get; set; }
    [NotNull]
    public string Hostname { get; set; }
    [NotNull]
    public string Profile { get; set; }

    public DefineProfileConfiguration CopyFrom(ICameraParametersReadOnly src)
    {
        Shutter = src.Shutter;
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