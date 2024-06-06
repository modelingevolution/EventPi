namespace EventPi.Services.Camera.Contract;

public interface IWeldingRecognitionConfigurationParameters
{
    int WeldingValue { get; }
    int NonWeldingValue { get; }
    bool DetectionEnabled { get; }
    int BrightPixelsBorder { get; }
    int DarkPixelsBorder { get; }
}