using MicroPlumberd;

namespace EventPi.Services.Camera.Contract;


[OutputStream("WeldingRecognition")]
public record WeldingRecognitionConfigurationState : IWeldingRecognitionConfigurationParameters
{
    public static string FullStreamName(string hostname, int cameraNr) => $"WeldingRecognition-{StreamId(hostname,cameraNr)}";
    public static string StreamId(string hostname, int cameraNr) => $"{hostname}/{cameraNr}";
    public int WeldingValue { get; set; }
    public int NonWeldingValue { get; set; }
    public bool DetectionEnabled{ get; set; }

    public int BrightPixelsBorder { get; set; }
    public int DarkPixelsBorder { get; set; }
}