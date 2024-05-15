using System.Collections.ObjectModel;
using EventPi.Abstractions;
using MicroPlumberd;

namespace EventPi.Services.NetworkMonitor.Contract;

[OutputStream("WirelessProfiles")]
public class WirelessProfilesState : Collection<WirelessProfile>, IStatefulStream<HostName>
{
    public static string FullStreamName(HostName id) => $"WirelessProfiles-{id}";
}