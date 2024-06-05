

using EventPi.Abstractions;
using MicroPlumberd;
using ModelingEvolution.Drawing;

namespace EventPi.Services.Camera.Contract;

[OutputStream("CameraProfileHistogramFilter")]
public record CameraProfileHistogramFilter : IStatefulStream<HostProfilePath>
{
    public static string FullStreamName(HostProfilePath id) => $"CameraProfileHistogramFilter-{id}";
    
    public Point<double>[] Points { get; set; }
    
}

[OutputStream("CameraHistogramFilter")]
public record CameraHistogramFilter : IStatefulStream<HostName>
{
    public static string FullStreamName(HostName id) => $"CameraHistogramFilter-{id}";

    public byte[] Values { get; set; }

}
[OutputStream("CameraProfile")]
public record CameraProfile : ICameraParametersReadOnly, IStatefulStream<HostProfilePath>
{
    public static string FullStreamName(HostProfilePath id) => $"CameraProfile-{id}";
    
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