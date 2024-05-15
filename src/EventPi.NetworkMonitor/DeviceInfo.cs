using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices.Marshalling;
using EventPi.Abstractions;
using NetworkManager.DBus;
using Tmds.DBus.Protocol;
using Connection = NetworkManager.DBus.Connection;

namespace EventPi.NetworkMonitor;

public class AccessPointNotFoundException : Exception
{
}
public class ProfileNotFoundException : Exception{}
public class ActivationFailedException : Exception
{
    public string ProfileFileName { get; init; }
    public ActivationFailedReason Reason { get; init; }
}
public record WifiDeviceInfo : DeviceInfo
{
    internal Wireless Wifi => Client.Service.CreateWireless(Id.Path);

    public async Task<AccessPointInfo?> AccessPoint()
    {
        var accessPointId= await this.Wifi.GetActiveAccessPointAsync();
        if (accessPointId == "/") return null;
        return await NetworkManagerClient.OnGetAccessPoint(this.Client, Client.Service.CreateAccessPoint(accessPointId),
            this);
    }

    
    public async Task ConnectAccessPoint(string ssid, bool wait = true)
    {
        var c = await NetworkManagerClient.Create();

        var ap = await c.GetAccessPoints().Where(x => x.Ssid == ssid).FirstOrDefaultAsync() ?? throw new AccessPointNotFoundException();
        
        var aPath = await c.NetworkManager.ActivateConnectionAsync("/", this.Device.Path, ap.AccessPointPath);
        
        if (wait)
        {
            var activeConnection = c.Service.CreateActive(aPath);
            var conId = await activeConnection.GetConnectionAsync();
            var con = c.Service.CreateConnection(conId);
            string fileName = await con.GetFilenameAsync();
            CancellationTokenSource cts = new CancellationTokenSource();
            ActivationFailedReason ret = ActivationFailedReason.Unknown;
            ActivationState state = ActivationState.Unknown;
            using var d = await activeConnection.WatchStateChangedAsync((Exception? e, (uint state, uint reason) ch) =>
            {
                Console.WriteLine($"{(ActivationState)ch.state}:... {(ActivationFailedReason)ch.reason}");
                if (e != null)
                {
                    cts.Cancel();
                    return;
                }

                if (ch.state == (uint)ActivationState.Activated || ch.state == (uint)ActivationState.Deactivated)
                {
                    ret = (ActivationFailedReason)ch.reason;
                    state = (ActivationState)ch.state;
                    cts.Cancel();
                }
            }, false);
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(110), cts.Token);

            }
            catch (OperationCanceledException)
            {
                // This is expected.
                if (state == ActivationState.Activated)
                    return;
                if (state == ActivationState.Deactivated)
                    throw new ActivationFailedException()
                    {
                        Reason = ret,
                        ProfileFileName = fileName
                    };
            }

            throw new TimeoutException();
        }
    }
    public event EventHandler<AccessPointDiscoveryArgs> AccessPointVisilibityChanged;
    public event EventHandler<AccessPointPropertyChangedArgs> AccessPointSignalChanged;
    private Disposables d = new Disposables();
    private readonly ConcurrentDictionary<string, Disposables> _accessPoints = new();
    public async Task<IAsyncDisposable> SubscribeAccessPoint(bool monitorSignal = false)
    {
        var addClient = await Client.Clone();
        var addDevice = addClient.Service.CreateWireless(Id.Path);
        d += await addDevice.WatchAccessPointAddedAsync(
            (Exception? ex, ObjectPath path) =>
            {
                if (ex == null) return;
                AccessPointVisilibityChanged?.Invoke(this, new AccessPointDiscoveryArgs(){Operation = Operation.Found, Path = path});
            });
        d += addClient;

        var removeClient = await Client.Clone();
        var removeDevice = removeClient.Service.CreateWireless(Id.Path);
        
        d += await removeDevice.WatchAccessPointRemovedAsync(
            (Exception? ex, ObjectPath path) =>
            {
                if (ex == null) return;
                AccessPointVisilibityChanged?.Invoke(this, new AccessPointDiscoveryArgs() { Operation = Operation.Lost, Path = path });

            });
        d += removeClient;

        if (monitorSignal)
            this.AccessPointVisilibityChanged += OnMonitorSignal;

        return d;
    }

    private void OnMonitorSignal(object? sender, AccessPointDiscoveryArgs e)
    {
        if (e.Operation == Operation.Found)
        {
            Task.Run(async () =>
            {
                Disposables u = new Disposables();
                _accessPoints.TryAdd(e.Path, u);
                var client = await Client.Clone();
                var dev= client.Service.CreateAccessPoint(e.Path);
                u += client;
                u += await dev.WatchPropertiesChangedAsync((ex, p) =>
                {
                    if(p.HasChanged("Strength"))
                    AccessPointSignalChanged?.Invoke(this, new AccessPointPropertyChangedArgs()
                    {
                        Path = e.Path,
                        Strength = p.Properties.Strength,
                    });
                });
                

            });
        }
        else
        {
            if (_accessPoints.TryGetValue(e.Path, out var d)) 
                Task.Run( () =>  d.DisposeAsync());
        }
    }
}

public enum Operation { Found, Lost }

public class AccessPointPropertyChangedArgs : EventArgs
{
    public string Path { get; init; }
    public byte Strength { get; init; }
    public string Ssid { get; init; }
}
public class AccessPointDiscoveryArgs : EventArgs
{
    public string Path { get; init; }
    public Operation Operation { get; init; }
}


public class Disposables : IAsyncDisposable
{
    private readonly List<object> _actions = new();
    public static Disposables operator +(Disposables left, IDisposable arg)
    {
        left._actions.Add(arg);
        return left;
    }
   
    public static Disposables operator +(Disposables left, IAsyncDisposable arg)
    {
        left._actions.Add(arg);
        return left;
    }
    public static Disposables operator -(Disposables left, IDisposable arg)
    {
        left._actions.Remove(arg);
        return left;
    }

    public static Disposables operator -(Disposables left, IAsyncDisposable arg)
    {
        left._actions.Remove(arg);
        return left;
    }
    public async ValueTask DisposeAsync()
    {
        foreach(var i in _actions)
            switch (i)
            {
                case IDisposable d: d.Dispose();
                    break;
                case IAsyncDisposable d: await d.DisposeAsync();
                    break;
            }
    }
}

public record ConnectionInfo(Ip4Config Ip4Config);

public record DeviceInfo
{
    internal NetworkManagerClient Client { get; init; }
    public PathId Id { get; init; }
    public DeviceState State { get; set; }
    public string InterfaceName { get; init; }
    public DeviceType DeviceType { get; init; }

    public async Task<ConnectionInfo?> GetConnectionInfo()
    {
        var activeId = await Device.GetActiveConnectionAsync();
        if (activeId == "/") return null;

        var active = Device.Service.CreateActive(activeId);
        var ipV4ConfigId = await active.GetIp4ConfigAsync();
        var ipV4Config = Device.Service.CreateIP4Config(ipV4ConfigId);
        var ips = await ipV4Config.GetAddressDataAsync();
        Ip4 ip = Ip4.Loopback;
        uint prefix = 32;
        foreach (var i in ips)
        {
            ip  = i["address"].GetString();
            prefix = i["prefix"].GetUInt32();
        }
        
        
        Ip4 gw = await ipV4Config.GetGatewayAsync();
        return new ConnectionInfo(Ip4Config: new Ip4Config(ip, prefix, gw));

    }
    public async Task<ProfileInfo?> GetConnectionProfile()
    {
        var p = await GetConnectionProfileId();
        if (p.HasValue && p != "/")
        {
            var info = Client.Service.CreateConnection(p.Value.ToString());
            return new ProfileInfo()
            {
                Client = Client,
                FileName = await info.GetFilenameAsync(),
                Id = p.Value
            };
        }

        return null;
    }
    public async Task<PathId?> GetConnectionProfileId()
    {
        var active = await Device.GetActiveConnectionAsync();
        if (active == "/") return null;
        var a = Client.Service.CreateActive(active);
        return await a.GetConnectionAsync();
    }
    public event EventHandler<DeviceStateEventArgs> StateChanged; 
        
    public async Task<IAsyncDisposable> SubscribeStateChanged()
    {
        var client = await Client.Clone();
        var device = client.Service.CreateDevice(Id.Path);
        Disposables d = new Disposables();
        d += await device.WatchStateChangedAsync(
            (Exception? ex, (uint NewState, uint OldState, uint Reason) change) =>
            {
                if (ex is null)
                {
                    StateChanged?.Invoke(this, new DeviceStateEventArgs()
                    {
                        NewState = (DeviceState)change.NewState,
                        OldState = (DeviceState)change.OldState
                    });
                } 
                else Console.Error.WriteLine(ex.Message);
                
            });
        d += client;
        return d;
    }
    
    internal Device Device => Client.Service.CreateDevice(Id.Path);
    public async Task ActivateProfile(ProfileInfo profileInfo)
    {
        await Client.NetworkManager.ActivateConnectionAsync(profileInfo.Id, Id, "/");
    }

    public async Task DisconnectAsync()
    {
        await Device.DisconnectAsync();
    }
}