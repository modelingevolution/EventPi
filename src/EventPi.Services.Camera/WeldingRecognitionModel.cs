using EventPi.Abstractions;
using EventPi.Services.Camera.Contract;
using EventPi.Services.CameraFrameFeaturesConfigurator;
using MicroPlumberd;

namespace EventPi.Services.Camera;

[EventHandler]
public partial class WeldingRecognitionModel
{
    private readonly IPlumber _plumber;
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public int WeldingBound { get; private set; }
    public int NonWeldingBound { get; private set; }
    public bool DetectionEnabled{ get; private set; }
    public int DarkPixelsBorder { get; private set; }
    public int BrightPixelsBorder { get; private set; }
    private readonly GrpcCppCameraProxy _proxy;
    public WeldingRecognitionModel(IPlumber plumber,GrpcCppCameraProxy proxy)
    {
        _plumber = plumber;
        _proxy = proxy;
    }
    public async Task Initialize(HostName hostName)
    {
        await _plumber.Subscribe(WeldingRecognitionConfiguration.FullStreamName(hostName), FromRelativeStreamPosition.End - 1, cancellationToken: _cts.Token)
            .WithSnapshotHandler(this);
    }
    private async Task Given(Metadata m, WeldingRecognitionConfigurationChanged ev)
    {
        WeldingBound = ev.WeldingValue;
        NonWeldingBound = ev.NonWeldingValue;
        DetectionEnabled = ev.DetectionEnabled;
        DarkPixelsBorder = ev.DarkPixelsBorder;
        BrightPixelsBorder = ev.BrightPixelsBorder;

        await _proxy.ProcessAsync(new CameraFrameFeaturesConfiguratorRequest()
        {
            BrightPixelsBorder = BrightPixelsBorder,
            DarkPixelsBorder = DarkPixelsBorder
        });
    }
  
}