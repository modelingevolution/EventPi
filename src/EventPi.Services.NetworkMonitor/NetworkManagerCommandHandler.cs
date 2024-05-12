using EventPi.Abstractions;
using EventPi.NetworkMonitor;
using EventPi.Services.NetworkMonitor.Contract;
using MicroPlumberd;
using MicroPlumberd.Services;

namespace EventPi.Services.NetworkMonitor;

[CommandHandler]
public partial class NetworkManagerCommandHandler(IPlumber plumber, IEnvironment env) : IAsyncDisposable
{
    private NetworkManagerClient? _client;

    [ThrowsFaultException<WrongHostError>]
    public async Task Handle(HostName hostName, DefineWirelessProfile data)
    {
        await Prepare(hostName);

        await WirelessProfilesService.AppendIfRequired(_client, plumber, env);
    }
    [ThrowsFaultException<WrongHostError>]
    public async Task Handle(HostName hostName, ConnectWirelessProfile data)
    {
        await Prepare(hostName);

        await WirelessProfilesService.AppendIfRequired(_client, plumber, env);
    }
    [ThrowsFaultException<WrongHostError>]
    public async Task Handle(HostName hostName, RequestWifiScan cmd)
    {
        await Prepare(hostName);
        await _client.RequestWifiScan();
    }
    [ThrowsFaultException<WrongHostError>]
    [ThrowsFaultException<ProfileNotFound>]
    public async Task Handle(HostName hostName, DeactivateWirelessProfile profile)
    {
        await Prepare(hostName);

        var p = await GetProfileById(profile.ProfileId);
    }
    [ThrowsFaultException<WrongHostError>]
    [ThrowsFaultException<ProfileNotFound>]
    public async Task Handle(HostName hostName, DisconnectWirelessNetwork profile)
    {
        await Prepare(hostName);

        var p = await GetProfileById(profile.ProfileId);
    }
    [ThrowsFaultException<WrongHostError>]
    [ThrowsFaultException<ProfileNotFound>]
    public async Task Handle(HostName hostName, ActivateWirelessProfile profile)
    {
        await Prepare(hostName);

        var p = await GetProfileById(profile.ProfileId);
            
        await p.Activate();

        await WirelessProfilesService.AppendIfRequired(_client, plumber, env);
    }
    [ThrowsFaultException<WrongHostError>]
    [ThrowsFaultException<ProfileNotFound>]
    public async Task Handle(HostName hostName, DeleteWirelessProfile profile)
    {
        await Prepare(hostName);

        var p = await GetProfileById(profile.ProfileId);

        await p.Delete();
            
        await WirelessProfilesService.AppendIfRequired(_client, plumber, env);
    }

   
    private async Task AppendStations(CancellationToken cancelationToken = default)
    {
        WirelessStationsState state = new WirelessStationsState();
        await foreach (var i in _client.GetWifiNetworks().WithCancellation(cancelationToken))
        {
            state.Add(new()
            {
                InterfaceName = i.SourceInterface,
                Signal = i.SignalStrength,
                Ssid = i.Ssid
            });
        }

        await plumber.AppendState(state, env.HostName, token: cancelationToken);
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