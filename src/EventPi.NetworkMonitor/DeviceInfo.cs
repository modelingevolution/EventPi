using NetworkManager.DBus;
using Tmds.DBus.Protocol;

namespace EventPi.NetworkMonitor;

public record WifiDeviceInfo : DeviceInfo
{
    internal Wireless Wifi => Client.Service.CreateWireless(Id.Path);
    public event EventHandler<AccessPointDiscoveryArgs> AccessPointVisilibityChanged;
    public async Task<IAsyncDisposable> SubscribeAccessPoint()
    {
        Disposables d = new Disposables();

        var addClient = await Client.Clone();
        var addDevice = addClient.Service.CreateWireless(Id.Path);
        d += await addDevice.WatchAccessPointAddedAsync(
            (Exception? ex, ObjectPath path) =>
            {
                AccessPointVisilibityChanged?.Invoke(this, new AccessPointDiscoveryArgs(){Operation = Operation.Found, Path = path});

            });
        d += addClient;

        var removeClient = await Client.Clone();
        var removeDevice = removeClient.Service.CreateWireless(Id.Path);
        d += await removeDevice.WatchAccessPointRemovedAsync(
            (Exception? ex, ObjectPath path) =>
            {
                AccessPointVisilibityChanged?.Invoke(this, new AccessPointDiscoveryArgs() { Operation = Operation.Lost, Path = path });

            });
        d += removeClient;

        return d;
    }
}
public enum Operation { Found, Lost }
public class AccessPointDiscoveryArgs : EventArgs
{
    public string Path { get; init; }
    public Operation Operation { get; init; }
}


public class Disposables : IAsyncDisposable
{
    private readonly List<Func<ValueTask>> _actions = new();
    public static Disposables operator +(Disposables left, IDisposable arg)
    {
        left._actions.Add(() =>
        {
            arg.Dispose();
            return ValueTask.CompletedTask;
        });
        return left;
    }
   
    public static Disposables operator +(Disposables left, IAsyncDisposable arg)
    {
        left._actions.Add(arg.DisposeAsync);
        return left;
    }
    public async ValueTask DisposeAsync()
    {
        foreach(var i in _actions)
            await i();
    }
}
public record DeviceInfo
{
    internal NetworkManagerClient Client { get; init; }
    public PathId Id { get; init; }
    public string InterfaceName { get; init; }
    public DeviceType DeviceType { get; init; }

    public async Task<PathId> GetActiveConnection()
    {
        var active = await Device.GetActiveConnectionAsync();
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
                        NewState = (DeviceStateChanged)change.NewState,
                        OldState = (DeviceStateChanged)change.OldState
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