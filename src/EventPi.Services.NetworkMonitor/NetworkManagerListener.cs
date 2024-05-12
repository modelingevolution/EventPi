using System.Threading.Channels;
using EventPi.Abstractions;
using EventPi.NetworkMonitor;
using EventPi.Services.NetworkMonitor.Contract;
using MicroPlumberd;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;

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
        var wifis = await _client.GetDevices().OfType<WifiDeviceInfo>().ToArrayAsync();
        
        foreach (var i in wifis)
        {
            i.StateChanged += OnWifiDeviceStateChanged;
            i.AccessPointVisilibityChanged += OnWifiAccessPointChanged;
            _d += await i.SubscribeStateChanged();
            _d += await i.SubscribeAccessPoint();
        }

        await Task.Factory.StartNew(OnStateAppender, TaskCreationOptions.LongRunning);
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
        _channel.Writer.WriteAsync(this.AppendStations);
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

static class WirelessStationService
{
    public static async Task<bool> AppendIfRequired(NetworkManagerClient client, IPlumber plumber, IEnvironment env, CancellationToken stoppingToken = default)
    {
        WirelessStationsState state = new WirelessStationsState();
        bool changed = false;
        WirelessStationsState currentState = await plumber.GetState<WirelessStationsState>(env.HostName);
        var currentStations = currentState.ToHashSet();

        await foreach (var i in client.GetWifiNetworks().WithCancellation(stoppingToken))
        {
            var n = new WirelessStation()
            {
                InterfaceName = i.SourceInterface,
                Signal = i.SignalStrength,
                Ssid = i.Ssid
            };

            if (!currentStations.Contains(n))
                changed = true;

            state.Add(n);
        }
        if(changed)
            await plumber.AppendState(state, env.HostName, token: stoppingToken);
        return changed;
    }

}
/// <summary>
/// No need for a domain-service. We just need common procedures.
/// </summary>
static class WirelessProfilesService
{
    public static async Task<bool> AppendIfRequired(NetworkManagerClient client, IPlumber plumber, IEnvironment env, CancellationToken stoppingToken = default)
    {
        WirelessProfilesState state = new WirelessProfilesState();
        WirelessProfilesState currentState = await plumber.GetState<WirelessProfilesState>(env.HostName);
        var activeConnection = await client.GetDevices().OfType<WifiDeviceInfo>()
            .SelectAwait(async x => await x.GetActiveConnection())
            .Where(x=> x != string.Empty && x != "/")
            .ToHashSetAsync();

        bool hasChanged = false;
        await foreach (var i in client.GetProfiles().WithCancellation(stoppingToken))
        {
            if (await i.Settings() is WifiProfileSettings wifi)
            {
                var isActive = activeConnection.Contains(i.Id);
                if (state == null || !currentState.Any(x => x.FileName == i.FileName && x.IsConnected == isActive))
                    hasChanged = true;

                state.Add(new()
                {
                    InterfaceName = wifi.InterfaceName,
                    Ssid = wifi.Ssid,
                    ProfileName = wifi.ProfileName,
                    FileName = i.FileName,
                    IsConnected = isActive
                });
            }
        }

        //WirelessProfilesState currentState = await plumber.GetState<WirelessProfilesState>(env.HostName);
        if (hasChanged)
            await plumber.AppendState(state, env.HostName, token: stoppingToken);
        return hasChanged;
    }
}