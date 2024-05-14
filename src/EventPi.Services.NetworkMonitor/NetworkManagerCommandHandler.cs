using EventPi.Abstractions;
using EventPi.NetworkMonitor;
using EventPi.Services.NetworkMonitor.Contract;
using MicroPlumberd;
using MicroPlumberd.Services;
using Microsoft.Extensions.Logging;
using System.Numerics;
using System.Threading.Channels;
using Tmds.DBus.Protocol;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ActivationFailedReason = EventPi.NetworkMonitor.ActivationFailedReason;
using DeviceState = EventPi.Services.NetworkMonitor.Contract.DeviceState;

namespace EventPi.Services.NetworkMonitor;

[CommandHandler]
public partial class NetworkManagerCommandHandler : IAsyncDisposable
{
    private NetworkManagerClient? _client;
    private readonly IPlumber _plumber;
    private readonly IEnvironment _env;
    private readonly ILogger<NetworkManagerCommandHandler> _log;
    private readonly Channel<Func<CancellationToken, Task>> _channel;
    private readonly CancellationTokenSource _cts;
    public NetworkManagerCommandHandler(IPlumber plumber, IEnvironment env, ILogger<NetworkManagerCommandHandler> log)
    {
        _plumber = plumber;
        _env = env;
        _log = log;
        _channel = Channel.CreateBounded<Func<CancellationToken, Task>>(new BoundedChannelOptions(5) { FullMode = BoundedChannelFullMode.DropOldest });
        _cts = new CancellationTokenSource();
        _ = Task.Factory.StartNew(OnStateAppender, TaskCreationOptions.LongRunning);
    }
    private async Task OnStateAppender()
    {
        await foreach (var i in _channel.Reader.ReadAllAsync(_cts.Token))
            await i(_cts.Token);

    }

    [ThrowsFaultException<WrongHostError>]
    [ThrowsFaultException<ConnectionError>]
    public async Task Handle(HostName hostName, DefineWirelessProfile data)
    {
        await Prepare(hostName);

        var ap = await _client.GetAccessPoints()
            .WhereAwait(async x => x.Ssid == data.Ssid)
            .OrderByDescending(x=>x.SignalStrength)
            .FirstOrDefaultAsync();

        if (ap != null)
        {
            var n = DateTime.Now;
            await ap.Setup(data.Password,
                $"{data.Ssid}-connection-{n.Year:D4}{n.Month:D2}{n.Day:D2}.{n.Hour:D2}{n.Minute:D2}{n.Second:D2}");
        }
        else
            throw new FaultException<ConnectionError>(new ConnectionError()
            {
                Message = "Access point was not found. Try again later.",
                Reason = ConnectionErrorReason.AccessPointNotFound
            });
        await WirelessProfilesService.AppendIfRequired(_client, _plumber, _env);
    }

    [ThrowsFaultException<WrongHostError>]
    [ThrowsFaultException<ConnectionError>]
    public async Task Handle(HostName hostName, ConnectAccessPoint cmd)
    {
        await Prepare(hostName);

        _log.LogInformation($"Connect access point: {cmd.Ssid}");
        if (await _client!.GetProfiles().SelectAwait(async x => await x.Settings()).OfType<WifiProfileSettings>()
                .AnyAsync(x => x.Ssid == cmd.Ssid))
        {
            var dev = await _client.GetDevices().OfType<WifiDeviceInfo>().FirstOrDefaultAsync();
            try
            {
                await dev.ConnectAccessPoint(cmd.Ssid);
            }
            catch (ActivationFailedException ex)
            {
                _log.LogInformation($"Activation log exception, reason: {(uint)ex.Reason} - {ex.Reason.ToString()}");
                // this should not happen
                throw new FaultException<ConnectionError>(new ConnectionError()
                {
                    Message = ex.Message,
                    Reason = MapReason(ex.Reason),
                    ProfileFileName = ex.ProfileFileName
                });
            }
            catch (Exception ex)
            {
                _log.LogInformation($"Activation exception, message: {ex.Message}");
                throw new FaultException<ConnectionError>(new ConnectionError()
                {
                    Message = ex.Message,
                    Reason = ConnectionErrorReason.Unknown
                });
            }
        }
        else
        {
            throw new FaultException<ConnectionError>(new ConnectionError()
            {
                Message = $"No profile defined for {cmd.Ssid} ssid.",
                Reason = ConnectionErrorReason.MissingProfile
            });
        }

        await WirelessProfilesService.AppendIfRequired(_client, _plumber, _env);
        await WirelessConnectivityService.Append(_client, _plumber, _env);
    }

    private ConnectionErrorReason MapReason(ActivationFailedReason exReason)
    {
        switch (exReason)
        {
            case ActivationFailedReason.IpConfigInvalid:
                return ConnectionErrorReason.IpConfigInvalid;
            case ActivationFailedReason.ConnectTimeout:
            case ActivationFailedReason.ServiceStartTimeout:
                return ConnectionErrorReason.ConnectTimeout;
            case ActivationFailedReason.NoSecrets:
                return ConnectionErrorReason.NoSecrets;
            case ActivationFailedReason.LoginFailed:
                return ConnectionErrorReason.LoginFailed;
            case ActivationFailedReason.DeviceDisconnected:
                return ConnectionErrorReason.DeviceDisconnected;
            default:
                return ConnectionErrorReason.Unknown;
        }
    }

    [ThrowsFaultException<WrongHostError>]
    public async Task Handle(HostName hostName, RequestWifiScan cmd)
    {
        await Prepare(hostName);

        _log.LogInformation($"Request wifi scan");

        _=Task.Run(async () =>
        {
            await using var c = await NetworkManagerClient.Create();
            await c.RequestWifiScan();
            await WirelessStationService.AppendIfRequired(c, _plumber, _env);
            await WirelessProfilesService.AppendIfRequired(c, _plumber, _env);
            await WirelessConnectivityService.Append(c, _plumber, _env);
        });
        _log.LogInformation($"Request send.");
    }
    [ThrowsFaultException<WrongHostError>]
    [ThrowsFaultException<ProfileNotFound>]
    public async Task Handle(HostName hostName, DeactivateWirelessProfile profile)
    {
        await Prepare(hostName);

        _log.LogInformation($"Deactivate Wireless Profile");

        var p = await GetProfileById(profile.ProfileId);

        await p.Deactivate();
    }
    [ThrowsFaultException<WrongHostError>]
    [ThrowsFaultException<ProfileNotFound>]
    public async Task Handle(HostName hostName, DisconnectWirelessNetwork profile)
    {
        await Prepare(hostName);

        _log.LogInformation($"Disconnect Wireless Network");

        var p = await GetProfileById(profile.ProfileId);

        await foreach (var i in _client.GetDevices().OfType<WifiDeviceInfo>())
        {
            var con = await i.GetConnectionProfileId();
            if (con != p.Id) continue;
            await i.DisconnectAsync();
            
            await WirelessProfilesService.AppendIfRequired(_client, _plumber, _env);
            await WirelessConnectivityService.Append(_client, _plumber, _env);
            break;
        }
    }
    [ThrowsFaultException<WrongHostError>]
    [ThrowsFaultException<ProfileNotFound>]
    public async Task Handle(HostName hostName, ActivateWirelessProfile profile)
    {
        await Prepare(hostName);

        _log.LogInformation($"Activate Wireless Profile");

        var p = await GetProfileById(profile.ProfileId);
            
        await p.Activate();
        await WirelessProfilesService.AppendIfRequired(_client, _plumber, _env);
        await WirelessConnectivityService.Append(_client, _plumber, _env);
    }
    [ThrowsFaultException<WrongHostError>]
    [ThrowsFaultException<ProfileNotFound>]
    public async Task Handle(HostName hostName, DeleteWirelessProfile profile)
    {
        await Prepare(hostName);

        _log.LogInformation($"Delete Wireless Profile");

        var p = await GetProfileById(profile.ProfileId);

        await p.Delete();
            
        await WirelessProfilesService.AppendIfRequired(_client, _plumber, _env);
    }

   
   
    private async Task<ProfileInfo?> GetProfileById(Guid profileId)
    {
        var p = await _client!.GetProfiles().FirstOrDefaultAsync(x => x.FileId == profileId);
        if (p == null) throw new FaultException<ProfileNotFound>(profileId);
        return p;
    }

    private async Task Prepare(HostName hostName)
    {
        if (hostName != _env.HostName) throw new FaultException<WrongHostError>(WrongHostError.Error);
        _client ??= await NetworkManagerClient.Create();
    }

    public async ValueTask DisposeAsync()
    {
        if (_client != null) await _client.DisposeAsync();
    }
}