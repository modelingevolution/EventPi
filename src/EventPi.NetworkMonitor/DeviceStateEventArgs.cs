namespace EventPi.NetworkMonitor;

public class DeviceStateEventArgs : EventArgs
{
    public DeviceStateChanged OldState { get; init; } 
    public DeviceStateChanged NewState { get; init; }
}