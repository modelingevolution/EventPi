using EventPi.Abstractions;
using MicroPlumberd.Services;

namespace EventPi.Services.NetworkMonitor.Contract;

[ThrowsFaultException<WrongHostError>]
[ThrowsFaultException<ProfileNotFound>]
public record DeleteWirelessProfile
{
    [GuidNotEmpty]
    public Guid ProfileId { get; init; }
}