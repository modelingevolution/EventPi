using EventPi.Abstractions;
using MicroPlumberd;
using MicroPlumberd.Services;

namespace EventPi.Services.NetworkMonitor.Contract;

[ThrowsFaultException<WrongHostError>]
[ThrowsFaultException<ProfileNotFound>]
public record DeactivateWirelessProfile
{
    [GuidNotEmpty]
    public Guid ProfileId { get; init; }
}