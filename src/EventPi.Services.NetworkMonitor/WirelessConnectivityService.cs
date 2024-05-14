using EventPi.Abstractions;
using EventPi.NetworkMonitor;
using EventPi.Services.NetworkMonitor.Contract;
using MicroPlumberd;
using DeviceState = EventPi.Services.NetworkMonitor.Contract.DeviceState;

namespace EventPi.Services.NetworkMonitor;

static class WirelessConnectivityService
{
    public static async Task Append(NetworkManagerClient client, IPlumber plumber, IEnvironment env, 
        DeviceState? st = null,
        PathId? id = null,
        CancellationToken stoppingToken = default)
    {
        var wifiDev = id != null ? (WifiDeviceInfo)(await client.GetDevice(id.Value))! : await client.GetDevices().OfType<WifiDeviceInfo>().FirstOrDefaultAsync(cancellationToken: stoppingToken);

        await Append(wifiDev, st, plumber, env, stoppingToken);
    }

    private static async Task Append(WifiDeviceInfo? wifiDev, DeviceState? st, IPlumber plumber, IEnvironment env,
        CancellationToken stoppingToken)
    {
        if (wifiDev != null)
        {
            var cstate = (DeviceState)wifiDev.State;
            if ((st ?? cstate) == DeviceState.Activated)
            {
                
                var ac = await wifiDev.GetConnectionProfile();
                //client.GetProfile(ac);
                var connectionInfo = await wifiDev.GetConnectionInfo();
                var accessPointInfo = await wifiDev.AccessPoint();

                WirelessConnectivityState state = new WirelessConnectivityState()
                {
                    ConnectionName = ac?.FileName,
                    Ssid = accessPointInfo.Ssid,
                    IpConfig = connectionInfo?.Ip4Config,
                    InterfaceName = wifiDev.InterfaceName,
                    State = st ?? (DeviceState)wifiDev.State,
                    Signal = accessPointInfo.SignalStrength
                };
                await plumber.AppendState(state, env.HostName, token: stoppingToken);
            }
            else
            {
                // We are disconnected
                WirelessConnectivityState state = new WirelessConnectivityState()
                {
                    ConnectionName = null,
                    Ssid = null,
                    IpConfig = null,
                    InterfaceName = wifiDev.InterfaceName,
                    State = st ?? (DeviceState)wifiDev.State,
                    Signal = 0
                };
                await plumber.AppendState(state, env.HostName, token: stoppingToken);
            }
        }
    }
}