using EventPi.Services.Camera.Contract;
using MicroPlumberd;

namespace EventPi.Services.Camera;

[OutputStream("CameraProfile")]
public record CameraProfile : ICameraParametersReadOnly
{
    public static string FullStreamName(string hostname, string profileName) => $"CameraProfile-{StreamId(hostname, profileName)}";
    public static string StreamId(string hostname, string profileName) => $"{hostname}/{profileName}";
    public string Hostname { get; set; }
    public int Shutter { get; set; }
    public float DigitalGain { get; set; }
    public float Brightness { get; set; }
    public float Contrast { get; set; }
    public float Sharpness { get; set; }
    public int CameraId { get; set; }
    public string Profile { get; set; }
    public float BlueGain { get; set; }
    public float RedGain { get; set; }
    public float AnalogueGain { get; set; }
}