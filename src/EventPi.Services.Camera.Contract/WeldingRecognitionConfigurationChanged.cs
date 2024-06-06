using MicroPlumberd;

namespace EventPi.Services.Camera.Contract;

[OutputStream("WeldingRecognition")]
public record WeldingRecognitionConfigurationState : IWeldingRecognitionConfigurationParameters
{
    public static string FullStreamName(string hostname) => $"WeldingRecognition-{StreamId(hostname)}";
    public static string StreamId(string hostname) => hostname;
    public int WeldingValue { get; set; }
    public int NonWeldingValue { get; set; }
    public bool DetectionEnabled{ get; set; }

    public int BrightPixelsBorder { get; set; }
    public int DarkPixelsBorder { get; set; }
}