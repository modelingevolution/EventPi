using System.Threading.Channels;
using EventPi.Abstractions;
using EventPi.NetworkMonitor;
using MicroPlumberd;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using DeviceState = EventPi.Services.NetworkMonitor.Contract.DeviceState;

namespace EventPi.Services.NetworkMonitor;

internal class NetworkManagerListener(IPlumber plumber, IEnvironment env, ILogger<NetworkManagerListener> log) : BackgroundService
{
    private NetworkManagerClient? _client;
    private Channel<Func<CancellationToken,Task>> _channel;
    private CancellationTokenSource _cts;
    private Disposables _d = new();
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        log.LogInformation("Starting NetworkManager listener");
        _channel = Channel.CreateBounded<Func<CancellationToken, Task>>(new BoundedChannelOptions(5){ FullMode = BoundedChannelFullMode.DropOldest});
        _client = await NetworkManagerClient.Create();
        await AppendStations(stoppingToken);
        await AppendWifiProfiles(stoppingToken);
        await AppendConnectivity(token:stoppingToken);
        var wifis = await _client.GetDevices().OfType<WifiDeviceInfo>().ToArrayAsync();
        
        foreach (var i in wifis)
        {
            i.StateChanged += OnWifiDeviceStateChanged;
            i.AccessPointVisilibityChanged += OnWifiAccessPointChanged;
            i.AccessPointSignalChanged += OnWifiSignalChanged;
            _d += await i.SubscribeStateChanged();
            _d += await i.SubscribeAccessPoint();
        }

        await Task.Factory.StartNew(OnStateAppender, TaskCreationOptions.LongRunning);
    }

    private void OnWifiSignalChanged(object? sender, AccessPointPropertyChangedArgs e)
    {
        _channel.Writer.WriteAsync(this.AppendStations);
    }

    private void OnWifiAccessPointChanged(object? sender, AccessPointDiscoveryArgs e)
    {
        _channel.Writer.WriteAsync(this.AppendStations);
       
    }

    private async Task OnStateAppender()
    {
        await Task.Delay(100);
        await foreach (var i in _channel.Reader.ReadAllAsync(_cts.Token))
            await i(_cts.Token);

    }
    private void OnWifiDeviceStateChanged(object? sender, DeviceStateEventArgs e)
    {
        WifiDeviceInfo w = (WifiDeviceInfo)sender;
        _channel.Writer.WriteAsync(this.AppendStations);
        _channel.Writer.WriteAsync(async (token) => await this.AppendConnectivity((DeviceState)e.NewState,w.Id, token));
        _channel.Writer.WriteAsync(this.AppendWifiProfiles);
    }

    public async Task AppendConnectivity(DeviceState? st = null,
        PathId? id = null, CancellationToken token = default)
    {
        await WirelessConnectivityService.Append(_client, plumber, env, st, id, token);
        log.LogInformation($"Appending wifi connectivity. ({st})");
    }
    private async Task AppendWifiProfiles(CancellationToken stoppingToken)
    {
        if(await WirelessProfilesService.AppendIfRequired(_client, plumber, env, stoppingToken))
            log.LogInformation("Appending wifi profiles.");
    }

    private async Task AppendStations(CancellationToken stoppingToken)
    {
        if(await WirelessStationService.AppendIfRequired(_client, plumber, env, stoppingToken))
            log.LogInformation("Appending wifi stations.");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if(_client != null)
            await _client.DisposeAsync();
        await _d.DisposeAsync();
    }

        
}