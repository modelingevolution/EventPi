using EventPi.Abstractions;
using EventPi.NetworkMonitor;
using EventPi.Services.NetworkMonitor.Contract;
using MicroPlumberd;
using MicroPlumberd.Services;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EventPi.Services.NetworkMonitor;

[CommandHandler]
public partial class NetworkManagerCommandHandler(IPlumber plumber, IEnvironment env, ILogger<NetworkManagerCommandHandler> log) : IAsyncDisposable
{
    private NetworkManagerClient? _client;

    [ThrowsFaultException<WrongHostError>]
    public async Task Handle(HostName hostName, DefineWirelessProfile data)
    {
        await Prepare(hostName);

        await WirelessProfilesService.AppendIfRequired(_client, plumber, env);
    }
    [ThrowsFaultException<WrongHostError>]
    public async Task Handle(HostName hostName, ConnectAccessPoint cmd)
    {
        await Prepare(hostName);

        log.LogInformation($"Connect access point: {cmd.Ssid}");

        var dev = await _client.GetDevices().OfType<WifiDeviceInfo>().FirstOrDefaultAsync();
        await dev.ConnectAccessPoint(cmd.Ssid);
        
        await WirelessProfilesService.AppendIfRequired(_client, plumber, env);
        await WirelessConnectivityService.Append(_client, plumber, env);
    }
    [ThrowsFaultException<WrongHostError>]
    public async Task Handle(HostName hostName, RequestWifiScan cmd)
    {
        await Prepare(hostName);

        log.LogInformation($"Request wifi scan");

        _=Task.Run(async () =>
        {
            await using var c = await NetworkManagerClient.Create();
            await c.RequestWifiScan();
            await WirelessStationService.AppendIfRequired(c, plumber, env);
            await WirelessProfilesService.AppendIfRequired(c, plumber, env);
            await WirelessConnectivityService.Append(c, plumber, env);
        });
        log.LogInformation($"Request send.");
    }
    [ThrowsFaultException<WrongHostError>]
    [ThrowsFaultException<ProfileNotFound>]
    public async Task Handle(HostName hostName, DeactivateWirelessProfile profile)
    {
        await Prepare(hostName);

        log.LogInformation($"Deactivate Wireless Profile");

        var p = await GetProfileById(profile.ProfileId);

        await p.Deactivate();
    }
    [ThrowsFaultException<WrongHostError>]
    [ThrowsFaultException<ProfileNotFound>]
    public async Task Handle(HostName hostName, DisconnectWirelessNetwork profile)
    {
        await Prepare(hostName);

        log.LogInformation($"Disconnect Wireless Network");

        var p = await GetProfileById(profile.ProfileId);

        await foreach (var i in _client.GetDevices().OfType<WifiDeviceInfo>())
        {
            var con = await i.GetConnectionProfileId();
            if (con != p.Id) continue;
            await i.DisconnectAsync();
            
            await WirelessProfilesService.AppendIfRequired(_client, plumber, env);
            await WirelessConnectivityService.Append(_client, plumber, env);
            break;
        }
    }
    [ThrowsFaultException<WrongHostError>]
    [ThrowsFaultException<ProfileNotFound>]
    public async Task Handle(HostName hostName, ActivateWirelessProfile profile)
    {
        await Prepare(hostName);

        log.LogInformation($"Activate Wireless Profile");

        var p = await GetProfileById(profile.ProfileId);
            
        await p.Activate();
        await WirelessProfilesService.AppendIfRequired(_client, plumber, env);
        await WirelessConnectivityService.Append(_client, plumber, env);
    }
    [ThrowsFaultException<WrongHostError>]
    [ThrowsFaultException<ProfileNotFound>]
    public async Task Handle(HostName hostName, DeleteWirelessProfile profile)
    {
        await Prepare(hostName);

        log.LogInformation($"Delete Wireless Profile");

        var p = await GetProfileById(profile.ProfileId);

        await p.Delete();
            
        await WirelessProfilesService.AppendIfRequired(_client, plumber, env);
    }

   
   
    private async Task<ProfileInfo?> GetProfileById(Guid profileId)
    {
        var p = await _client!.GetProfiles().FirstOrDefaultAsync(x => x.FileId == profileId);
        if (p == null) throw new FaultException<ProfileNotFound>(profileId);
        return p;
    }

    private async Task Prepare(HostName hostName)
    {
        if (hostName != env.HostName) throw new FaultException<WrongHostError>(WrongHostError.Error);
        _client ??= await NetworkManagerClient.Create();
    }

    public async ValueTask DisposeAsync()
    {
        if (_client != null) await _client.DisposeAsync();
    }
}