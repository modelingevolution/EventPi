using System.ComponentModel.DataAnnotations;
using EventPi.Abstractions;
using MicroPlumberd.Services;

namespace EventPi.Services.NetworkMonitor.Contract;

[ThrowsFaultException<WrongHostError>]
[ThrowsFaultException<ProfileNotFound>]
public record DisconnectWirelessNetwork
{
    [Required(AllowEmptyStrings = false)]
    public string Ssid { get; init; }
    [GuidNotEmpty]
    public Guid ProfileId { get; init; }
}