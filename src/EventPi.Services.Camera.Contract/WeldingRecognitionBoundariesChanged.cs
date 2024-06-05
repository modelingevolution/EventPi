namespace EventPi.Services.Camera.Contract;

public record WeldingRecognitionConfigurationChanged
{
    public int WeldingValue { get; set; }
    public int NonWeldingValue { get; set; }
    public bool DetectionEnabled{ get; set; }

    public int BrightPixelsBorder { get; set; }
    public int DarkPixelsBorder { get; set; }
}