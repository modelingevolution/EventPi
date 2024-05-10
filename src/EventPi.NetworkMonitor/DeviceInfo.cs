namespace EventPi.NetworkMonitor;

public record DeviceInfo
{
    internal NetworkManagerClient Client { get; init; }
    internal string Path { get; init; }
    public string InterfaceName { get; init; }
    public DeviceType DeviceType { get; init; }
    public event EventHandler<DeviceStateEventArgs> StateChanged; 
        
    public async Task<IAsyncDisposable> SubscribeStateChanged()
    {
        var client = await Client.Clone();
        var device = client.Service.CreateDevice(Path);
        await device.WatchStateChangedAsync(
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
        return client;
    }
}