using EventPi.Abstractions;
using EventPi.NetworkMonitor;
using EventPi.Services.NetworkMonitor.Contract;
using MicroPlumberd;

namespace EventPi.Services.NetworkMonitor;

static class WirelessStationService
{
    public static async Task<bool> AppendIfRequired(NetworkManagerClient client, IPlumber plumber, IEnvironment env, CancellationToken stoppingToken = default)
    {
        WirelessStationsState state = new WirelessStationsState();
        bool changed = false;
        WirelessStationsState currentState = await plumber.GetState<WirelessStationsState>(env.HostName);
        var currentStations = currentState != null ? currentState.ToHashSet() : new HashSet<WirelessStation>();

        await foreach (var i in client.GetAccessPoints().WithCancellation(stoppingToken))
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