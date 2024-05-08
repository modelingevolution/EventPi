using MicroPlumberd;

namespace EventPi.Services.Camera.Contract;

[OutputStream("CameraParameters")]
public record CameraParametersState : ICameraParametersReadOnly
{
    public static string FullStreamName(string hostname) => $"CameraParameters-{StreamId(hostname)}";
    public static string StreamId(string hostname) => hostname;
    public int Shutter { get; init; }
    public float AnalogueGain { get; init; }
    public float DigitalGain { get; init; }
    public float Contrast { get; init; }
    public float Sharpness { get; init; }
    public float Brightness { get; init; }
    public float BlueGain { get; init; }
    public float RedGain { get; init; }
}