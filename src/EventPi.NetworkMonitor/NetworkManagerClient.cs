using NetworkManager.DBus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EventPi.Abstractions;
using NetworkManager.DBus;
using Tmds.DBus.Protocol;
using Connection = Tmds.DBus.Protocol.Connection;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace EventPi.NetworkMonitor
{
    
    static class Extension
    {
        public static async Task<T> FirstOrDefault<T>(this IAsyncEnumerable<T> items, Predicate<T>? predicate = null)
        {
            predicate ??= x => true;
            await foreach (var i in items)
            {
                if(predicate(i)) return i;
            }

            return default;
        }
    }

    public readonly struct PathId : IEquatable<PathId>, IComparable<PathId>, IComparable
    {
        private readonly Guid _id;
        private readonly string? _path;

        private PathId(Guid id)
        {
            _id = id;
            _path = null;
        }

        private PathId(string path)
        {
            _path = path;
            _id = _path.ToGuid();
        }
        public string Path => _path;
        public Guid Id => _id;

        public static implicit operator PathId(string path) => new PathId(path);
        public static implicit operator PathId(Guid id) => new PathId(id);
        public static implicit operator ObjectPath(PathId p) => p.Path;
        public static implicit operator PathId(ObjectPath p) => new PathId(p);

        public bool Equals(PathId other) => _id.Equals(other._id);

        public override bool Equals(object? obj) => obj is PathId other && Equals(other);

        public override int GetHashCode() => _id.GetHashCode();

        public static bool operator ==(PathId left, PathId right) => left.Equals(right);

        public static bool operator !=(PathId left, PathId right) => !left.Equals(right);

        public int CompareTo(PathId other) => _id.CompareTo(other._id);

        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is PathId other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(PathId)}");
        }

        public override string ToString() => _path ?? _id.ToString();
    }
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
        public async IAsyncEnumerable<DeviceInfo> GetDevices()
        {

            foreach (var devicePath in await NetworkManager.GetDevicesAsync())
            {
                var device = Service.CreateDevice(devicePath);
                var interfaceName = await device.GetInterfaceAsync();
                var type = (DeviceType)await device.GetDeviceTypeAsync();
                
                if (type == DeviceType.Wifi)
                {
                    yield return new WifiDeviceInfo()
                    {
                        Id = devicePath,
                        DeviceType = type,
                        InterfaceName = interfaceName,
                        Client = this
                    };
                }
                else
                    yield return new DeviceInfo()
                    {
                        Id = devicePath,
                        DeviceType = type,
                        InterfaceName = interfaceName,
                        Client = this
                    };
            }
        }

        public async Task RequestWifiScan()
        {
            var wifi = await GetWifiNetworks().FirstOrDefault();
            await wifi.RequestScan();
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
            var wifi = Service.CreateWireless(d.Id.Path);
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
                    DevicePath = d.Id.Path,
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
            await Task.WhenAll(_clones.Select(x => x.DisposeAsync().AsTask()));
            _clones.Clear();
        }
    }
}
