using MicroPlumberd;

namespace EventPi.Services.Camera;

[OutputStream("Camera")]
public record CameraState
{
    public int Shutter { get; set; }
    public float AnalogueGain { get; set; }
    public float DigitalGain { get; set; }
    public float Contrast { get; set; }
    public float Sharpness { get; set; }
    public float Brightness { get; set; }
    public float BlueGain { get; set; }
    public float RedGain { get; set; }
}