namespace EventPi.Services.NetworkMonitor.Contract;

public enum ActiveConnectionState : uint
{
    Unknown = 0,
    Activating = 1,
    Activated = 2,
    Deactivating = 3,
    Deactivated = 4
}