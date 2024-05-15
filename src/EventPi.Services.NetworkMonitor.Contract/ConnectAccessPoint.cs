using System.Diagnostics.CodeAnalysis;
using MicroPlumberd.Services;

namespace EventPi.Services.NetworkMonitor.Contract;

[ThrowsFaultException<WrongHostError>]
[ThrowsFaultException<ConnectionError>]
public record ConnectAccessPoint
{
    [NotNull]
    public string Ssid { get; set; }
}