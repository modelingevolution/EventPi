namespace EventPi.Services.Camera.Contract;


public record SetWeldingRecognitionConfiguration
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int WeldingValue { get; set; }
    public int NonWeldingValue { get; set; }
    public bool DetectionEnabled { get; set; }

    public int BrightPixelsBorder { get; set; }
    public int DarkPixelsBorder { get; set; }
}