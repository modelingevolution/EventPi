using EventPi.Abstractions;
using EventPi.NetworkMonitor;
using EventPi.Services.NetworkMonitor.Contract;
using MicroPlumberd;

namespace EventPi.Services.NetworkMonitor;

/// <summary>
/// No need for a domain-service. We just need common procedures.
/// </summary>
static class WirelessProfilesService
{
    public static async Task<bool> AppendIfRequired(NetworkManagerClient client, IPlumber plumber, IEnvironment env, CancellationToken stoppingToken = default)
    {
        WirelessProfilesState state = new WirelessProfilesState();
        
        var activeConnection = await client.GetDevices().OfType<WifiDeviceInfo>()
            .SelectAwait(async x => await x.GetConnectionProfileId())
            .Where(x=> x != string.Empty && x != "/")
            .ToHashSetAsync();

        bool hasChanged = false;
        var actualProfiles = await client.GetProfiles().ToArrayAsync(cancellationToken: stoppingToken);
        WirelessProfilesState currentState = await plumber.GetState<WirelessProfilesState>(env.HostName);
        foreach (var i in actualProfiles)
        {
            if (await i.Settings() is WifiProfileSettings wifi)
            {
                var isActive = activeConnection.Contains(i.Id);
                var profiles = currentState != null ? currentState.AsEnumerable() : Array.Empty<WirelessProfile>();
                if (currentState == null || !profiles.Any(x => x.FileName == i.FileName && x.IsConnected == isActive))
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
        if(!hasChanged)
            if (currentState != null && currentState.Count != actualProfiles.Length)
                hasChanged = true;
        //WirelessProfilesState currentState = await plumber.GetState<WirelessProfilesState>(env.HostName);
        if (hasChanged)
            await plumber.AppendState(state, env.HostName, token: stoppingToken);
        return hasChanged;
    }
}