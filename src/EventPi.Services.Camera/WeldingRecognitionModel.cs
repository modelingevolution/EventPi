using EventPi.Services.Camera.Contract;
using EventPi.Services.CameraFrameFeaturesConfigurator;
using MicroPlumberd;

namespace EventPi.Services.Camera;

[EventHandler]
public partial class WeldingRecognitionModel
{
    public int WeldingBound { get; private set; }
    public int NonWeldingBound { get; private set; }
    public bool DetectionEnabled{ get; private set; }
    public int DarkPixelsBorder { get; private set; }
    public int BrightPixelsBorder { get; private set; }
   
    private async Task Given(Metadata m, WeldingRecognitionConfiguration ev)
    {
        WeldingBound = ev.WeldingValue;
        NonWeldingBound = ev.NonWeldingValue;
        DetectionEnabled = ev.DetectionEnabled;
        DarkPixelsBorder = ev.DarkPixelsBorder;
        BrightPixelsBorder = ev.BrightPixelsBorder;
    }
    private async Task Given(Metadata m, WeldingRecognitionConfigurationState ev)
    {
        WeldingBound = ev.WeldingValue;
        NonWeldingBound = ev.NonWeldingValue;
        DetectionEnabled = ev.DetectionEnabled;
        DarkPixelsBorder = ev.DarkPixelsBorder;
        BrightPixelsBorder = ev.BrightPixelsBorder;
    }
}