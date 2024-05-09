using NetworkManager.DBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetworkManager.DBus;
using Tmds.DBus.Protocol;
using Connection = Tmds.DBus.Protocol.Connection;
namespace EventPi.NetworkMonitor
{
    public class NetworkManagerClient : IAsyncDisposable
    {
        internal NetworkManagerService Service;
        internal NetworkManager.DBus.NetworkManager NetworkManager;
        internal Connection Connection;
        internal Settings Settings;
        public static async Task<NetworkManagerClient> Create()
        {
            NetworkManagerClient client = new NetworkManagerClient();
            await client.Initialize();
            return client;
        }
        private async Task Initialize()
        {
            this.Connection = new Connection(Address.System!);
            await Connection.ConnectAsync();
            this.Service = new NetworkManagerService(Connection, "org.freedesktop.NetworkManager");
            this.NetworkManager = Service.CreateNetworkManager("/org/freedesktop/NetworkManager");
            this.Settings = Service.CreateSettings("/org/freedesktop/NetworkManager/Settings");
        }

        public async IAsyncEnumerable<DeviceInfo> GetDevices()
        {

            foreach (var devicePath in await NetworkManager.GetDevicesAsync())
            {
                var device = Service.CreateDevice(devicePath);
                var interfaceName = await device.GetInterfaceAsync();
                var type = (DeviceType)await device.GetDeviceTypeAsync();

                yield return new DeviceInfo()
                {
                    Path = devicePath,
                    DeviceType = type,
                    InterfaceName = interfaceName,
                    Client = this
                };
            }
        }

        
        
        public async IAsyncEnumerable<WifiNetwork> GetWifiNetworks()
        {
            await foreach (var d in GetDevices())
            {
                if (d.DeviceType != DeviceType.Wifi) continue;

                await foreach (var i in GetWifiNetworks(d)) yield return i;
                yield break;
            }
        }
        public async IAsyncEnumerable<WifiNetwork> GetWifiNetwork(string interfaceName)
        {
            await foreach (var d in GetDevices())
            {
                if (d.DeviceType != DeviceType.Wifi || d.InterfaceName != interfaceName) continue;

                await foreach (var i in GetWifiNetworks(d)) yield return i;
                yield break;
            }
        }
        private async IAsyncEnumerable<WifiNetwork> GetWifiNetworks(DeviceInfo d)
        {
            var wifi = Service.CreateWireless(d.Path);
            var list = await wifi.GetAccessPointsAsync();

            foreach (var accessPoint in list.Select(x => Service.CreateAccessPoint(x)))
            {
                var wssid = await accessPoint.GetSsidAsync();
                var strength = await accessPoint.GetStrengthAsync();
                var s = Encoding.UTF8.GetString(wssid);
                //var wpfFlags = await accessPoint.GetWpaFlagsAsync();
                var mode = (WifiAccessPointMode)await accessPoint.GetModeAsync();
                var maxKbps = await accessPoint.GetMaxBitrateAsync();

                yield return new WifiNetwork
                {
                    AccessPointMode = mode,
                    AccessPointPath = accessPoint.Path,
                    DevicePath = d.Path,
                    MaxKbitRate = maxKbps,
                    SignalStrength = strength,
                    Ssid = s,
                    SourceInterface = d.InterfaceName,
                    SourceDevice = d,
                    Client = this
                };
            }
        }
        public async ValueTask DisposeAsync()
        {
            if (Connection is IAsyncDisposable connectionAsyncDisposable)
                await connectionAsyncDisposable.DisposeAsync();
            else
                Connection.Dispose();
        }
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
    public enum WifiAccessPointMode
    {
        //the device or access point mode is unknown
        Unkown = 0,
        //for both devices and access point objects, indicates the object is part of an Ad-Hoc 802.11 network without a central coordinating access point.    
        AdHoc = 1,
        //the device or access point is in infrastructure mode. For devices, this indicates the device is an 802.11 client/station. For access point objects, this indicates the object is an access point that provides connectivity to clients.
        Infrastructure = 2,
        //the device is an access point/hotspot. Not valid for access point objects; used only for hotspot mode on the local machine.
        AP = 3
    }
    public record WifiNetwork
    {
        
        public string SourceInterface { get; init; }
        internal string DevicePath { get; init; }
        internal string AccessPointPath { get; init; }
        public string Ssid { get; init; }
        public byte SignalStrength { get; init; }
        public WifiAccessPointMode AccessPointMode { get; init; }
        public uint MaxKbitRate { get; init; }
        public DeviceInfo SourceDevice { get; set; }
        internal NetworkManagerClient Client { get; init; }
        public async Task Connect(string password)
        {
            var connectionSettings = new Dictionary<string, Dictionary<string, Variant>>
            {
                {
                    "802-11-wireless", new Dictionary<string, Variant>
                    {
                        { "ssid", new Variant(Ssid) },
                        { "mode", new Variant("infrastructure") },
                        { "security", new Variant("802-11-wireless-security") }
                    }
                },
                {
                    "802-11-wireless-security", new Dictionary<string, Variant>
                    {
                        { "psk", new Variant(password) },
                        { "auth-alg", new Variant("shared") },
                        { "key-mgmt", new Variant("wpa-psk") },
                        { "psk-flags", new Variant(0u)} // none, system is reposible for storing pwd
                    }
                },
                {
                    "connection", new Dictionary<string, Variant>
                    {
                        { "type", new Variant("802-11-wireless") },
                        { "id", new Variant(Ssid) }
                    }
                },
            };
            await Client.NetworkManager.AddAndActivateConnectionAsync(connectionSettings, DevicePath, AccessPointPath);
        }
    }

    public class DeviceStateEventArgs : EventArgs
    {
        public DeviceStateChanged OldState { get; init; } 
        public DeviceStateChanged NewState { get; init; }
    }
    public record DeviceInfo
    {
        internal NetworkManagerClient Client { get; init; }
        internal string Path { get; init; }
        public string InterfaceName { get; init; }
        public DeviceType DeviceType { get; init; }
        public event EventHandler<DeviceStateEventArgs> StateChanged; 
        
        public async Task SubscribeStateChanged()
        {
            var device = Client.Service.CreateDevice(Path);
            await device.WatchStateChangedAsync(
                (Exception? ex, (uint NewState, uint OldState, uint Reason) change) =>
                {
                    if (ex is null)
                    {
                        StateChanged?.Invoke(this, new DeviceStateEventArgs()
                        {
                            NewState = (DeviceStateChanged)change.NewState,
                            OldState = (DeviceStateChanged)change.OldState
                        });
                    }
                });
        }
    }
}
