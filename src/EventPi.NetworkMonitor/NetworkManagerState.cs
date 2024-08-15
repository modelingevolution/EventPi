namespace EventPi.NetworkMonitor;

public enum NetworkManagerState : uint
{
    Unknown = 0,
    ASleep = 10,
    Disconnected = 20,
    Disconnecting = 30,
    Connecting = 40,
    ConnectedLocal = 50,
    ConnectedSite = 60,
    ConnectedGlobal = 70
}