using EventPi.Abstractions;
using EventPi.Services.Camera.Contract;
using MicroPlumberd;

namespace EventPi.Services.Camera;

[OutputStream("WeldingRecognitionConfiguration")]
public record WeldingRecognitionConfiguration : IStatefulStream<HostName>, IWeldingRecognitionConfigurationParameters
{
    public static string FullStreamName(HostName id) => $"WeldingRecognitionConfiguration-{id}";
    public int WeldingValue { get; set; }
    public int NonWeldingValue { get; set; }
    public bool DetectionEnabled { get; set; }
    public int BrightPixelsBorder { get; set; }
    public int DarkPixelsBorder { get; set; }
}