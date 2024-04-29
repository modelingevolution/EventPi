using System.ComponentModel.DataAnnotations;
using MicroPlumberd;
using ProtoBuf;

namespace EventPi.Services.Camera;

[OutputStream("Camera")]
public class SetCameraParameters
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Range(1,40000)]
    public int Shutter { get; set; }

    [Range(0, 10)]
    public float AnalogueGain { get; set; }

    [Range(0, 10)]
    public float DigitalGain { get; set; }

    [Range(0, 10)]
    public float Contrast { get; set; }

    [Range(0, 1)]
    public float Sharpness { get; set; }

    [Range(-1,1)]
    public float Brightness { get; set; }

    [Range(-1, 10)]
    public float BlueGain { get; set; }

    [Range(-1, 10)]
    public float RedGain { get; set; }
}