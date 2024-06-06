using EventPi.Services.Camera.Contract;
using MicroPlumberd;

namespace EventPi.Services.Camera;

public class WeldingRecognitionVm
{
    private readonly WeldingRecognitionModel _model;
    private readonly ICommandBus _bus;
    private bool _initialized;
    private string _hostName;

    public int WeldingBound => _model.WeldingBound;
    public int NonWeldingBound => _model.NonWeldingBound;
    public bool DetectionEnabled => _model.DetectionEnabled;
    public int DarkPixelsBorder => _model.DarkPixelsBorder;
    public int BrightPixelsBorder => _model.BrightPixelsBorder;

    public WeldingRecognitionVm(WeldingRecognitionModel model, ICommandBus bus)
    {
        _model = model;
        _bus = bus;
    }

    public async Task OnSendConfiguration(int brightPixelsBorder, int darkPixelsBorder, int weldingBound,
        int nonWeldingBound, bool detectionEnabled)
    {
        SetWeldingRecognitionConfiguration cmd = new SetWeldingRecognitionConfiguration()
        {
            BrightPixelsBorder = brightPixelsBorder,
            DarkPixelsBorder = darkPixelsBorder,
            DetectionEnabled = detectionEnabled,
            WeldingValue = weldingBound,
            NonWeldingValue = nonWeldingBound
        };
        await _bus.SendAsync(WeldingRecognitionConfigurationState.FullStreamName(_hostName), cmd);
    }

    public async Task Initialize(string hostName)
    {
        if (_initialized)
        {
            if (_hostName != hostName)
                throw new InvalidOperationException();
            return;
        }
        _initialized = true;
    }

}