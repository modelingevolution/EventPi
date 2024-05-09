

using EventPi.Abstractions;
using MicroPlumberd;
using ModelingEvolution.Drawing;

namespace EventPi.Services.Camera.Contract;

[OutputStream("CameraHistogramFilter")]
public record CameraHistogramFilter : IStatefulStream<HostProfilePath>
{
    public static string FullStreamName(HostProfilePath id) => $"CameraHistogramFilter-{id}";
    
    public Point<double>[] Points { get; set; }
    
}
[OutputStream("CameraProfile")]
public record CameraProfile : ICameraParametersReadOnly, IStatefulStream<HostProfilePath>
{
    public static string FullStreamName(HostProfilePath id) => $"CameraProfile-{id}";
    
    public int Shutter { get; set; }
    public float DigitalGain { get; set; }
    public float Brightness { get; set; }
    public float Contrast { get; set; }
    public float Sharpness { get; set; }
    public int CameraId { get; set; }
    public float BlueGain { get; set; }
    public float RedGain { get; set; }
    public float AnalogueGain { get; set; }
}