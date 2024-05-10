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

    [OutputStream("WirelessProfiles")]
    public class WirelessProfilesState : Collection<WirelessProfile>, IStatefulStream<HostName>
    {
        public static string FullStreamName(HostName id) => $"WirelessProfiles-{id}";
    }

    public record DeleteWirelessProfile
    {
        public string ProfileName { get; init; }
    }
    public record Disconnect
    {
        public string ProfileName { get; init; }
    }
    public record Connect
    {
        public string ProfileName { get; init; }
    }
    public record WirelessProfile
    {
        public string ProfileName { get; init; }
        public string Ssid { get; init; }
        public string InterfaceName { get; init; }
    }
    public record DefineWirelessProfile 
    {
        public string InterfaceName { get; init; }
        public string Ssid { get; init; }
        
        // Should be SecretString
        public string Pwd { get; init; }
    }
    [OutputStream("WirelessConnectivity")]
    public record WirelessConnectivityState : IStatefulStream<HostName>
    {
        public static string FullStreamName(HostName id) => $"WirelessConnectivity-{id}";
        public string Ssid { get; init; }
        public string InterfaceName { get; init; }
        public string? ConnectionName { get; init; }
        public DeviceStateChanged State { get; init; }
    }

    public record WirelessStation
    {
        public string Ssid { get; init; }
        public string InterfaceName { get; init; }
        public byte Signal { get; init; }
    }
    public enum DeviceStateChanged
    {
        /// <summary>
        /// the device's state is unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// the device is recognized, but not managed by NetworkManager
        /// </summary>
        Unmanaged = 10,
        /// <summary>
        /// the device is managed by NetworkManager, but is not available for use. Reasons may include the wireless switched off, missing firmware, no ethernet carrier, missing supplicant or modem manager, etc.
        /// </summary>
        Unavailable = 20,
        /// <summary>
        /// the device can be activated, but is currently idle and not connected to a network.
        /// </summary>
        Disconnected = 30,

        /// <summary>
        /// the device is preparing the connection to the network. This may include operations like changing the MAC address, setting physical link properties, and anything else required to connect to the requested network.
        /// </summary>
        Prepare = 40,
        /// <summary>
        /// the device is connecting to the requested network. This may include operations like associating with the WiFi AP, dialing the modem, connecting to the remote Bluetooth device, etc.
        /// </summary>
        Config = 50,

        /// <summary>
        /// the device requires more information to continue connecting to the requested network. This includes secrets like WiFi passphrases, login passwords, PIN codes, etc.
        /// </summary>
        NeedAuth = 60,
        /// <summary>
        /// the device is requesting IPv4 and/or IPv6 addresses and routing information from the network.
        /// </summary>
        IpConfig = 70,
        /// <summary>
        /// the device is checking whether further action is required for the requested network connection. This may include checking whether only local network access is available, whether a captive portal is blocking access to the Internet, etc.
        /// </summary>
        IpCheck = 80,

        /// <summary>
        /// the device is waiting for a secondary connection (like a VPN) which must activated before the device can be activated
        /// </summary>
        Secondaries = 90,

        /// <summary>
        /// the device has a network connection, either local or global.
        /// </summary>
        Activated = 100,

        /// <summary>
        /// a disconnection from the current network connection was requested, and the device is cleaning up resources used for that connection. The network connection may still be valid.
        /// </summary>
        Deactivating = 110,

        /// <summary>
        /// the device failed to connect to the requested network and is cleaning up the connection request
        /// </summary>
        Failed = 120,
    }
}
