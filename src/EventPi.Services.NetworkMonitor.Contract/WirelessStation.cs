using System.ComponentModel;

namespace EventPi.Services.NetworkMonitor.Contract;

public record WirelessStation
{
    public string Ssid { get; init; }
    [Description("Interface name")]
    public string InterfaceName { get; init; }
    public byte Signal { get; init; }

       
}