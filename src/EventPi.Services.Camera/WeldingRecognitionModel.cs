using EventPi.Abstractions;
using EventPi.Services.Camera.Contract;
using EventPi.Services.CameraFrameFeaturesConfigurator;
using MicroPlumberd;
using Microsoft.Extensions.Hosting;

namespace EventPi.Services.Camera;
[EventHandler]
public partial class WeldingRecognitionProvider(IPlumber plumber, 
    IEnvironment env, 
    ICommandBus bus) : BackgroundService
{
    private ISubscriptionRunner _camProfileDefaultSub;
    private ISubscriptionRunner _camProfileWeldingtSub;
    private ISubscriptionRunner? _camProfileDefaultSubOnStart;
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

        await (_camProfileDefaultSubOnStart = plumber.Subscribe(CameraProfile.FullStreamName(defaultStreamId), FromRelativeStreamPosition.End - 1, cancellationToken: cts))
     .WithSnapshotHandler(this);


    }
    private async Task Given(Metadata m, CameraProfile ev)
    {
        if(_camProfileDefaultSubOnStart != null)
        await _camProfileDefaultSubOnStart.DisposeAsync();
        _camProfileDefaultSubOnStart = null;

        _ = Task.Run(async () =>
        {
            await Task.Delay(5000);
            var camParams = new SetCameraParameters();
            await bus.SendAsync(CameraParametersState.StreamId(env.HostName), camParams.CopyFrom(ev));
        });
      
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
        Console.WriteLine($"==> Welding recognition model: WeldingBound: {WeldingBound}, " +
            $"Non: {NonWeldingBound}, Dark: {DarkPixelsBorder}, Bright: {BrightPixelsBorder}, Enabled: {this.DetectionEnabled}");
    }
}