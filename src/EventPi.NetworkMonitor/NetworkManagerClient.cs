using NetworkManager.DBus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using Connection = Tmds.DBus.Protocol.Connection;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace EventPi.NetworkMonitor
{
    public class NetworkManagerClient : IAsyncDisposable
    {
        private readonly List<NetworkManagerClient> _clones = new List<NetworkManagerClient>();
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
        public async Task<bool> IsConnectionActive(string name)
        {
            var connections = await NetworkManager.GetActiveConnectionsAsync();
            ObjectPath device = "/";
            try
            {
                device = await NetworkManager.GetDeviceByIpIfaceAsync(name);
            }
            catch { return false; }
            foreach (var connectionPath in connections)
            {
                var connectionProxy = Service.CreateActive(connectionPath);
                var devices = await connectionProxy.GetDevicesAsync();
                foreach (var i in devices)
                {
                    if (i.Equals(device))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public async Task<bool> ActivateConnection(string name)
        {

            var connections = await Settings.GetConnectionsAsync();

            // Find the connection with id "wg0"
            foreach (var connectionPath in connections)
            {
                var connectionProxy = Service.CreateConnection(connectionPath);
                var settings = await connectionProxy.GetSettingsAsync();
                if (settings.ContainsKey("connection") &&
                    settings["connection"].ContainsKey("interface-name") &&
                    settings["connection"]["interface-name"] == name)
                {

                    // Activate the connection
                    Console.WriteLine($"{connectionPath}");
                    await NetworkManager.ActivateConnectionAsync(connectionPath, "/", "/");
                    Console.WriteLine("wg0 connection activated.");
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> DisableConnection(string name)
        {
            var connections = await NetworkManager.GetActiveConnectionsAsync();

            var device = await NetworkManager.GetDeviceByIpIfaceAsync(name);

            // Find the connection with id "wg0"
            
            foreach (var connectionPath in connections)
            {
                var connectionProxy = Service.CreateActive(connectionPath);
                var devices = await connectionProxy.GetDevicesAsync();
                foreach(var i in devices)
                {
                    if (i.Equals(device))
                    {
                        await NetworkManager.DeactivateConnectionAsync(connectionPath);

                        return true;
                    }
                }
            }
            return false;
        }
        internal async Task<NetworkManagerClient> Clone()
        {
            var clone = new NetworkManagerClient();
            _clones.Add(clone);
            await clone.Initialize();
            return clone;
        }
        private async Task Initialize()
        {
            this.Connection = new Connection(Address.System!);
            await Connection.ConnectAsync();
            this.Service = new NetworkManagerService(Connection, "org.freedesktop.NetworkManager");
            this.NetworkManager = Service.CreateNetworkManager("/org/freedesktop/NetworkManager");
            this.Settings = Service.CreateSettings("/org/freedesktop/NetworkManager/Settings");
        }

        public async Task<HashSet<PathId>> GetActiveConnections()
        {
            HashSet<PathId> result = new HashSet<PathId>();
            
            var activeConnections = await this.NetworkManager.GetActiveConnectionsAsync();
            foreach (var i in activeConnections)
            {
                var active = this.Service.CreateActive(i);
                
                var connection = await active.GetConnectionAsync();
                result.Add(connection.ToString());
            }

            return result;
        }
        public async IAsyncEnumerable<ProfileInfo> GetProfiles()
        {
           
            var connections = await this.Settings.GetConnectionsAsync();
            foreach (var i in connections)
            {
                var con = this.Service.CreateConnection(i);
                
                yield return new ProfileInfo()
                {
                    Client = this,
                    FileName = await con.GetFilenameAsync(),
                    Id = i,
                };
            }
        }

        public async Task<DeviceInfo?> GetDevice(PathId devicePath)
        {
            var device = Service.CreateDevice(devicePath.Path);
            var interfaceName = await device.GetInterfaceAsync();
            var type = (DeviceType)await device.GetDeviceTypeAsync();
            var state = (DeviceState)await device.GetStateAsync();
            if (type == DeviceType.Wifi)
            {
                return new WifiDeviceInfo()
                {
                    Id = devicePath,
                    DeviceType = type,
                    InterfaceName = interfaceName,
                    Client = this,
                    State = state
                };
            }
            else
                return new DeviceInfo()
                {
                    Id = devicePath,
                    DeviceType = type,
                    InterfaceName = interfaceName,
                    Client = this,
                    State = state
                };

            return null;
        }
        public async IAsyncEnumerable<DeviceInfo> GetDevices()
        {

            foreach (var devicePath in await NetworkManager.GetDevicesAsync())
            {
                var d = await GetDevice(devicePath);
                if (d != null)
                    yield return d;
            }
        }

        public async Task RequestWifiScan()
        {
            var wifi = await GetAccessPoints().FirstOrDefault();
            await wifi.RequestScan();
        }
        
        public async IAsyncEnumerable<AccessPointInfo> GetAccessPoints()
        {
            await foreach (var d in GetDevices())
            {
                if (d.DeviceType != DeviceType.Wifi) continue;

                await foreach (var i in GetAccessPoints(d)) yield return i;
                yield break;
            }
        }
        public async IAsyncEnumerable<AccessPointInfo> GetAccessPoint(string interfaceName)
        {
            await foreach (var d in GetDevices())
            {
                if (d.DeviceType != DeviceType.Wifi || d.InterfaceName != interfaceName) continue;

                await foreach (var i in GetAccessPoints(d)) yield return i;
                yield break;
            }
        }
        private async IAsyncEnumerable<AccessPointInfo> GetAccessPoints(DeviceInfo d)
        {
            var wifi = Service.CreateWireless(d.Id.Path);
            var list = await wifi.GetAccessPointsAsync();

            foreach (var accessPoint in list.Select(x => Service.CreateAccessPoint(x)))
            {
                yield return await OnGetAccessPoint(this, accessPoint, d);
            }
        }

        internal static async Task<AccessPointInfo> OnGetAccessPoint(NetworkManagerClient client, AccessPoint accessPoint, DeviceInfo d)
        {
            var wssid = await accessPoint.GetSsidAsync();
            var strength = await accessPoint.GetStrengthAsync();
            var s = Encoding.UTF8.GetString(wssid);
            //var wpfFlags = await accessPoint.GetWpaFlagsAsync();
            var mode = (WifiAccessPointMode)await accessPoint.GetModeAsync();
            var maxKbps = await accessPoint.GetMaxBitrateAsync();

            return new AccessPointInfo
            {
                AccessPointMode = mode,
                AccessPointPath = accessPoint.Path,
                DevicePath = d.Id.Path,
                MaxKbitRate = maxKbps,
                SignalStrength = strength,
                Ssid = s,
                SourceInterface = d.InterfaceName,
                SourceDevice = d,
                Client = client
            };
        }
        public async ValueTask DisposeAsync()
        {
            if (Connection is IAsyncDisposable connectionAsyncDisposable)
                await connectionAsyncDisposable.DisposeAsync();
            else
                Connection.Dispose();
            await Task.WhenAll(_clones.Select(x => x.DisposeAsync().AsTask()));
            _clones.Clear();
        }
    }
}
