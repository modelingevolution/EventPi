using System.Collections.ObjectModel;
using EventPi.Abstractions;
using MicroPlumberd;

namespace EventPi.Services.NetworkMonitor.Contract
{
    [OutputStream("WirelessStations")]
    public class WirelessStationsState : Collection<WirelessStation>, IStatefulStream<HostName>
    {
        public static string FullStreamName(HostName id) => $"WirelessStations-{id}";
    }
}
