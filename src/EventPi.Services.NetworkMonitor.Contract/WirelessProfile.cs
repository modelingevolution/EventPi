using EventPi.Abstractions;

namespace EventPi.Services.NetworkMonitor.Contract;

public record WirelessProfile
{
    public Guid Id => FileName.ToGuid();
    public string ProfileName { get; init; }
    public string Ssid { get; init; }
    public string InterfaceName { get; init; }
    public string FileName { get; init; }
    public bool IsConnected { get; init; }
        
}