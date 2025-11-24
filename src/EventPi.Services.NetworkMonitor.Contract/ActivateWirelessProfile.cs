using EventPi.Abstractions;
using MicroPlumberd;
using MicroPlumberd.Services;

namespace EventPi.Services.NetworkMonitor.Contract;

[ThrowsFaultException<WrongHostError>]
[ThrowsFaultException<ProfileNotFound>]
[ThrowsFaultException<ConnectionError>]
public record ActivateWirelessProfile
{
    [GuidNotEmpty]
    public Guid ProfileId { get; init; }
}