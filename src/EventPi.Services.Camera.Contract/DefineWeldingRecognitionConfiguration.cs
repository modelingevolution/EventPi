namespace EventPi.Services.Camera.Contract;

public class DefineWeldingRecognitionConfiguration : IWeldingRecognitionConfigurationParameters
{
    public Guid Id { get; set; } = Guid.NewGuid();


    public int WeldingValue { get; set; }
    public int NonWeldingValue { get; set; }
    public bool DetectionEnabled { get; set; }
    public int BrightPixelsBorder { get; set; }
    public int DarkPixelsBorder { get; set; }

    public DefineWeldingRecognitionConfiguration CopyFrom(IWeldingRecognitionConfigurationParameters src)
    {
       
        WeldingValue = src.WeldingValue;
        NonWeldingValue = src.NonWeldingValue;
        DetectionEnabled = src.DetectionEnabled;
        BrightPixelsBorder = src.BrightPixelsBorder;
        DarkPixelsBorder = src.DarkPixelsBorder;
        return this;
    }

  
}