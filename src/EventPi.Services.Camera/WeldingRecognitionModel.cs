using EventPi.Abstractions;
using EventPi.Services.Camera.Contract;
using EventPi.Services.CameraFrameFeaturesConfigurator;
using MicroPlumberd;
using Microsoft.Extensions.Hosting;

namespace EventPi.Services.Camera;

[EventHandler]
public partial class CameraProfileModel
{
    public CameraProfile Profile { get; private set; }
    private async Task Given(Metadata m, CameraProfile ev)
    {
        Profile = ev;
    }
}
public class WeldingRecognitionProvider(IPlumber plumber, IEnvironment env) : BackgroundService
{
    private ISubscriptionRunner _camProfileDefaultSub;
    private ISubscriptionRunner _camProfileWeldingtSub;
    public CameraProfileModel Default { get; } = new();
    public CameraProfileModel Welding { get; } = new();

    private async Task Initialize(CancellationToken cts)
    {
        var defaultStreamId = HostProfilePath.Create(env.HostName!, "default");
        var weldingStreamId = HostProfilePath.Create(env.HostName!, "welding");
        await (_camProfileDefaultSub = plumber.Subscribe(CameraProfile.FullStreamName(defaultStreamId), FromRelativeStreamPosition.End - 1, cancellationToken: cts))
        .WithSnapshotHandler(Default);

        await (_camProfileWeldingtSub = plumber.Subscribe(CameraProfile.FullStreamName(weldingStreamId), FromRelativeStreamPosition.End - 1, cancellationToken: cts))
      .WithSnapshotHandler(Welding);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Initialize(stoppingToken);
    }

}


[EventHandler]
public partial class WeldingRecognitionModel
{
    public int WeldingBound { get; private set; }
    public int NonWeldingBound { get; private set; }
    public bool DetectionEnabled{ get; private set; }
    public int DarkPixelsBorder { get; private set; }
    public int BrightPixelsBorder { get; private set; }

  
     private async Task Given(Metadata m, WeldingRecognitionConfigurationState ev)
    {
        WeldingBound = ev.WeldingValue;
        NonWeldingBound = ev.NonWeldingValue;
        DetectionEnabled = ev.DetectionEnabled;
        DarkPixelsBorder = ev.DarkPixelsBorder;
        BrightPixelsBorder = ev.BrightPixelsBorder;
    }
}