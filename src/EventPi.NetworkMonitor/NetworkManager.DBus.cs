namespace NetworkManager.DBus
{
    using System;
    using Tmds.DBus.Protocol;
    using SafeHandle = System.Runtime.InteropServices.SafeHandle;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    partial class ObjectManager : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.DBus.ObjectManager";
        internal ObjectManager(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal Task<Dictionary<ObjectPath, Dictionary<string, Dictionary<string, VariantValue>>>> GetManagedObjectsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aeoaesaesv(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "GetManagedObjects");
                return writer.CreateMessage();
            }
        }
        internal ValueTask<IDisposable> WatchInterfacesAddedAsync(Action<Exception?, (ObjectPath ObjectPath, Dictionary<string, Dictionary<string, VariantValue>> InterfacesAndProperties)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "InterfacesAdded", (Message m, object? s) => ReadMessage_oaesaesv(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
        internal ValueTask<IDisposable> WatchInterfacesRemovedAsync(Action<Exception?, (ObjectPath ObjectPath, string[] Interfaces)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "InterfacesRemoved", (Message m, object? s) => ReadMessage_oas(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
    }
    record NetworkManagerProperties
    {
        internal ObjectPath[] Devices { get; set; } = default!;
        internal ObjectPath[] AllDevices { get; set; } = default!;
        internal ObjectPath[] Checkpoints { get; set; } = default!;
        internal bool NetworkingEnabled { get; set; } = default!;
        internal bool WirelessEnabled { get; set; } = default!;
        internal bool WirelessHardwareEnabled { get; set; } = default!;
        internal bool WwanEnabled { get; set; } = default!;
        internal bool WwanHardwareEnabled { get; set; } = default!;
        internal bool WimaxEnabled { get; set; } = default!;
        internal bool WimaxHardwareEnabled { get; set; } = default!;
        internal uint RadioFlags { get; set; } = default!;
        internal ObjectPath[] ActiveConnections { get; set; } = default!;
        internal ObjectPath PrimaryConnection { get; set; } = default!;
        internal string PrimaryConnectionType { get; set; } = default!;
        internal uint Metered { get; set; } = default!;
        internal ObjectPath ActivatingConnection { get; set; } = default!;
        internal bool Startup { get; set; } = default!;
        internal string Version { get; set; } = default!;
        internal uint[] VersionInfo { get; set; } = default!;
        internal uint[] Capabilities { get; set; } = default!;
        internal uint State { get; set; } = default!;
        internal uint Connectivity { get; set; } = default!;
        internal bool ConnectivityCheckAvailable { get; set; } = default!;
        internal bool ConnectivityCheckEnabled { get; set; } = default!;
        internal string ConnectivityCheckUri { get; set; } = default!;
        internal Dictionary<string, VariantValue> GlobalDnsConfiguration { get; set; } = default!;
    }
    partial class NetworkManager : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager";
        internal NetworkManager(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal Task ReloadAsync(uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "u",
                    member: "Reload");
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        internal Task<ObjectPath[]> GetDevicesAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_ao(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "GetDevices");
                return writer.CreateMessage();
            }
        }
        internal Task<ObjectPath[]> GetAllDevicesAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_ao(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "GetAllDevices");
                return writer.CreateMessage();
            }
        }
        internal Task<ObjectPath> GetDeviceByIpIfaceAsync(string iface)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_o(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "GetDeviceByIpIface");
                writer.WriteString(iface);
                return writer.CreateMessage();
            }
        }
        internal Task<ObjectPath> ActivateConnectionAsync(ObjectPath connection, ObjectPath device, ObjectPath specificObject)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_o(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ooo",
                    member: "ActivateConnection");
                writer.WriteObjectPath(connection);
                writer.WriteObjectPath(device);
                writer.WriteObjectPath(specificObject);
                return writer.CreateMessage();
            }
        }
        internal Task<(ObjectPath Path, ObjectPath ActiveConnection)> AddAndActivateConnectionAsync(Dictionary<string, Dictionary<string, Variant>> connection, ObjectPath device, ObjectPath specificObject)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_oo(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "a{sa{sv}}oo",
                    member: "AddAndActivateConnection");
                WriteType_aesaesv(ref writer, connection);
                writer.WriteObjectPath(device);
                writer.WriteObjectPath(specificObject);
                return writer.CreateMessage();
            }
        }
        internal Task<(ObjectPath Path, ObjectPath ActiveConnection, Dictionary<string, VariantValue> Result)> AddAndActivateConnection2Async(Dictionary<string, Dictionary<string, Variant>> connection, ObjectPath device, ObjectPath specificObject, Dictionary<string, Variant> options)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_ooaesv(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "a{sa{sv}}ooa{sv}",
                    member: "AddAndActivateConnection2");
                WriteType_aesaesv(ref writer, connection);
                writer.WriteObjectPath(device);
                writer.WriteObjectPath(specificObject);
                writer.WriteDictionary(options);
                return writer.CreateMessage();
            }
        }
        internal Task DeactivateConnectionAsync(ObjectPath activeConnection)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "o",
                    member: "DeactivateConnection");
                writer.WriteObjectPath(activeConnection);
                return writer.CreateMessage();
            }
        }
        internal Task SleepAsync(bool sleep)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "b",
                    member: "Sleep");
                writer.WriteBool(sleep);
                return writer.CreateMessage();
            }
        }
        internal Task EnableAsync(bool enable)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "b",
                    member: "Enable");
                writer.WriteBool(enable);
                return writer.CreateMessage();
            }
        }
        internal Task<Dictionary<string, string>> GetPermissionsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aess(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "GetPermissions");
                return writer.CreateMessage();
            }
        }
        internal Task SetLoggingAsync(string level, string domains)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ss",
                    member: "SetLogging");
                writer.WriteString(level);
                writer.WriteString(domains);
                return writer.CreateMessage();
            }
        }
        internal Task<(string Level, string Domains)> GetLoggingAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_ss(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "GetLogging");
                return writer.CreateMessage();
            }
        }
        internal Task<uint> CheckConnectivityAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_u(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "CheckConnectivity");
                return writer.CreateMessage();
            }
        }
        internal Task<uint> StateAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_u(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "state");
                return writer.CreateMessage();
            }
        }
        internal Task<ObjectPath> CheckpointCreateAsync(ObjectPath[] devices, uint rollbackTimeout, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_o(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "aouu",
                    member: "CheckpointCreate");
                writer.WriteArray(devices);
                writer.WriteUInt32(rollbackTimeout);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        internal Task CheckpointDestroyAsync(ObjectPath checkpoint)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "o",
                    member: "CheckpointDestroy");
                writer.WriteObjectPath(checkpoint);
                return writer.CreateMessage();
            }
        }
        internal Task<Dictionary<string, uint>> CheckpointRollbackAsync(ObjectPath checkpoint)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aesu(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "o",
                    member: "CheckpointRollback");
                writer.WriteObjectPath(checkpoint);
                return writer.CreateMessage();
            }
        }
        internal Task CheckpointAdjustRollbackTimeoutAsync(ObjectPath checkpoint, uint addTimeout)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ou",
                    member: "CheckpointAdjustRollbackTimeout");
                writer.WriteObjectPath(checkpoint);
                writer.WriteUInt32(addTimeout);
                return writer.CreateMessage();
            }
        }
        internal ValueTask<IDisposable> WatchCheckPermissionsAsync(Action<Exception?> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "CheckPermissions", handler, emitOnCapturedContext, flags);
        internal ValueTask<IDisposable> WatchStateChangedAsync(Action<Exception?, uint> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "StateChanged", (Message m, object? s) => ReadMessage_u(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
        internal ValueTask<IDisposable> WatchDeviceAddedAsync(Action<Exception?, ObjectPath> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "DeviceAdded", (Message m, object? s) => ReadMessage_o(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
        internal ValueTask<IDisposable> WatchDeviceRemovedAsync(Action<Exception?, ObjectPath> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "DeviceRemoved", (Message m, object? s) => ReadMessage_o(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
        internal Task SetDevicesAsync(ObjectPath[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Devices");
                writer.WriteSignature("ao");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetAllDevicesAsync(ObjectPath[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("AllDevices");
                writer.WriteSignature("ao");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetCheckpointsAsync(ObjectPath[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Checkpoints");
                writer.WriteSignature("ao");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetNetworkingEnabledAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("NetworkingEnabled");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetWirelessEnabledAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("WirelessEnabled");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetWirelessHardwareEnabledAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("WirelessHardwareEnabled");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetWwanEnabledAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("WwanEnabled");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetWwanHardwareEnabledAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("WwanHardwareEnabled");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetWimaxEnabledAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("WimaxEnabled");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetWimaxHardwareEnabledAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("WimaxHardwareEnabled");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetRadioFlagsAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("RadioFlags");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetActiveConnectionsAsync(ObjectPath[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("ActiveConnections");
                writer.WriteSignature("ao");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetPrimaryConnectionAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("PrimaryConnection");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetPrimaryConnectionTypeAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("PrimaryConnectionType");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetMeteredAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Metered");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetActivatingConnectionAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("ActivatingConnection");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetStartupAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Startup");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetVersionAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Version");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetVersionInfoAsync(uint[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("VersionInfo");
                writer.WriteSignature("au");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetCapabilitiesAsync(uint[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Capabilities");
                writer.WriteSignature("au");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetStateAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("State");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetConnectivityAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Connectivity");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetConnectivityCheckAvailableAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("ConnectivityCheckAvailable");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetConnectivityCheckEnabledAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("ConnectivityCheckEnabled");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetConnectivityCheckUriAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("ConnectivityCheckUri");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetGlobalDnsConfigurationAsync(Dictionary<string, Variant> value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("GlobalDnsConfiguration");
                writer.WriteSignature("a{sv}");
                writer.WriteDictionary(value);
                return writer.CreateMessage();
            }
        }
        //internal Task<ObjectPath[]> GetDevicesAsync()
        //    => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Devices"), (Message m, object? s) => ReadMessage_v_ao(m, (NetworkManagerObject)s!), this);
        //internal Task<ObjectPath[]> GetAllDevicesAsync()
        //    => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "AllDevices"), (Message m, object? s) => ReadMessage_v_ao(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath[]> GetCheckpointsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Checkpoints"), (Message m, object? s) => ReadMessage_v_ao(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetNetworkingEnabledAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "NetworkingEnabled"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetWirelessEnabledAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "WirelessEnabled"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetWirelessHardwareEnabledAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "WirelessHardwareEnabled"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetWwanEnabledAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "WwanEnabled"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetWwanHardwareEnabledAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "WwanHardwareEnabled"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetWimaxEnabledAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "WimaxEnabled"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetWimaxHardwareEnabledAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "WimaxHardwareEnabled"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetRadioFlagsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "RadioFlags"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath[]> GetActiveConnectionsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "ActiveConnections"), (Message m, object? s) => ReadMessage_v_ao(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath> GetPrimaryConnectionAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "PrimaryConnection"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetPrimaryConnectionTypeAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "PrimaryConnectionType"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetMeteredAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Metered"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath> GetActivatingConnectionAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "ActivatingConnection"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetStartupAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Startup"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetVersionAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Version"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<uint[]> GetVersionInfoAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "VersionInfo"), (Message m, object? s) => ReadMessage_v_au(m, (NetworkManagerObject)s!), this);
        internal Task<uint[]> GetCapabilitiesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Capabilities"), (Message m, object? s) => ReadMessage_v_au(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetStateAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "State"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetConnectivityAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Connectivity"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetConnectivityCheckAvailableAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "ConnectivityCheckAvailable"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetConnectivityCheckEnabledAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "ConnectivityCheckEnabled"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetConnectivityCheckUriAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "ConnectivityCheckUri"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<Dictionary<string, VariantValue>> GetGlobalDnsConfigurationAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "GlobalDnsConfiguration"), (Message m, object? s) => ReadMessage_v_aesv(m, (NetworkManagerObject)s!), this);
        internal Task<NetworkManagerProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), this);
            static NetworkManagerProperties ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        internal ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<NetworkManagerProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<NetworkManagerProperties> ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<NetworkManagerProperties>(ReadProperties(ref reader, changed), changed.ToArray(), ReadInvalidated(ref reader));
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "Devices": invalidated.Add("Devices"); break;
                        case "AllDevices": invalidated.Add("AllDevices"); break;
                        case "Checkpoints": invalidated.Add("Checkpoints"); break;
                        case "NetworkingEnabled": invalidated.Add("NetworkingEnabled"); break;
                        case "WirelessEnabled": invalidated.Add("WirelessEnabled"); break;
                        case "WirelessHardwareEnabled": invalidated.Add("WirelessHardwareEnabled"); break;
                        case "WwanEnabled": invalidated.Add("WwanEnabled"); break;
                        case "WwanHardwareEnabled": invalidated.Add("WwanHardwareEnabled"); break;
                        case "WimaxEnabled": invalidated.Add("WimaxEnabled"); break;
                        case "WimaxHardwareEnabled": invalidated.Add("WimaxHardwareEnabled"); break;
                        case "RadioFlags": invalidated.Add("RadioFlags"); break;
                        case "ActiveConnections": invalidated.Add("ActiveConnections"); break;
                        case "PrimaryConnection": invalidated.Add("PrimaryConnection"); break;
                        case "PrimaryConnectionType": invalidated.Add("PrimaryConnectionType"); break;
                        case "Metered": invalidated.Add("Metered"); break;
                        case "ActivatingConnection": invalidated.Add("ActivatingConnection"); break;
                        case "Startup": invalidated.Add("Startup"); break;
                        case "Version": invalidated.Add("Version"); break;
                        case "VersionInfo": invalidated.Add("VersionInfo"); break;
                        case "Capabilities": invalidated.Add("Capabilities"); break;
                        case "State": invalidated.Add("State"); break;
                        case "Connectivity": invalidated.Add("Connectivity"); break;
                        case "ConnectivityCheckAvailable": invalidated.Add("ConnectivityCheckAvailable"); break;
                        case "ConnectivityCheckEnabled": invalidated.Add("ConnectivityCheckEnabled"); break;
                        case "ConnectivityCheckUri": invalidated.Add("ConnectivityCheckUri"); break;
                        case "GlobalDnsConfiguration": invalidated.Add("GlobalDnsConfiguration"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static NetworkManagerProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new NetworkManagerProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "Devices":
                        reader.ReadSignature("ao");
                        props.Devices = reader.ReadArrayOfObjectPath();
                        changedList?.Add("Devices");
                        break;
                    case "AllDevices":
                        reader.ReadSignature("ao");
                        props.AllDevices = reader.ReadArrayOfObjectPath();
                        changedList?.Add("AllDevices");
                        break;
                    case "Checkpoints":
                        reader.ReadSignature("ao");
                        props.Checkpoints = reader.ReadArrayOfObjectPath();
                        changedList?.Add("Checkpoints");
                        break;
                    case "NetworkingEnabled":
                        reader.ReadSignature("b");
                        props.NetworkingEnabled = reader.ReadBool();
                        changedList?.Add("NetworkingEnabled");
                        break;
                    case "WirelessEnabled":
                        reader.ReadSignature("b");
                        props.WirelessEnabled = reader.ReadBool();
                        changedList?.Add("WirelessEnabled");
                        break;
                    case "WirelessHardwareEnabled":
                        reader.ReadSignature("b");
                        props.WirelessHardwareEnabled = reader.ReadBool();
                        changedList?.Add("WirelessHardwareEnabled");
                        break;
                    case "WwanEnabled":
                        reader.ReadSignature("b");
                        props.WwanEnabled = reader.ReadBool();
                        changedList?.Add("WwanEnabled");
                        break;
                    case "WwanHardwareEnabled":
                        reader.ReadSignature("b");
                        props.WwanHardwareEnabled = reader.ReadBool();
                        changedList?.Add("WwanHardwareEnabled");
                        break;
                    case "WimaxEnabled":
                        reader.ReadSignature("b");
                        props.WimaxEnabled = reader.ReadBool();
                        changedList?.Add("WimaxEnabled");
                        break;
                    case "WimaxHardwareEnabled":
                        reader.ReadSignature("b");
                        props.WimaxHardwareEnabled = reader.ReadBool();
                        changedList?.Add("WimaxHardwareEnabled");
                        break;
                    case "RadioFlags":
                        reader.ReadSignature("u");
                        props.RadioFlags = reader.ReadUInt32();
                        changedList?.Add("RadioFlags");
                        break;
                    case "ActiveConnections":
                        reader.ReadSignature("ao");
                        props.ActiveConnections = reader.ReadArrayOfObjectPath();
                        changedList?.Add("ActiveConnections");
                        break;
                    case "PrimaryConnection":
                        reader.ReadSignature("o");
                        props.PrimaryConnection = reader.ReadObjectPath();
                        changedList?.Add("PrimaryConnection");
                        break;
                    case "PrimaryConnectionType":
                        reader.ReadSignature("s");
                        props.PrimaryConnectionType = reader.ReadString();
                        changedList?.Add("PrimaryConnectionType");
                        break;
                    case "Metered":
                        reader.ReadSignature("u");
                        props.Metered = reader.ReadUInt32();
                        changedList?.Add("Metered");
                        break;
                    case "ActivatingConnection":
                        reader.ReadSignature("o");
                        props.ActivatingConnection = reader.ReadObjectPath();
                        changedList?.Add("ActivatingConnection");
                        break;
                    case "Startup":
                        reader.ReadSignature("b");
                        props.Startup = reader.ReadBool();
                        changedList?.Add("Startup");
                        break;
                    case "Version":
                        reader.ReadSignature("s");
                        props.Version = reader.ReadString();
                        changedList?.Add("Version");
                        break;
                    case "VersionInfo":
                        reader.ReadSignature("au");
                        props.VersionInfo = reader.ReadArrayOfUInt32();
                        changedList?.Add("VersionInfo");
                        break;
                    case "Capabilities":
                        reader.ReadSignature("au");
                        props.Capabilities = reader.ReadArrayOfUInt32();
                        changedList?.Add("Capabilities");
                        break;
                    case "State":
                        reader.ReadSignature("u");
                        props.State = reader.ReadUInt32();
                        changedList?.Add("State");
                        break;
                    case "Connectivity":
                        reader.ReadSignature("u");
                        props.Connectivity = reader.ReadUInt32();
                        changedList?.Add("Connectivity");
                        break;
                    case "ConnectivityCheckAvailable":
                        reader.ReadSignature("b");
                        props.ConnectivityCheckAvailable = reader.ReadBool();
                        changedList?.Add("ConnectivityCheckAvailable");
                        break;
                    case "ConnectivityCheckEnabled":
                        reader.ReadSignature("b");
                        props.ConnectivityCheckEnabled = reader.ReadBool();
                        changedList?.Add("ConnectivityCheckEnabled");
                        break;
                    case "ConnectivityCheckUri":
                        reader.ReadSignature("s");
                        props.ConnectivityCheckUri = reader.ReadString();
                        changedList?.Add("ConnectivityCheckUri");
                        break;
                    case "GlobalDnsConfiguration":
                        reader.ReadSignature("a{sv}");
                        props.GlobalDnsConfiguration = reader.ReadDictionaryOfStringToVariantValue();
                        changedList?.Add("GlobalDnsConfiguration");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record IP4ConfigProperties
    {
        internal uint[][] Addresses { get; set; } = default!;
        internal Dictionary<string, VariantValue>[] AddressData { get; set; } = default!;
        internal string Gateway { get; set; } = default!;
        internal uint[][] Routes { get; set; } = default!;
        internal Dictionary<string, VariantValue>[] RouteData { get; set; } = default!;
        internal Dictionary<string, VariantValue>[] NameserverData { get; set; } = default!;
        internal uint[] Nameservers { get; set; } = default!;
        internal string[] Domains { get; set; } = default!;
        internal string[] Searches { get; set; } = default!;
        internal string[] DnsOptions { get; set; } = default!;
        internal int DnsPriority { get; set; } = default!;
        internal string[] WinsServerData { get; set; } = default!;
        internal uint[] WinsServers { get; set; } = default!;
    }
    partial class IP4Config : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.IP4Config";
        internal IP4Config(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal Task SetAddressesAsync(uint[][] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Addresses");
                writer.WriteSignature("aau");
                WriteType_aau(ref writer, value);
                return writer.CreateMessage();
            }
        }
        internal Task SetAddressDataAsync(Dictionary<string, Variant>[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("AddressData");
                writer.WriteSignature("aa{sv}");
                WriteType_aaesv(ref writer, value);
                return writer.CreateMessage();
            }
        }
        internal Task SetGatewayAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Gateway");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetRoutesAsync(uint[][] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Routes");
                writer.WriteSignature("aau");
                WriteType_aau(ref writer, value);
                return writer.CreateMessage();
            }
        }
        internal Task SetRouteDataAsync(Dictionary<string, Variant>[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("RouteData");
                writer.WriteSignature("aa{sv}");
                WriteType_aaesv(ref writer, value);
                return writer.CreateMessage();
            }
        }
        internal Task SetNameserverDataAsync(Dictionary<string, Variant>[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("NameserverData");
                writer.WriteSignature("aa{sv}");
                WriteType_aaesv(ref writer, value);
                return writer.CreateMessage();
            }
        }
        internal Task SetNameserversAsync(uint[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Nameservers");
                writer.WriteSignature("au");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetDomainsAsync(string[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Domains");
                writer.WriteSignature("as");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetSearchesAsync(string[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Searches");
                writer.WriteSignature("as");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetDnsOptionsAsync(string[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("DnsOptions");
                writer.WriteSignature("as");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetDnsPriorityAsync(int value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("DnsPriority");
                writer.WriteSignature("i");
                writer.WriteInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetWinsServerDataAsync(string[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("WinsServerData");
                writer.WriteSignature("as");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetWinsServersAsync(uint[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("WinsServers");
                writer.WriteSignature("au");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task<uint[][]> GetAddressesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Addresses"), (Message m, object? s) => ReadMessage_v_aau(m, (NetworkManagerObject)s!), this);
        internal Task<Dictionary<string, VariantValue>[]> GetAddressDataAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "AddressData"), (Message m, object? s) => ReadMessage_v_aaesv(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetGatewayAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Gateway"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<uint[][]> GetRoutesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Routes"), (Message m, object? s) => ReadMessage_v_aau(m, (NetworkManagerObject)s!), this);
        internal Task<Dictionary<string, VariantValue>[]> GetRouteDataAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "RouteData"), (Message m, object? s) => ReadMessage_v_aaesv(m, (NetworkManagerObject)s!), this);
        internal Task<Dictionary<string, VariantValue>[]> GetNameserverDataAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "NameserverData"), (Message m, object? s) => ReadMessage_v_aaesv(m, (NetworkManagerObject)s!), this);
        internal Task<uint[]> GetNameserversAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Nameservers"), (Message m, object? s) => ReadMessage_v_au(m, (NetworkManagerObject)s!), this);
        internal Task<string[]> GetDomainsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Domains"), (Message m, object? s) => ReadMessage_v_as(m, (NetworkManagerObject)s!), this);
        internal Task<string[]> GetSearchesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Searches"), (Message m, object? s) => ReadMessage_v_as(m, (NetworkManagerObject)s!), this);
        internal Task<string[]> GetDnsOptionsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "DnsOptions"), (Message m, object? s) => ReadMessage_v_as(m, (NetworkManagerObject)s!), this);
        internal Task<int> GetDnsPriorityAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "DnsPriority"), (Message m, object? s) => ReadMessage_v_i(m, (NetworkManagerObject)s!), this);
        internal Task<string[]> GetWinsServerDataAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "WinsServerData"), (Message m, object? s) => ReadMessage_v_as(m, (NetworkManagerObject)s!), this);
        internal Task<uint[]> GetWinsServersAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "WinsServers"), (Message m, object? s) => ReadMessage_v_au(m, (NetworkManagerObject)s!), this);
        internal Task<IP4ConfigProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), this);
            static IP4ConfigProperties ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        internal ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<IP4ConfigProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<IP4ConfigProperties> ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<IP4ConfigProperties>(ReadProperties(ref reader, changed), changed.ToArray(), ReadInvalidated(ref reader));
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "Addresses": invalidated.Add("Addresses"); break;
                        case "AddressData": invalidated.Add("AddressData"); break;
                        case "Gateway": invalidated.Add("Gateway"); break;
                        case "Routes": invalidated.Add("Routes"); break;
                        case "RouteData": invalidated.Add("RouteData"); break;
                        case "NameserverData": invalidated.Add("NameserverData"); break;
                        case "Nameservers": invalidated.Add("Nameservers"); break;
                        case "Domains": invalidated.Add("Domains"); break;
                        case "Searches": invalidated.Add("Searches"); break;
                        case "DnsOptions": invalidated.Add("DnsOptions"); break;
                        case "DnsPriority": invalidated.Add("DnsPriority"); break;
                        case "WinsServerData": invalidated.Add("WinsServerData"); break;
                        case "WinsServers": invalidated.Add("WinsServers"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static IP4ConfigProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new IP4ConfigProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "Addresses":
                        reader.ReadSignature("aau");
                        props.Addresses = ReadType_aau(ref reader);
                        changedList?.Add("Addresses");
                        break;
                    case "AddressData":
                        reader.ReadSignature("aa{sv}");
                        props.AddressData = ReadType_aaesv(ref reader);
                        changedList?.Add("AddressData");
                        break;
                    case "Gateway":
                        reader.ReadSignature("s");
                        props.Gateway = reader.ReadString();
                        changedList?.Add("Gateway");
                        break;
                    case "Routes":
                        reader.ReadSignature("aau");
                        props.Routes = ReadType_aau(ref reader);
                        changedList?.Add("Routes");
                        break;
                    case "RouteData":
                        reader.ReadSignature("aa{sv}");
                        props.RouteData = ReadType_aaesv(ref reader);
                        changedList?.Add("RouteData");
                        break;
                    case "NameserverData":
                        reader.ReadSignature("aa{sv}");
                        props.NameserverData = ReadType_aaesv(ref reader);
                        changedList?.Add("NameserverData");
                        break;
                    case "Nameservers":
                        reader.ReadSignature("au");
                        props.Nameservers = reader.ReadArrayOfUInt32();
                        changedList?.Add("Nameservers");
                        break;
                    case "Domains":
                        reader.ReadSignature("as");
                        props.Domains = reader.ReadArrayOfString();
                        changedList?.Add("Domains");
                        break;
                    case "Searches":
                        reader.ReadSignature("as");
                        props.Searches = reader.ReadArrayOfString();
                        changedList?.Add("Searches");
                        break;
                    case "DnsOptions":
                        reader.ReadSignature("as");
                        props.DnsOptions = reader.ReadArrayOfString();
                        changedList?.Add("DnsOptions");
                        break;
                    case "DnsPriority":
                        reader.ReadSignature("i");
                        props.DnsPriority = reader.ReadInt32();
                        changedList?.Add("DnsPriority");
                        break;
                    case "WinsServerData":
                        reader.ReadSignature("as");
                        props.WinsServerData = reader.ReadArrayOfString();
                        changedList?.Add("WinsServerData");
                        break;
                    case "WinsServers":
                        reader.ReadSignature("au");
                        props.WinsServers = reader.ReadArrayOfUInt32();
                        changedList?.Add("WinsServers");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record ActiveProperties
    {
        internal ObjectPath Connection { get; set; } = default!;
        internal ObjectPath SpecificObject { get; set; } = default!;
        internal string Id { get; set; } = default!;
        internal string Uuid { get; set; } = default!;
        internal string Type { get; set; } = default!;
        internal ObjectPath[] Devices { get; set; } = default!;
        internal uint State { get; set; } = default!;
        internal uint StateFlags { get; set; } = default!;
        internal bool Default { get; set; } = default!;
        internal ObjectPath Ip4Config { get; set; } = default!;
        internal ObjectPath Dhcp4Config { get; set; } = default!;
        internal bool Default6 { get; set; } = default!;
        internal ObjectPath Ip6Config { get; set; } = default!;
        internal ObjectPath Dhcp6Config { get; set; } = default!;
        internal bool Vpn { get; set; } = default!;
        internal ObjectPath Controller { get; set; } = default!;
        internal ObjectPath Master { get; set; } = default!;
    }
    partial class Active : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.Connection.Active";
        internal Active(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal ValueTask<IDisposable> WatchStateChangedAsync(Action<Exception?, (uint State, uint Reason)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "StateChanged", (Message m, object? s) => ReadMessage_uu(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
        internal Task SetConnectionAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Connection");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetSpecificObjectAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("SpecificObject");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetIdAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Id");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetUuidAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Uuid");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetTypeAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Type");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetDevicesAsync(ObjectPath[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Devices");
                writer.WriteSignature("ao");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetStateAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("State");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetStateFlagsAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("StateFlags");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetDefaultAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Default");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetIp4ConfigAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Ip4Config");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetDhcp4ConfigAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Dhcp4Config");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetDefault6Async(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Default6");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetIp6ConfigAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Ip6Config");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetDhcp6ConfigAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Dhcp6Config");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetVpnAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Vpn");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetControllerAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Controller");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetMasterAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Master");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task<ObjectPath> GetConnectionAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Connection"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath> GetSpecificObjectAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "SpecificObject"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetIdAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Id"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetUuidAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Uuid"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetTypeAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Type"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath[]> GetDevicesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Devices"), (Message m, object? s) => ReadMessage_v_ao(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetStateAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "State"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetStateFlagsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "StateFlags"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetDefaultAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Default"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath> GetIp4ConfigAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Ip4Config"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath> GetDhcp4ConfigAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Dhcp4Config"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetDefault6Async()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Default6"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath> GetIp6ConfigAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Ip6Config"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath> GetDhcp6ConfigAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Dhcp6Config"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetVpnAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Vpn"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath> GetControllerAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Controller"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath> GetMasterAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Master"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<ActiveProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), this);
            static ActiveProperties ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        internal ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<ActiveProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<ActiveProperties> ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<ActiveProperties>(ReadProperties(ref reader, changed), changed.ToArray(), ReadInvalidated(ref reader));
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "Connection": invalidated.Add("Connection"); break;
                        case "SpecificObject": invalidated.Add("SpecificObject"); break;
                        case "Id": invalidated.Add("Id"); break;
                        case "Uuid": invalidated.Add("Uuid"); break;
                        case "Type": invalidated.Add("Type"); break;
                        case "Devices": invalidated.Add("Devices"); break;
                        case "State": invalidated.Add("State"); break;
                        case "StateFlags": invalidated.Add("StateFlags"); break;
                        case "Default": invalidated.Add("Default"); break;
                        case "Ip4Config": invalidated.Add("Ip4Config"); break;
                        case "Dhcp4Config": invalidated.Add("Dhcp4Config"); break;
                        case "Default6": invalidated.Add("Default6"); break;
                        case "Ip6Config": invalidated.Add("Ip6Config"); break;
                        case "Dhcp6Config": invalidated.Add("Dhcp6Config"); break;
                        case "Vpn": invalidated.Add("Vpn"); break;
                        case "Controller": invalidated.Add("Controller"); break;
                        case "Master": invalidated.Add("Master"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static ActiveProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new ActiveProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "Connection":
                        reader.ReadSignature("o");
                        props.Connection = reader.ReadObjectPath();
                        changedList?.Add("Connection");
                        break;
                    case "SpecificObject":
                        reader.ReadSignature("o");
                        props.SpecificObject = reader.ReadObjectPath();
                        changedList?.Add("SpecificObject");
                        break;
                    case "Id":
                        reader.ReadSignature("s");
                        props.Id = reader.ReadString();
                        changedList?.Add("Id");
                        break;
                    case "Uuid":
                        reader.ReadSignature("s");
                        props.Uuid = reader.ReadString();
                        changedList?.Add("Uuid");
                        break;
                    case "Type":
                        reader.ReadSignature("s");
                        props.Type = reader.ReadString();
                        changedList?.Add("Type");
                        break;
                    case "Devices":
                        reader.ReadSignature("ao");
                        props.Devices = reader.ReadArrayOfObjectPath();
                        changedList?.Add("Devices");
                        break;
                    case "State":
                        reader.ReadSignature("u");
                        props.State = reader.ReadUInt32();
                        changedList?.Add("State");
                        break;
                    case "StateFlags":
                        reader.ReadSignature("u");
                        props.StateFlags = reader.ReadUInt32();
                        changedList?.Add("StateFlags");
                        break;
                    case "Default":
                        reader.ReadSignature("b");
                        props.Default = reader.ReadBool();
                        changedList?.Add("Default");
                        break;
                    case "Ip4Config":
                        reader.ReadSignature("o");
                        props.Ip4Config = reader.ReadObjectPath();
                        changedList?.Add("Ip4Config");
                        break;
                    case "Dhcp4Config":
                        reader.ReadSignature("o");
                        props.Dhcp4Config = reader.ReadObjectPath();
                        changedList?.Add("Dhcp4Config");
                        break;
                    case "Default6":
                        reader.ReadSignature("b");
                        props.Default6 = reader.ReadBool();
                        changedList?.Add("Default6");
                        break;
                    case "Ip6Config":
                        reader.ReadSignature("o");
                        props.Ip6Config = reader.ReadObjectPath();
                        changedList?.Add("Ip6Config");
                        break;
                    case "Dhcp6Config":
                        reader.ReadSignature("o");
                        props.Dhcp6Config = reader.ReadObjectPath();
                        changedList?.Add("Dhcp6Config");
                        break;
                    case "Vpn":
                        reader.ReadSignature("b");
                        props.Vpn = reader.ReadBool();
                        changedList?.Add("Vpn");
                        break;
                    case "Controller":
                        reader.ReadSignature("o");
                        props.Controller = reader.ReadObjectPath();
                        changedList?.Add("Controller");
                        break;
                    case "Master":
                        reader.ReadSignature("o");
                        props.Master = reader.ReadObjectPath();
                        changedList?.Add("Master");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    partial class AgentManager : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.AgentManager";
        internal AgentManager(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal Task RegisterAsync(string identifier)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "Register");
                writer.WriteString(identifier);
                return writer.CreateMessage();
            }
        }
        internal Task RegisterWithCapabilitiesAsync(string identifier, uint capabilities)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "su",
                    member: "RegisterWithCapabilities");
                writer.WriteString(identifier);
                writer.WriteUInt32(capabilities);
                return writer.CreateMessage();
            }
        }
        internal Task UnregisterAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "Unregister");
                return writer.CreateMessage();
            }
        }
    }
    record StatisticsProperties
    {
        internal uint RefreshRateMs { get; set; } = default!;
        internal ulong TxBytes { get; set; } = default!;
        internal ulong RxBytes { get; set; } = default!;
    }
    partial class Statistics : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.Device.Statistics";
        internal Statistics(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal Task SetRefreshRateMsAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("RefreshRateMs");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetTxBytesAsync(ulong value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("TxBytes");
                writer.WriteSignature("t");
                writer.WriteUInt64(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetRxBytesAsync(ulong value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("RxBytes");
                writer.WriteSignature("t");
                writer.WriteUInt64(value);
                return writer.CreateMessage();
            }
        }
        internal Task<uint> GetRefreshRateMsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "RefreshRateMs"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<ulong> GetTxBytesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "TxBytes"), (Message m, object? s) => ReadMessage_v_t(m, (NetworkManagerObject)s!), this);
        internal Task<ulong> GetRxBytesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "RxBytes"), (Message m, object? s) => ReadMessage_v_t(m, (NetworkManagerObject)s!), this);
        internal Task<StatisticsProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), this);
            static StatisticsProperties ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        internal ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<StatisticsProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<StatisticsProperties> ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<StatisticsProperties>(ReadProperties(ref reader, changed), changed.ToArray(), ReadInvalidated(ref reader));
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "RefreshRateMs": invalidated.Add("RefreshRateMs"); break;
                        case "TxBytes": invalidated.Add("TxBytes"); break;
                        case "RxBytes": invalidated.Add("RxBytes"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static StatisticsProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new StatisticsProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "RefreshRateMs":
                        reader.ReadSignature("u");
                        props.RefreshRateMs = reader.ReadUInt32();
                        changedList?.Add("RefreshRateMs");
                        break;
                    case "TxBytes":
                        reader.ReadSignature("t");
                        props.TxBytes = reader.ReadUInt64();
                        changedList?.Add("TxBytes");
                        break;
                    case "RxBytes":
                        reader.ReadSignature("t");
                        props.RxBytes = reader.ReadUInt64();
                        changedList?.Add("RxBytes");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record WirelessProperties
    {
        internal string HwAddress { get; set; } = default!;
        internal string PermHwAddress { get; set; } = default!;
        internal uint Mode { get; set; } = default!;
        internal uint Bitrate { get; set; } = default!;
        internal ObjectPath[] AccessPoints { get; set; } = default!;
        internal ObjectPath ActiveAccessPoint { get; set; } = default!;
        internal uint WirelessCapabilities { get; set; } = default!;
        internal long LastScan { get; set; } = default!;
    }
    partial class Wireless : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.Device.Wireless";
        internal Wireless(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal Task<ObjectPath[]> GetAccessPointsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_ao(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "GetAccessPoints");
                return writer.CreateMessage();
            }
        }
        internal Task<ObjectPath[]> GetAllAccessPointsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_ao(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "GetAllAccessPoints");
                return writer.CreateMessage();
            }
        }
        internal Task RequestScanAsync(Dictionary<string, Variant> options)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "a{sv}",
                    member: "RequestScan");
                writer.WriteDictionary(options);
                return writer.CreateMessage();
            }
        }
        internal ValueTask<IDisposable> WatchAccessPointAddedAsync(Action<Exception?, ObjectPath> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "AccessPointAdded", (Message m, object? s) => ReadMessage_o(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
        internal ValueTask<IDisposable> WatchAccessPointRemovedAsync(Action<Exception?, ObjectPath> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "AccessPointRemoved", (Message m, object? s) => ReadMessage_o(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
        internal Task SetHwAddressAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("HwAddress");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetPermHwAddressAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("PermHwAddress");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetModeAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Mode");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetBitrateAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Bitrate");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetAccessPointsAsync(ObjectPath[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("AccessPoints");
                writer.WriteSignature("ao");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetActiveAccessPointAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("ActiveAccessPoint");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetWirelessCapabilitiesAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("WirelessCapabilities");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetLastScanAsync(long value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("LastScan");
                writer.WriteSignature("x");
                writer.WriteInt64(value);
                return writer.CreateMessage();
            }
        }
        internal Task<string> GetHwAddressAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "HwAddress"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetPermHwAddressAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "PermHwAddress"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetModeAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Mode"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetBitrateAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Bitrate"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        //internal Task<ObjectPath[]> GetAccessPointsAsync()
        //    => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "AccessPoints"), (Message m, object? s) => ReadMessage_v_ao(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath> GetActiveAccessPointAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "ActiveAccessPoint"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetWirelessCapabilitiesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "WirelessCapabilities"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<long> GetLastScanAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "LastScan"), (Message m, object? s) => ReadMessage_v_x(m, (NetworkManagerObject)s!), this);
        internal Task<WirelessProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), this);
            static WirelessProperties ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        internal ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<WirelessProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<WirelessProperties> ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<WirelessProperties>(ReadProperties(ref reader, changed), changed.ToArray(), ReadInvalidated(ref reader));
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "HwAddress": invalidated.Add("HwAddress"); break;
                        case "PermHwAddress": invalidated.Add("PermHwAddress"); break;
                        case "Mode": invalidated.Add("Mode"); break;
                        case "Bitrate": invalidated.Add("Bitrate"); break;
                        case "AccessPoints": invalidated.Add("AccessPoints"); break;
                        case "ActiveAccessPoint": invalidated.Add("ActiveAccessPoint"); break;
                        case "WirelessCapabilities": invalidated.Add("WirelessCapabilities"); break;
                        case "LastScan": invalidated.Add("LastScan"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static WirelessProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new WirelessProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "HwAddress":
                        reader.ReadSignature("s");
                        props.HwAddress = reader.ReadString();
                        changedList?.Add("HwAddress");
                        break;
                    case "PermHwAddress":
                        reader.ReadSignature("s");
                        props.PermHwAddress = reader.ReadString();
                        changedList?.Add("PermHwAddress");
                        break;
                    case "Mode":
                        reader.ReadSignature("u");
                        props.Mode = reader.ReadUInt32();
                        changedList?.Add("Mode");
                        break;
                    case "Bitrate":
                        reader.ReadSignature("u");
                        props.Bitrate = reader.ReadUInt32();
                        changedList?.Add("Bitrate");
                        break;
                    case "AccessPoints":
                        reader.ReadSignature("ao");
                        props.AccessPoints = reader.ReadArrayOfObjectPath();
                        changedList?.Add("AccessPoints");
                        break;
                    case "ActiveAccessPoint":
                        reader.ReadSignature("o");
                        props.ActiveAccessPoint = reader.ReadObjectPath();
                        changedList?.Add("ActiveAccessPoint");
                        break;
                    case "WirelessCapabilities":
                        reader.ReadSignature("u");
                        props.WirelessCapabilities = reader.ReadUInt32();
                        changedList?.Add("WirelessCapabilities");
                        break;
                    case "LastScan":
                        reader.ReadSignature("x");
                        props.LastScan = reader.ReadInt64();
                        changedList?.Add("LastScan");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record DeviceProperties
    {
        internal string Udi { get; set; } = default!;
        internal string Path { get; set; } = default!;
        internal string Interface { get; set; } = default!;
        internal string IpInterface { get; set; } = default!;
        internal string Driver { get; set; } = default!;
        internal string DriverVersion { get; set; } = default!;
        internal string FirmwareVersion { get; set; } = default!;
        internal uint Capabilities { get; set; } = default!;
        internal uint Ip4Address { get; set; } = default!;
        internal uint State { get; set; } = default!;
        internal (uint, uint) StateReason { get; set; } = default!;
        internal ObjectPath ActiveConnection { get; set; } = default!;
        internal ObjectPath Ip4Config { get; set; } = default!;
        internal ObjectPath Dhcp4Config { get; set; } = default!;
        internal ObjectPath Ip6Config { get; set; } = default!;
        internal ObjectPath Dhcp6Config { get; set; } = default!;
        internal bool Managed { get; set; } = default!;
        internal bool Autoconnect { get; set; } = default!;
        internal bool FirmwareMissing { get; set; } = default!;
        internal bool NmPluginMissing { get; set; } = default!;
        internal uint DeviceType { get; set; } = default!;
        internal ObjectPath[] AvailableConnections { get; set; } = default!;
        internal string PhysicalPortId { get; set; } = default!;
        internal uint Mtu { get; set; } = default!;
        internal uint Metered { get; set; } = default!;
        internal Dictionary<string, VariantValue>[] LldpNeighbors { get; set; } = default!;
        internal bool Real { get; set; } = default!;
        internal uint Ip4Connectivity { get; set; } = default!;
        internal uint Ip6Connectivity { get; set; } = default!;
        internal uint InterfaceFlags { get; set; } = default!;
        internal string HwAddress { get; set; } = default!;
        internal ObjectPath[] Ports { get; set; } = default!;
    }
    partial class Device : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.Device";
        internal Device(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal Task ReapplyAsync(Dictionary<string, Dictionary<string, Variant>> connection, ulong versionId, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "a{sa{sv}}tu",
                    member: "Reapply");
                WriteType_aesaesv(ref writer, connection);
                writer.WriteUInt64(versionId);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        internal Task<(Dictionary<string, Dictionary<string, VariantValue>> Connection, ulong VersionId)> GetAppliedConnectionAsync(uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aesaesvt(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "u",
                    member: "GetAppliedConnection");
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        internal Task DisconnectAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "Disconnect");
                return writer.CreateMessage();
            }
        }
        internal Task DeleteAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "Delete");
                return writer.CreateMessage();
            }
        }
        internal ValueTask<IDisposable> WatchStateChangedAsync(Action<Exception?, (uint NewState, uint OldState, uint Reason)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "StateChanged", (Message m, object? s) => ReadMessage_uuu(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
        internal Task SetUdiAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Udi");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetPathAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Path");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetInterfaceAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Interface");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetIpInterfaceAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("IpInterface");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetDriverAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Driver");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetDriverVersionAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("DriverVersion");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetFirmwareVersionAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("FirmwareVersion");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetCapabilitiesAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Capabilities");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetIp4AddressAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Ip4Address");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetStateAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("State");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetStateReasonAsync((uint, uint) value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("StateReason");
                writer.WriteSignature("(uu)");
                WriteType_ruuz(ref writer, value);
                return writer.CreateMessage();
            }
        }
        internal Task SetActiveConnectionAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("ActiveConnection");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetIp4ConfigAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Ip4Config");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetDhcp4ConfigAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Dhcp4Config");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetIp6ConfigAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Ip6Config");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetDhcp6ConfigAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Dhcp6Config");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetManagedAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Managed");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetAutoconnectAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Autoconnect");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetFirmwareMissingAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("FirmwareMissing");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetNmPluginMissingAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("NmPluginMissing");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetDeviceTypeAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("DeviceType");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetAvailableConnectionsAsync(ObjectPath[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("AvailableConnections");
                writer.WriteSignature("ao");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetPhysicalPortIdAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("PhysicalPortId");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetMtuAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Mtu");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetMeteredAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Metered");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetLldpNeighborsAsync(Dictionary<string, Variant>[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("LldpNeighbors");
                writer.WriteSignature("aa{sv}");
                WriteType_aaesv(ref writer, value);
                return writer.CreateMessage();
            }
        }
        internal Task SetRealAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Real");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetIp4ConnectivityAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Ip4Connectivity");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetIp6ConnectivityAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Ip6Connectivity");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetInterfaceFlagsAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("InterfaceFlags");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetHwAddressAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("HwAddress");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetPortsAsync(ObjectPath[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Ports");
                writer.WriteSignature("ao");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task<string> GetUdiAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Udi"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetPathAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Path"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetInterfaceAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Interface"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetIpInterfaceAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "IpInterface"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetDriverAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Driver"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetDriverVersionAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "DriverVersion"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetFirmwareVersionAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "FirmwareVersion"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetCapabilitiesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Capabilities"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetIp4AddressAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Ip4Address"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetStateAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "State"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<(uint, uint)> GetStateReasonAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "StateReason"), (Message m, object? s) => ReadMessage_v_ruuz(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath> GetActiveConnectionAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "ActiveConnection"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath> GetIp4ConfigAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Ip4Config"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath> GetDhcp4ConfigAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Dhcp4Config"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath> GetIp6ConfigAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Ip6Config"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath> GetDhcp6ConfigAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Dhcp6Config"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetManagedAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Managed"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetAutoconnectAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Autoconnect"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetFirmwareMissingAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "FirmwareMissing"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetNmPluginMissingAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "NmPluginMissing"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetDeviceTypeAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "DeviceType"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath[]> GetAvailableConnectionsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "AvailableConnections"), (Message m, object? s) => ReadMessage_v_ao(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetPhysicalPortIdAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "PhysicalPortId"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetMtuAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Mtu"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetMeteredAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Metered"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<Dictionary<string, VariantValue>[]> GetLldpNeighborsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "LldpNeighbors"), (Message m, object? s) => ReadMessage_v_aaesv(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetRealAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Real"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetIp4ConnectivityAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Ip4Connectivity"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetIp6ConnectivityAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Ip6Connectivity"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetInterfaceFlagsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "InterfaceFlags"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetHwAddressAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "HwAddress"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath[]> GetPortsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Ports"), (Message m, object? s) => ReadMessage_v_ao(m, (NetworkManagerObject)s!), this);
        internal Task<DeviceProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), this);
            static DeviceProperties ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        internal ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<DeviceProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<DeviceProperties> ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<DeviceProperties>(ReadProperties(ref reader, changed), changed.ToArray(), ReadInvalidated(ref reader));
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "Udi": invalidated.Add("Udi"); break;
                        case "Path": invalidated.Add("Path"); break;
                        case "Interface": invalidated.Add("Interface"); break;
                        case "IpInterface": invalidated.Add("IpInterface"); break;
                        case "Driver": invalidated.Add("Driver"); break;
                        case "DriverVersion": invalidated.Add("DriverVersion"); break;
                        case "FirmwareVersion": invalidated.Add("FirmwareVersion"); break;
                        case "Capabilities": invalidated.Add("Capabilities"); break;
                        case "Ip4Address": invalidated.Add("Ip4Address"); break;
                        case "State": invalidated.Add("State"); break;
                        case "StateReason": invalidated.Add("StateReason"); break;
                        case "ActiveConnection": invalidated.Add("ActiveConnection"); break;
                        case "Ip4Config": invalidated.Add("Ip4Config"); break;
                        case "Dhcp4Config": invalidated.Add("Dhcp4Config"); break;
                        case "Ip6Config": invalidated.Add("Ip6Config"); break;
                        case "Dhcp6Config": invalidated.Add("Dhcp6Config"); break;
                        case "Managed": invalidated.Add("Managed"); break;
                        case "Autoconnect": invalidated.Add("Autoconnect"); break;
                        case "FirmwareMissing": invalidated.Add("FirmwareMissing"); break;
                        case "NmPluginMissing": invalidated.Add("NmPluginMissing"); break;
                        case "DeviceType": invalidated.Add("DeviceType"); break;
                        case "AvailableConnections": invalidated.Add("AvailableConnections"); break;
                        case "PhysicalPortId": invalidated.Add("PhysicalPortId"); break;
                        case "Mtu": invalidated.Add("Mtu"); break;
                        case "Metered": invalidated.Add("Metered"); break;
                        case "LldpNeighbors": invalidated.Add("LldpNeighbors"); break;
                        case "Real": invalidated.Add("Real"); break;
                        case "Ip4Connectivity": invalidated.Add("Ip4Connectivity"); break;
                        case "Ip6Connectivity": invalidated.Add("Ip6Connectivity"); break;
                        case "InterfaceFlags": invalidated.Add("InterfaceFlags"); break;
                        case "HwAddress": invalidated.Add("HwAddress"); break;
                        case "Ports": invalidated.Add("Ports"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static DeviceProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new DeviceProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "Udi":
                        reader.ReadSignature("s");
                        props.Udi = reader.ReadString();
                        changedList?.Add("Udi");
                        break;
                    case "Path":
                        reader.ReadSignature("s");
                        props.Path = reader.ReadString();
                        changedList?.Add("Path");
                        break;
                    case "Interface":
                        reader.ReadSignature("s");
                        props.Interface = reader.ReadString();
                        changedList?.Add("Interface");
                        break;
                    case "IpInterface":
                        reader.ReadSignature("s");
                        props.IpInterface = reader.ReadString();
                        changedList?.Add("IpInterface");
                        break;
                    case "Driver":
                        reader.ReadSignature("s");
                        props.Driver = reader.ReadString();
                        changedList?.Add("Driver");
                        break;
                    case "DriverVersion":
                        reader.ReadSignature("s");
                        props.DriverVersion = reader.ReadString();
                        changedList?.Add("DriverVersion");
                        break;
                    case "FirmwareVersion":
                        reader.ReadSignature("s");
                        props.FirmwareVersion = reader.ReadString();
                        changedList?.Add("FirmwareVersion");
                        break;
                    case "Capabilities":
                        reader.ReadSignature("u");
                        props.Capabilities = reader.ReadUInt32();
                        changedList?.Add("Capabilities");
                        break;
                    case "Ip4Address":
                        reader.ReadSignature("u");
                        props.Ip4Address = reader.ReadUInt32();
                        changedList?.Add("Ip4Address");
                        break;
                    case "State":
                        reader.ReadSignature("u");
                        props.State = reader.ReadUInt32();
                        changedList?.Add("State");
                        break;
                    case "StateReason":
                        reader.ReadSignature("(uu)");
                        props.StateReason = ReadType_ruuz(ref reader);
                        changedList?.Add("StateReason");
                        break;
                    case "ActiveConnection":
                        reader.ReadSignature("o");
                        props.ActiveConnection = reader.ReadObjectPath();
                        changedList?.Add("ActiveConnection");
                        break;
                    case "Ip4Config":
                        reader.ReadSignature("o");
                        props.Ip4Config = reader.ReadObjectPath();
                        changedList?.Add("Ip4Config");
                        break;
                    case "Dhcp4Config":
                        reader.ReadSignature("o");
                        props.Dhcp4Config = reader.ReadObjectPath();
                        changedList?.Add("Dhcp4Config");
                        break;
                    case "Ip6Config":
                        reader.ReadSignature("o");
                        props.Ip6Config = reader.ReadObjectPath();
                        changedList?.Add("Ip6Config");
                        break;
                    case "Dhcp6Config":
                        reader.ReadSignature("o");
                        props.Dhcp6Config = reader.ReadObjectPath();
                        changedList?.Add("Dhcp6Config");
                        break;
                    case "Managed":
                        reader.ReadSignature("b");
                        props.Managed = reader.ReadBool();
                        changedList?.Add("Managed");
                        break;
                    case "Autoconnect":
                        reader.ReadSignature("b");
                        props.Autoconnect = reader.ReadBool();
                        changedList?.Add("Autoconnect");
                        break;
                    case "FirmwareMissing":
                        reader.ReadSignature("b");
                        props.FirmwareMissing = reader.ReadBool();
                        changedList?.Add("FirmwareMissing");
                        break;
                    case "NmPluginMissing":
                        reader.ReadSignature("b");
                        props.NmPluginMissing = reader.ReadBool();
                        changedList?.Add("NmPluginMissing");
                        break;
                    case "DeviceType":
                        reader.ReadSignature("u");
                        props.DeviceType = reader.ReadUInt32();
                        changedList?.Add("DeviceType");
                        break;
                    case "AvailableConnections":
                        reader.ReadSignature("ao");
                        props.AvailableConnections = reader.ReadArrayOfObjectPath();
                        changedList?.Add("AvailableConnections");
                        break;
                    case "PhysicalPortId":
                        reader.ReadSignature("s");
                        props.PhysicalPortId = reader.ReadString();
                        changedList?.Add("PhysicalPortId");
                        break;
                    case "Mtu":
                        reader.ReadSignature("u");
                        props.Mtu = reader.ReadUInt32();
                        changedList?.Add("Mtu");
                        break;
                    case "Metered":
                        reader.ReadSignature("u");
                        props.Metered = reader.ReadUInt32();
                        changedList?.Add("Metered");
                        break;
                    case "LldpNeighbors":
                        reader.ReadSignature("aa{sv}");
                        props.LldpNeighbors = ReadType_aaesv(ref reader);
                        changedList?.Add("LldpNeighbors");
                        break;
                    case "Real":
                        reader.ReadSignature("b");
                        props.Real = reader.ReadBool();
                        changedList?.Add("Real");
                        break;
                    case "Ip4Connectivity":
                        reader.ReadSignature("u");
                        props.Ip4Connectivity = reader.ReadUInt32();
                        changedList?.Add("Ip4Connectivity");
                        break;
                    case "Ip6Connectivity":
                        reader.ReadSignature("u");
                        props.Ip6Connectivity = reader.ReadUInt32();
                        changedList?.Add("Ip6Connectivity");
                        break;
                    case "InterfaceFlags":
                        reader.ReadSignature("u");
                        props.InterfaceFlags = reader.ReadUInt32();
                        changedList?.Add("InterfaceFlags");
                        break;
                    case "HwAddress":
                        reader.ReadSignature("s");
                        props.HwAddress = reader.ReadString();
                        changedList?.Add("HwAddress");
                        break;
                    case "Ports":
                        reader.ReadSignature("ao");
                        props.Ports = reader.ReadArrayOfObjectPath();
                        changedList?.Add("Ports");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record WifiP2PProperties
    {
        internal string HwAddress { get; set; } = default!;
        internal ObjectPath[] Peers { get; set; } = default!;
    }
    partial class WifiP2P : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.Device.WifiP2P";
        internal WifiP2P(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal Task StartFindAsync(Dictionary<string, Variant> options)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "a{sv}",
                    member: "StartFind");
                writer.WriteDictionary(options);
                return writer.CreateMessage();
            }
        }
        internal Task StopFindAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "StopFind");
                return writer.CreateMessage();
            }
        }
        internal ValueTask<IDisposable> WatchPeerAddedAsync(Action<Exception?, ObjectPath> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "PeerAdded", (Message m, object? s) => ReadMessage_o(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
        internal ValueTask<IDisposable> WatchPeerRemovedAsync(Action<Exception?, ObjectPath> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "PeerRemoved", (Message m, object? s) => ReadMessage_o(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
        internal Task SetHwAddressAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("HwAddress");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetPeersAsync(ObjectPath[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Peers");
                writer.WriteSignature("ao");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task<string> GetHwAddressAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "HwAddress"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath[]> GetPeersAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Peers"), (Message m, object? s) => ReadMessage_v_ao(m, (NetworkManagerObject)s!), this);
        internal Task<WifiP2PProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), this);
            static WifiP2PProperties ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        internal ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<WifiP2PProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<WifiP2PProperties> ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<WifiP2PProperties>(ReadProperties(ref reader, changed), changed.ToArray(), ReadInvalidated(ref reader));
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "HwAddress": invalidated.Add("HwAddress"); break;
                        case "Peers": invalidated.Add("Peers"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static WifiP2PProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new WifiP2PProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "HwAddress":
                        reader.ReadSignature("s");
                        props.HwAddress = reader.ReadString();
                        changedList?.Add("HwAddress");
                        break;
                    case "Peers":
                        reader.ReadSignature("ao");
                        props.Peers = reader.ReadArrayOfObjectPath();
                        changedList?.Add("Peers");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record BridgeProperties
    {
        internal string HwAddress { get; set; } = default!;
        internal bool Carrier { get; set; } = default!;
        internal ObjectPath[] Slaves { get; set; } = default!;
    }
    partial class Bridge : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.Device.Bridge";
        internal Bridge(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal Task SetHwAddressAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("HwAddress");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetCarrierAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Carrier");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetSlavesAsync(ObjectPath[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Slaves");
                writer.WriteSignature("ao");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task<string> GetHwAddressAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "HwAddress"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetCarrierAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Carrier"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<ObjectPath[]> GetSlavesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Slaves"), (Message m, object? s) => ReadMessage_v_ao(m, (NetworkManagerObject)s!), this);
        internal Task<BridgeProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), this);
            static BridgeProperties ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        internal ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<BridgeProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<BridgeProperties> ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<BridgeProperties>(ReadProperties(ref reader, changed), changed.ToArray(), ReadInvalidated(ref reader));
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "HwAddress": invalidated.Add("HwAddress"); break;
                        case "Carrier": invalidated.Add("Carrier"); break;
                        case "Slaves": invalidated.Add("Slaves"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static BridgeProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new BridgeProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "HwAddress":
                        reader.ReadSignature("s");
                        props.HwAddress = reader.ReadString();
                        changedList?.Add("HwAddress");
                        break;
                    case "Carrier":
                        reader.ReadSignature("b");
                        props.Carrier = reader.ReadBool();
                        changedList?.Add("Carrier");
                        break;
                    case "Slaves":
                        reader.ReadSignature("ao");
                        props.Slaves = reader.ReadArrayOfObjectPath();
                        changedList?.Add("Slaves");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    partial class Loopback : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.Device.Loopback";
        internal Loopback(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
    }
    record VethProperties
    {
        internal ObjectPath Peer { get; set; } = default!;
    }
    partial class Veth : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.Device.Veth";
        internal Veth(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal Task SetPeerAsync(ObjectPath value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Peer");
                writer.WriteSignature("o");
                writer.WriteObjectPath(value);
                return writer.CreateMessage();
            }
        }
        internal Task<ObjectPath> GetPeerAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Peer"), (Message m, object? s) => ReadMessage_v_o(m, (NetworkManagerObject)s!), this);
        internal Task<VethProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), this);
            static VethProperties ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        internal ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<VethProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<VethProperties> ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<VethProperties>(ReadProperties(ref reader, changed), changed.ToArray(), ReadInvalidated(ref reader));
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "Peer": invalidated.Add("Peer"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static VethProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new VethProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "Peer":
                        reader.ReadSignature("o");
                        props.Peer = reader.ReadObjectPath();
                        changedList?.Add("Peer");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record WiredProperties
    {
        internal string HwAddress { get; set; } = default!;
        internal string PermHwAddress { get; set; } = default!;
        internal uint Speed { get; set; } = default!;
        internal string[] S390Subchannels { get; set; } = default!;
        internal bool Carrier { get; set; } = default!;
    }
    partial class Wired : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.Device.Wired";
        internal Wired(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal Task SetHwAddressAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("HwAddress");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetPermHwAddressAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("PermHwAddress");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetSpeedAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Speed");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetS390SubchannelsAsync(string[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("S390Subchannels");
                writer.WriteSignature("as");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetCarrierAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Carrier");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task<string> GetHwAddressAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "HwAddress"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetPermHwAddressAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "PermHwAddress"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetSpeedAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Speed"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<string[]> GetS390SubchannelsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "S390Subchannels"), (Message m, object? s) => ReadMessage_v_as(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetCarrierAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Carrier"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<WiredProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), this);
            static WiredProperties ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        internal ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<WiredProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<WiredProperties> ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<WiredProperties>(ReadProperties(ref reader, changed), changed.ToArray(), ReadInvalidated(ref reader));
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "HwAddress": invalidated.Add("HwAddress"); break;
                        case "PermHwAddress": invalidated.Add("PermHwAddress"); break;
                        case "Speed": invalidated.Add("Speed"); break;
                        case "S390Subchannels": invalidated.Add("S390Subchannels"); break;
                        case "Carrier": invalidated.Add("Carrier"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static WiredProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new WiredProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "HwAddress":
                        reader.ReadSignature("s");
                        props.HwAddress = reader.ReadString();
                        changedList?.Add("HwAddress");
                        break;
                    case "PermHwAddress":
                        reader.ReadSignature("s");
                        props.PermHwAddress = reader.ReadString();
                        changedList?.Add("PermHwAddress");
                        break;
                    case "Speed":
                        reader.ReadSignature("u");
                        props.Speed = reader.ReadUInt32();
                        changedList?.Add("Speed");
                        break;
                    case "S390Subchannels":
                        reader.ReadSignature("as");
                        props.S390Subchannels = reader.ReadArrayOfString();
                        changedList?.Add("S390Subchannels");
                        break;
                    case "Carrier":
                        reader.ReadSignature("b");
                        props.Carrier = reader.ReadBool();
                        changedList?.Add("Carrier");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    partial class Ppp : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.Device.Ppp";
        internal Ppp(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
    }
    record DnsManagerProperties
    {
        internal string Mode { get; set; } = default!;
        internal string RcManager { get; set; } = default!;
        internal Dictionary<string, VariantValue>[] Configuration { get; set; } = default!;
    }
    partial class DnsManager : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.DnsManager";
        internal DnsManager(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal Task SetModeAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Mode");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetRcManagerAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("RcManager");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetConfigurationAsync(Dictionary<string, Variant>[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Configuration");
                writer.WriteSignature("aa{sv}");
                WriteType_aaesv(ref writer, value);
                return writer.CreateMessage();
            }
        }
        internal Task<string> GetModeAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Mode"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetRcManagerAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "RcManager"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<Dictionary<string, VariantValue>[]> GetConfigurationAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Configuration"), (Message m, object? s) => ReadMessage_v_aaesv(m, (NetworkManagerObject)s!), this);
        internal Task<DnsManagerProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), this);
            static DnsManagerProperties ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        internal ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<DnsManagerProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<DnsManagerProperties> ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<DnsManagerProperties>(ReadProperties(ref reader, changed), changed.ToArray(), ReadInvalidated(ref reader));
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "Mode": invalidated.Add("Mode"); break;
                        case "RcManager": invalidated.Add("RcManager"); break;
                        case "Configuration": invalidated.Add("Configuration"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static DnsManagerProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new DnsManagerProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "Mode":
                        reader.ReadSignature("s");
                        props.Mode = reader.ReadString();
                        changedList?.Add("Mode");
                        break;
                    case "RcManager":
                        reader.ReadSignature("s");
                        props.RcManager = reader.ReadString();
                        changedList?.Add("RcManager");
                        break;
                    case "Configuration":
                        reader.ReadSignature("aa{sv}");
                        props.Configuration = ReadType_aaesv(ref reader);
                        changedList?.Add("Configuration");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record AccessPointProperties
    {
        internal uint Flags { get; set; } = default!;
        internal uint WpaFlags { get; set; } = default!;
        internal uint RsnFlags { get; set; } = default!;
        internal byte[] Ssid { get; set; } = default!;
        internal uint Frequency { get; set; } = default!;
        internal string HwAddress { get; set; } = default!;
        internal uint Mode { get; set; } = default!;
        internal uint MaxBitrate { get; set; } = default!;
        internal byte Strength { get; set; } = default!;
        internal int LastSeen { get; set; } = default!;
    }
    partial class AccessPoint : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.AccessPoint";
        internal AccessPoint(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal Task SetFlagsAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Flags");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetWpaFlagsAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("WpaFlags");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetRsnFlagsAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("RsnFlags");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetSsidAsync(byte[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Ssid");
                writer.WriteSignature("ay");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetFrequencyAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Frequency");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetHwAddressAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("HwAddress");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetModeAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Mode");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetMaxBitrateAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("MaxBitrate");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetStrengthAsync(byte value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Strength");
                writer.WriteSignature("y");
                writer.WriteByte(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetLastSeenAsync(int value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("LastSeen");
                writer.WriteSignature("i");
                writer.WriteInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task<uint> GetFlagsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Flags"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetWpaFlagsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "WpaFlags"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetRsnFlagsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "RsnFlags"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<byte[]> GetSsidAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Ssid"), (Message m, object? s) => ReadMessage_v_ay(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetFrequencyAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Frequency"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetHwAddressAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "HwAddress"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetModeAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Mode"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetMaxBitrateAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "MaxBitrate"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<byte> GetStrengthAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Strength"), (Message m, object? s) => ReadMessage_v_y(m, (NetworkManagerObject)s!), this);
        internal Task<int> GetLastSeenAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "LastSeen"), (Message m, object? s) => ReadMessage_v_i(m, (NetworkManagerObject)s!), this);
        internal Task<AccessPointProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), this);
            static AccessPointProperties ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        internal ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<AccessPointProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<AccessPointProperties> ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<AccessPointProperties>(ReadProperties(ref reader, changed), changed.ToArray(), ReadInvalidated(ref reader));
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "Flags": invalidated.Add("Flags"); break;
                        case "WpaFlags": invalidated.Add("WpaFlags"); break;
                        case "RsnFlags": invalidated.Add("RsnFlags"); break;
                        case "Ssid": invalidated.Add("Ssid"); break;
                        case "Frequency": invalidated.Add("Frequency"); break;
                        case "HwAddress": invalidated.Add("HwAddress"); break;
                        case "Mode": invalidated.Add("Mode"); break;
                        case "MaxBitrate": invalidated.Add("MaxBitrate"); break;
                        case "Strength": invalidated.Add("Strength"); break;
                        case "LastSeen": invalidated.Add("LastSeen"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static AccessPointProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new AccessPointProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "Flags":
                        reader.ReadSignature("u");
                        props.Flags = reader.ReadUInt32();
                        changedList?.Add("Flags");
                        break;
                    case "WpaFlags":
                        reader.ReadSignature("u");
                        props.WpaFlags = reader.ReadUInt32();
                        changedList?.Add("WpaFlags");
                        break;
                    case "RsnFlags":
                        reader.ReadSignature("u");
                        props.RsnFlags = reader.ReadUInt32();
                        changedList?.Add("RsnFlags");
                        break;
                    case "Ssid":
                        reader.ReadSignature("ay");
                        props.Ssid = reader.ReadArrayOfByte();
                        changedList?.Add("Ssid");
                        break;
                    case "Frequency":
                        reader.ReadSignature("u");
                        props.Frequency = reader.ReadUInt32();
                        changedList?.Add("Frequency");
                        break;
                    case "HwAddress":
                        reader.ReadSignature("s");
                        props.HwAddress = reader.ReadString();
                        changedList?.Add("HwAddress");
                        break;
                    case "Mode":
                        reader.ReadSignature("u");
                        props.Mode = reader.ReadUInt32();
                        changedList?.Add("Mode");
                        break;
                    case "MaxBitrate":
                        reader.ReadSignature("u");
                        props.MaxBitrate = reader.ReadUInt32();
                        changedList?.Add("MaxBitrate");
                        break;
                    case "Strength":
                        reader.ReadSignature("y");
                        props.Strength = reader.ReadByte();
                        changedList?.Add("Strength");
                        break;
                    case "LastSeen":
                        reader.ReadSignature("i");
                        props.LastSeen = reader.ReadInt32();
                        changedList?.Add("LastSeen");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record IP6ConfigProperties
    {
        internal (byte[], uint, byte[])[] Addresses { get; set; } = default!;
        internal Dictionary<string, VariantValue>[] AddressData { get; set; } = default!;
        internal string Gateway { get; set; } = default!;
        internal (byte[], uint, byte[], uint)[] Routes { get; set; } = default!;
        internal Dictionary<string, VariantValue>[] RouteData { get; set; } = default!;
        internal byte[][] Nameservers { get; set; } = default!;
        internal string[] Domains { get; set; } = default!;
        internal string[] Searches { get; set; } = default!;
        internal string[] DnsOptions { get; set; } = default!;
        internal int DnsPriority { get; set; } = default!;
    }
    partial class IP6Config : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.IP6Config";
        internal IP6Config(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal Task SetAddressesAsync((byte[], uint, byte[])[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Addresses");
                writer.WriteSignature("a(ayuay)");
                WriteType_arayuayz(ref writer, value);
                return writer.CreateMessage();
            }
        }
        internal Task SetAddressDataAsync(Dictionary<string, Variant>[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("AddressData");
                writer.WriteSignature("aa{sv}");
                WriteType_aaesv(ref writer, value);
                return writer.CreateMessage();
            }
        }
        internal Task SetGatewayAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Gateway");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetRoutesAsync((byte[], uint, byte[], uint)[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Routes");
                writer.WriteSignature("a(ayuayu)");
                WriteType_arayuayuz(ref writer, value);
                return writer.CreateMessage();
            }
        }
        internal Task SetRouteDataAsync(Dictionary<string, Variant>[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("RouteData");
                writer.WriteSignature("aa{sv}");
                WriteType_aaesv(ref writer, value);
                return writer.CreateMessage();
            }
        }
        internal Task SetNameserversAsync(byte[][] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Nameservers");
                writer.WriteSignature("aay");
                WriteType_aay(ref writer, value);
                return writer.CreateMessage();
            }
        }
        internal Task SetDomainsAsync(string[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Domains");
                writer.WriteSignature("as");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetSearchesAsync(string[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Searches");
                writer.WriteSignature("as");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetDnsOptionsAsync(string[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("DnsOptions");
                writer.WriteSignature("as");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetDnsPriorityAsync(int value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("DnsPriority");
                writer.WriteSignature("i");
                writer.WriteInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task<(byte[], uint, byte[])[]> GetAddressesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Addresses"), (Message m, object? s) => ReadMessage_v_arayuayz(m, (NetworkManagerObject)s!), this);
        internal Task<Dictionary<string, VariantValue>[]> GetAddressDataAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "AddressData"), (Message m, object? s) => ReadMessage_v_aaesv(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetGatewayAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Gateway"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<(byte[], uint, byte[], uint)[]> GetRoutesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Routes"), (Message m, object? s) => ReadMessage_v_arayuayuz(m, (NetworkManagerObject)s!), this);
        internal Task<Dictionary<string, VariantValue>[]> GetRouteDataAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "RouteData"), (Message m, object? s) => ReadMessage_v_aaesv(m, (NetworkManagerObject)s!), this);
        internal Task<byte[][]> GetNameserversAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Nameservers"), (Message m, object? s) => ReadMessage_v_aay(m, (NetworkManagerObject)s!), this);
        internal Task<string[]> GetDomainsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Domains"), (Message m, object? s) => ReadMessage_v_as(m, (NetworkManagerObject)s!), this);
        internal Task<string[]> GetSearchesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Searches"), (Message m, object? s) => ReadMessage_v_as(m, (NetworkManagerObject)s!), this);
        internal Task<string[]> GetDnsOptionsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "DnsOptions"), (Message m, object? s) => ReadMessage_v_as(m, (NetworkManagerObject)s!), this);
        internal Task<int> GetDnsPriorityAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "DnsPriority"), (Message m, object? s) => ReadMessage_v_i(m, (NetworkManagerObject)s!), this);
        internal Task<IP6ConfigProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), this);
            static IP6ConfigProperties ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        internal ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<IP6ConfigProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<IP6ConfigProperties> ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<IP6ConfigProperties>(ReadProperties(ref reader, changed), changed.ToArray(), ReadInvalidated(ref reader));
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "Addresses": invalidated.Add("Addresses"); break;
                        case "AddressData": invalidated.Add("AddressData"); break;
                        case "Gateway": invalidated.Add("Gateway"); break;
                        case "Routes": invalidated.Add("Routes"); break;
                        case "RouteData": invalidated.Add("RouteData"); break;
                        case "Nameservers": invalidated.Add("Nameservers"); break;
                        case "Domains": invalidated.Add("Domains"); break;
                        case "Searches": invalidated.Add("Searches"); break;
                        case "DnsOptions": invalidated.Add("DnsOptions"); break;
                        case "DnsPriority": invalidated.Add("DnsPriority"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static IP6ConfigProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new IP6ConfigProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "Addresses":
                        reader.ReadSignature("a(ayuay)");
                        props.Addresses = ReadType_arayuayz(ref reader);
                        changedList?.Add("Addresses");
                        break;
                    case "AddressData":
                        reader.ReadSignature("aa{sv}");
                        props.AddressData = ReadType_aaesv(ref reader);
                        changedList?.Add("AddressData");
                        break;
                    case "Gateway":
                        reader.ReadSignature("s");
                        props.Gateway = reader.ReadString();
                        changedList?.Add("Gateway");
                        break;
                    case "Routes":
                        reader.ReadSignature("a(ayuayu)");
                        props.Routes = ReadType_arayuayuz(ref reader);
                        changedList?.Add("Routes");
                        break;
                    case "RouteData":
                        reader.ReadSignature("aa{sv}");
                        props.RouteData = ReadType_aaesv(ref reader);
                        changedList?.Add("RouteData");
                        break;
                    case "Nameservers":
                        reader.ReadSignature("aay");
                        props.Nameservers = ReadType_aay(ref reader);
                        changedList?.Add("Nameservers");
                        break;
                    case "Domains":
                        reader.ReadSignature("as");
                        props.Domains = reader.ReadArrayOfString();
                        changedList?.Add("Domains");
                        break;
                    case "Searches":
                        reader.ReadSignature("as");
                        props.Searches = reader.ReadArrayOfString();
                        changedList?.Add("Searches");
                        break;
                    case "DnsOptions":
                        reader.ReadSignature("as");
                        props.DnsOptions = reader.ReadArrayOfString();
                        changedList?.Add("DnsOptions");
                        break;
                    case "DnsPriority":
                        reader.ReadSignature("i");
                        props.DnsPriority = reader.ReadInt32();
                        changedList?.Add("DnsPriority");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record SettingsProperties
    {
        internal ObjectPath[] Connections { get; set; } = default!;
        internal string Hostname { get; set; } = default!;
        internal bool CanModify { get; set; } = default!;
    }
    partial class Settings : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.Settings";
        internal Settings(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal Task<ObjectPath[]> ListConnectionsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_ao(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "ListConnections");
                return writer.CreateMessage();
            }
        }
        internal Task<ObjectPath> GetConnectionByUuidAsync(string uuid)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_o(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "GetConnectionByUuid");
                writer.WriteString(uuid);
                return writer.CreateMessage();
            }
        }
        internal Task<ObjectPath> AddConnectionAsync(Dictionary<string, Dictionary<string, Variant>> connection)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_o(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "a{sa{sv}}",
                    member: "AddConnection");
                WriteType_aesaesv(ref writer, connection);
                return writer.CreateMessage();
            }
        }
        internal Task<ObjectPath> AddConnectionUnsavedAsync(Dictionary<string, Dictionary<string, Variant>> connection)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_o(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "a{sa{sv}}",
                    member: "AddConnectionUnsaved");
                WriteType_aesaesv(ref writer, connection);
                return writer.CreateMessage();
            }
        }
        internal Task<(ObjectPath Path, Dictionary<string, VariantValue> Result)> AddConnection2Async(Dictionary<string, Dictionary<string, Variant>> settings, uint flags, Dictionary<string, Variant> args)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_oaesv(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "a{sa{sv}}ua{sv}",
                    member: "AddConnection2");
                WriteType_aesaesv(ref writer, settings);
                writer.WriteUInt32(flags);
                writer.WriteDictionary(args);
                return writer.CreateMessage();
            }
        }
        internal Task<(bool Status, string[] Failures)> LoadConnectionsAsync(string[] filenames)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_bas(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "as",
                    member: "LoadConnections");
                writer.WriteArray(filenames);
                return writer.CreateMessage();
            }
        }
        internal Task<bool> ReloadConnectionsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "ReloadConnections");
                return writer.CreateMessage();
            }
        }
        internal Task SaveHostnameAsync(string hostname)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "SaveHostname");
                writer.WriteString(hostname);
                return writer.CreateMessage();
            }
        }
        internal ValueTask<IDisposable> WatchNewConnectionAsync(Action<Exception?, ObjectPath> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "NewConnection", (Message m, object? s) => ReadMessage_o(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
        internal ValueTask<IDisposable> WatchConnectionRemovedAsync(Action<Exception?, ObjectPath> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "ConnectionRemoved", (Message m, object? s) => ReadMessage_o(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
        internal Task SetConnectionsAsync(ObjectPath[] value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Connections");
                writer.WriteSignature("ao");
                writer.WriteArray(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetHostnameAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Hostname");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetCanModifyAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("CanModify");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task<ObjectPath[]> GetConnectionsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Connections"), (Message m, object? s) => ReadMessage_v_ao(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetHostnameAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Hostname"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<bool> GetCanModifyAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "CanModify"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<SettingsProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), this);
            static SettingsProperties ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        internal ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<SettingsProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<SettingsProperties> ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<SettingsProperties>(ReadProperties(ref reader, changed), changed.ToArray(), ReadInvalidated(ref reader));
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "Connections": invalidated.Add("Connections"); break;
                        case "Hostname": invalidated.Add("Hostname"); break;
                        case "CanModify": invalidated.Add("CanModify"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static SettingsProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new SettingsProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "Connections":
                        reader.ReadSignature("ao");
                        props.Connections = reader.ReadArrayOfObjectPath();
                        changedList?.Add("Connections");
                        break;
                    case "Hostname":
                        reader.ReadSignature("s");
                        props.Hostname = reader.ReadString();
                        changedList?.Add("Hostname");
                        break;
                    case "CanModify":
                        reader.ReadSignature("b");
                        props.CanModify = reader.ReadBool();
                        changedList?.Add("CanModify");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record ConnectionProperties
    {
        internal bool Unsaved { get; set; } = default!;
        internal uint Flags { get; set; } = default!;
        internal string Filename { get; set; } = default!;
    }
    partial class Connection : NetworkManagerObject
    {
        private const string __Interface = "org.freedesktop.NetworkManager.Settings.Connection";
        internal Connection(NetworkManagerService service, ObjectPath path) : base(service, path)
        { }
        internal Task UpdateAsync(Dictionary<string, Dictionary<string, Variant>> properties)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "a{sa{sv}}",
                    member: "Update");
                WriteType_aesaesv(ref writer, properties);
                return writer.CreateMessage();
            }
        }
        internal Task UpdateUnsavedAsync(Dictionary<string, Dictionary<string, Variant>> properties)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "a{sa{sv}}",
                    member: "UpdateUnsaved");
                WriteType_aesaesv(ref writer, properties);
                return writer.CreateMessage();
            }
        }
        internal Task DeleteAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "Delete");
                return writer.CreateMessage();
            }
        }
        internal Task<Dictionary<string, Dictionary<string, VariantValue>>> GetSettingsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aesaesv(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "GetSettings");
                return writer.CreateMessage();
            }
        }
        internal Task<Dictionary<string, Dictionary<string, VariantValue>>> GetSecretsAsync(string settingName)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aesaesv(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "GetSecrets");
                writer.WriteString(settingName);
                return writer.CreateMessage();
            }
        }
        internal Task ClearSecretsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "ClearSecrets");
                return writer.CreateMessage();
            }
        }
        internal Task SaveAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "Save");
                return writer.CreateMessage();
            }
        }
        internal Task<Dictionary<string, VariantValue>> Update2Async(Dictionary<string, Dictionary<string, Variant>> settings, uint flags, Dictionary<string, Variant> args)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aesv(m, (NetworkManagerObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "a{sa{sv}}ua{sv}",
                    member: "Update2");
                WriteType_aesaesv(ref writer, settings);
                writer.WriteUInt32(flags);
                writer.WriteDictionary(args);
                return writer.CreateMessage();
            }
        }
        internal ValueTask<IDisposable> WatchUpdatedAsync(Action<Exception?> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "Updated", handler, emitOnCapturedContext, flags);
        internal ValueTask<IDisposable> WatchRemovedAsync(Action<Exception?> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "Removed", handler, emitOnCapturedContext, flags);
        internal Task SetUnsavedAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Unsaved");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetFlagsAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Flags");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        internal Task SetFilenameAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("Filename");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        internal Task<bool> GetUnsavedAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Unsaved"), (Message m, object? s) => ReadMessage_v_b(m, (NetworkManagerObject)s!), this);
        internal Task<uint> GetFlagsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Flags"), (Message m, object? s) => ReadMessage_v_u(m, (NetworkManagerObject)s!), this);
        internal Task<string> GetFilenameAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Filename"), (Message m, object? s) => ReadMessage_v_s(m, (NetworkManagerObject)s!), this);
        internal Task<ConnectionProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), this);
            static ConnectionProperties ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        internal ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<ConnectionProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (NetworkManagerObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<ConnectionProperties> ReadMessage(Message message, NetworkManagerObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new(), invalidated = new();
                return new PropertyChanges<ConnectionProperties>(ReadProperties(ref reader, changed), changed.ToArray(), ReadInvalidated(ref reader));
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "Unsaved": invalidated.Add("Unsaved"); break;
                        case "Flags": invalidated.Add("Flags"); break;
                        case "Filename": invalidated.Add("Filename"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static ConnectionProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new ConnectionProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "Unsaved":
                        reader.ReadSignature("b");
                        props.Unsaved = reader.ReadBool();
                        changedList?.Add("Unsaved");
                        break;
                    case "Flags":
                        reader.ReadSignature("u");
                        props.Flags = reader.ReadUInt32();
                        changedList?.Add("Flags");
                        break;
                    case "Filename":
                        reader.ReadSignature("s");
                        props.Filename = reader.ReadString();
                        changedList?.Add("Filename");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    partial class NetworkManagerService
    {
        internal Tmds.DBus.Protocol.Connection Connection { get; }
        internal string Destination { get; }
        internal NetworkManagerService(Tmds.DBus.Protocol.Connection connection, string destination)
            => (Connection, Destination) = (connection, destination);
        internal ObjectManager CreateObjectManager(string path) => new ObjectManager(this, path);
        internal NetworkManager CreateNetworkManager(string path) => new NetworkManager(this, path);
        internal IP4Config CreateIP4Config(string path) => new IP4Config(this, path);
        internal Active CreateActive(string path) => new Active(this, path);
        internal AgentManager CreateAgentManager(string path) => new AgentManager(this, path);
        internal Statistics CreateStatistics(string path) => new Statistics(this, path);
        internal Wireless CreateWireless(string path) => new Wireless(this, path);
        internal Device CreateDevice(string path) => new Device(this, path);
        internal WifiP2P CreateWifiP2P(string path) => new WifiP2P(this, path);
        internal Bridge CreateBridge(string path) => new Bridge(this, path);
        internal Loopback CreateLoopback(string path) => new Loopback(this, path);
        internal Veth CreateVeth(string path) => new Veth(this, path);
        internal Wired CreateWired(string path) => new Wired(this, path);
        internal Ppp CreatePpp(string path) => new Ppp(this, path);
        internal DnsManager CreateDnsManager(string path) => new DnsManager(this, path);
        internal AccessPoint CreateAccessPoint(string path) => new AccessPoint(this, path);
        internal IP6Config CreateIP6Config(string path) => new IP6Config(this, path);
        internal Settings CreateSettings(string path) => new Settings(this, path);
        internal Connection CreateConnection(string path) => new Connection(this, path);
    }
    class NetworkManagerObject
    {
        internal NetworkManagerService Service { get; }
        internal ObjectPath Path { get; }
        protected Tmds.DBus.Protocol.Connection Connection => Service.Connection;
        protected NetworkManagerObject(NetworkManagerService service, ObjectPath path)
            => (Service, Path) = (service, path);
        protected MessageBuffer CreateGetPropertyMessage(string @interface, string property)
        {
            var writer = this.Connection.GetMessageWriter();
            writer.WriteMethodCallHeader(
                destination: Service.Destination,
                path: Path,
                @interface: "org.freedesktop.DBus.Properties",
                signature: "ss",
                member: "Get");
            writer.WriteString(@interface);
            writer.WriteString(property);
            return writer.CreateMessage();
        }
        protected MessageBuffer CreateGetAllPropertiesMessage(string @interface)
        {
            var writer = this.Connection.GetMessageWriter();
            writer.WriteMethodCallHeader(
                destination: Service.Destination,
                path: Path,
                @interface: "org.freedesktop.DBus.Properties",
                signature: "s",
                member: "GetAll");
            writer.WriteString(@interface);
            return writer.CreateMessage();
        }
        protected ValueTask<IDisposable> WatchPropertiesChangedAsync<TProperties>(string @interface, MessageValueReader<PropertyChanges<TProperties>> reader, Action<Exception?, PropertyChanges<TProperties>> handler, bool emitOnCapturedContext, ObserverFlags flags)
        {
            var rule = new MatchRule
            {
                Type = MessageType.Signal,
                Sender = Service.Destination,
                Path = Path,
                Interface = "org.freedesktop.DBus.Properties",
                Member = "PropertiesChanged",
                Arg0 = @interface
            };
            return this.Connection.AddMatchAsync(rule, reader,
                                                    (Exception? ex, PropertyChanges<TProperties> changes, object? rs, object? hs) => ((Action<Exception?, PropertyChanges<TProperties>>)hs!).Invoke(ex, changes),
                                                    this, handler, emitOnCapturedContext, flags);
        }
        internal ValueTask<IDisposable> WatchSignalAsync<TArg>(string sender, string @interface, ObjectPath path, string signal, MessageValueReader<TArg> reader, Action<Exception?, TArg> handler, bool emitOnCapturedContext, ObserverFlags flags)
        {
            var rule = new MatchRule
            {
                Type = MessageType.Signal,
                Sender = sender,
                Path = path,
                Member = signal,
                Interface = @interface
            };
            return this.Connection.AddMatchAsync(rule, reader,
                                                    (Exception? ex, TArg arg, object? rs, object? hs) => ((Action<Exception?, TArg>)hs!).Invoke(ex, arg),
                                                    this, handler, emitOnCapturedContext, flags);
        }
        internal ValueTask<IDisposable> WatchSignalAsync(string sender, string @interface, ObjectPath path, string signal, Action<Exception?> handler, bool emitOnCapturedContext, ObserverFlags flags)
        {
            var rule = new MatchRule
            {
                Type = MessageType.Signal,
                Sender = sender,
                Path = path,
                Member = signal,
                Interface = @interface
            };
            return this.Connection.AddMatchAsync<object>(rule, (Message message, object? state) => null!,
                                                            (Exception? ex, object v, object? rs, object? hs) => ((Action<Exception?>)hs!).Invoke(ex), this, handler, emitOnCapturedContext, flags);
        }
        protected static Dictionary<ObjectPath, Dictionary<string, Dictionary<string, VariantValue>>> ReadMessage_aeoaesaesv(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            return ReadType_aeoaesaesv(ref reader);
        }
        protected static (ObjectPath, Dictionary<string, Dictionary<string, VariantValue>>) ReadMessage_oaesaesv(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadObjectPath();
            var arg1 = ReadType_aesaesv(ref reader);
            return (arg0, arg1);
        }
        protected static (ObjectPath, string[]) ReadMessage_oas(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadObjectPath();
            var arg1 = reader.ReadArrayOfString();
            return (arg0, arg1);
        }
        protected static ObjectPath[] ReadMessage_ao(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadArrayOfObjectPath();
        }
        protected static ObjectPath ReadMessage_o(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadObjectPath();
        }
        protected static (ObjectPath, ObjectPath) ReadMessage_oo(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadObjectPath();
            var arg1 = reader.ReadObjectPath();
            return (arg0, arg1);
        }
        protected static (ObjectPath, ObjectPath, Dictionary<string, VariantValue>) ReadMessage_ooaesv(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadObjectPath();
            var arg1 = reader.ReadObjectPath();
            var arg2 = reader.ReadDictionaryOfStringToVariantValue();
            return (arg0, arg1, arg2);
        }
        protected static Dictionary<string, string> ReadMessage_aess(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            return ReadType_aess(ref reader);
        }
        protected static (string, string) ReadMessage_ss(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadString();
            var arg1 = reader.ReadString();
            return (arg0, arg1);
        }
        protected static uint ReadMessage_u(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadUInt32();
        }
        protected static Dictionary<string, uint> ReadMessage_aesu(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            return ReadType_aesu(ref reader);
        }
        protected static ObjectPath[] ReadMessage_v_ao(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("ao");
            return reader.ReadArrayOfObjectPath();
        }
        protected static bool ReadMessage_v_b(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("b");
            return reader.ReadBool();
        }
        protected static uint ReadMessage_v_u(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("u");
            return reader.ReadUInt32();
        }
        protected static ObjectPath ReadMessage_v_o(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("o");
            return reader.ReadObjectPath();
        }
        protected static string ReadMessage_v_s(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("s");
            return reader.ReadString();
        }
        protected static uint[] ReadMessage_v_au(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("au");
            return reader.ReadArrayOfUInt32();
        }
        protected static Dictionary<string, VariantValue> ReadMessage_v_aesv(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("a{sv}");
            return reader.ReadDictionaryOfStringToVariantValue();
        }
        protected static uint[][] ReadMessage_v_aau(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("aau");
            return ReadType_aau(ref reader);
        }
        protected static Dictionary<string, VariantValue>[] ReadMessage_v_aaesv(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("aa{sv}");
            return ReadType_aaesv(ref reader);
        }
        protected static string[] ReadMessage_v_as(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("as");
            return reader.ReadArrayOfString();
        }
        protected static int ReadMessage_v_i(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("i");
            return reader.ReadInt32();
        }
        protected static (uint, uint) ReadMessage_uu(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadUInt32();
            var arg1 = reader.ReadUInt32();
            return (arg0, arg1);
        }
        protected static ulong ReadMessage_v_t(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("t");
            return reader.ReadUInt64();
        }
        protected static long ReadMessage_v_x(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("x");
            return reader.ReadInt64();
        }
        protected static (Dictionary<string, Dictionary<string, VariantValue>>, ulong) ReadMessage_aesaesvt(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = ReadType_aesaesv(ref reader);
            var arg1 = reader.ReadUInt64();
            return (arg0, arg1);
        }
        protected static (uint, uint, uint) ReadMessage_uuu(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadUInt32();
            var arg1 = reader.ReadUInt32();
            var arg2 = reader.ReadUInt32();
            return (arg0, arg1, arg2);
        }
        protected static (uint, uint) ReadMessage_v_ruuz(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("(uu)");
            return ReadType_ruuz(ref reader);
        }
        protected static byte[] ReadMessage_v_ay(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("ay");
            return reader.ReadArrayOfByte();
        }
        protected static byte ReadMessage_v_y(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("y");
            return reader.ReadByte();
        }
        protected static (byte[], uint, byte[])[] ReadMessage_v_arayuayz(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("a(ayuay)");
            return ReadType_arayuayz(ref reader);
        }
        protected static (byte[], uint, byte[], uint)[] ReadMessage_v_arayuayuz(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("a(ayuayu)");
            return ReadType_arayuayuz(ref reader);
        }
        protected static byte[][] ReadMessage_v_aay(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("aay");
            return ReadType_aay(ref reader);
        }
        protected static (ObjectPath, Dictionary<string, VariantValue>) ReadMessage_oaesv(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadObjectPath();
            var arg1 = reader.ReadDictionaryOfStringToVariantValue();
            return (arg0, arg1);
        }
        protected static (bool, string[]) ReadMessage_bas(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadBool();
            var arg1 = reader.ReadArrayOfString();
            return (arg0, arg1);
        }
        protected static bool ReadMessage_b(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadBool();
        }
        protected static Dictionary<string, Dictionary<string, VariantValue>> ReadMessage_aesaesv(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            return ReadType_aesaesv(ref reader);
        }
        protected static Dictionary<string, VariantValue> ReadMessage_aesv(Message message, NetworkManagerObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadDictionaryOfStringToVariantValue();
        }
        protected static uint[][] ReadType_aau(ref Reader reader)
        {
            List<uint[]> list = new();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Array);
            while (reader.HasNext(arrayEnd))
            {
                list.Add(reader.ReadArrayOfUInt32());
            }
            return list.ToArray();
        }
        protected static Dictionary<string, VariantValue>[] ReadType_aaesv(ref Reader reader)
        {
            List<Dictionary<string, VariantValue>> list = new();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Array);
            while (reader.HasNext(arrayEnd))
            {
                list.Add(reader.ReadDictionaryOfStringToVariantValue());
            }
            return list.ToArray();
        }
        protected static (uint, uint) ReadType_ruuz(ref Reader reader)
        {
            return (reader.ReadUInt32(), reader.ReadUInt32());
        }
        protected static (byte[], uint, byte[])[] ReadType_arayuayz(ref Reader reader)
        {
            List<(byte[], uint, byte[])> list = new();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                list.Add(ReadType_rayuayz(ref reader));
            }
            return list.ToArray();
        }
        protected static (byte[], uint, byte[]) ReadType_rayuayz(ref Reader reader)
        {
            return (reader.ReadArrayOfByte(), reader.ReadUInt32(), reader.ReadArrayOfByte());
        }
        protected static (byte[], uint, byte[], uint)[] ReadType_arayuayuz(ref Reader reader)
        {
            List<(byte[], uint, byte[], uint)> list = new();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                list.Add(ReadType_rayuayuz(ref reader));
            }
            return list.ToArray();
        }
        protected static (byte[], uint, byte[], uint) ReadType_rayuayuz(ref Reader reader)
        {
            return (reader.ReadArrayOfByte(), reader.ReadUInt32(), reader.ReadArrayOfByte(), reader.ReadUInt32());
        }
        protected static byte[][] ReadType_aay(ref Reader reader)
        {
            List<byte[]> list = new();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Array);
            while (reader.HasNext(arrayEnd))
            {
                list.Add(reader.ReadArrayOfByte());
            }
            return list.ToArray();
        }
        protected static Dictionary<ObjectPath, Dictionary<string, Dictionary<string, VariantValue>>> ReadType_aeoaesaesv(ref Reader reader)
        {
            Dictionary<ObjectPath, Dictionary<string, Dictionary<string, VariantValue>>> dictionary = new();
            ArrayEnd dictEnd = reader.ReadDictionaryStart();
            while (reader.HasNext(dictEnd))
            {
                var key = reader.ReadObjectPath();
                var value = ReadType_aesaesv(ref reader);
                dictionary[key] = value;
            }
            return dictionary;
        }
        protected static Dictionary<string, Dictionary<string, VariantValue>> ReadType_aesaesv(ref Reader reader)
        {
            Dictionary<string, Dictionary<string, VariantValue>> dictionary = new();
            ArrayEnd dictEnd = reader.ReadDictionaryStart();
            while (reader.HasNext(dictEnd))
            {
                var key = reader.ReadString();
                var value = reader.ReadDictionaryOfStringToVariantValue();
                dictionary[key] = value;
            }
            return dictionary;
        }
        protected static Dictionary<string, string> ReadType_aess(ref Reader reader)
        {
            Dictionary<string, string> dictionary = new();
            ArrayEnd dictEnd = reader.ReadDictionaryStart();
            while (reader.HasNext(dictEnd))
            {
                var key = reader.ReadString();
                var value = reader.ReadString();
                dictionary[key] = value;
            }
            return dictionary;
        }
        protected static Dictionary<string, uint> ReadType_aesu(ref Reader reader)
        {
            Dictionary<string, uint> dictionary = new();
            ArrayEnd dictEnd = reader.ReadDictionaryStart();
            while (reader.HasNext(dictEnd))
            {
                var key = reader.ReadString();
                var value = reader.ReadUInt32();
                dictionary[key] = value;
            }
            return dictionary;
        }
        protected static void WriteType_aesaesv(ref MessageWriter writer, Dictionary<string, Dictionary<string, Variant>> value)
        {
            ArrayStart arrayStart = writer.WriteDictionaryStart();
            foreach (var item in value)
            {
                writer.WriteDictionaryEntryStart();
                writer.WriteString(item.Key);
                writer.WriteDictionary(item.Value);
            }
            writer.WriteDictionaryEnd(arrayStart);
        }
        protected static void WriteType_aau(ref MessageWriter writer, uint[][] value)
        {
            ArrayStart arrayStart = writer.WriteArrayStart(DBusType.Array);
            foreach (var item in value)
            {
                writer.WriteArray(item);
            }
            writer.WriteArrayEnd(arrayStart);
        }
        protected static void WriteType_aaesv(ref MessageWriter writer, Dictionary<string, Variant>[] value)
        {
            ArrayStart arrayStart = writer.WriteArrayStart(DBusType.Array);
            foreach (var item in value)
            {
                writer.WriteDictionary(item);
            }
            writer.WriteArrayEnd(arrayStart);
        }
        protected static void WriteType_ruuz(ref MessageWriter writer, (uint, uint) value)
        {
            writer.WriteStructureStart();
            writer.WriteUInt32(value.Item1);
            writer.WriteUInt32(value.Item2);
        }
        protected static void WriteType_arayuayz(ref MessageWriter writer, (byte[], uint, byte[])[] value)
        {
            ArrayStart arrayStart = writer.WriteArrayStart(DBusType.Struct);
            foreach (var item in value)
            {
                WriteType_rayuayz(ref writer, item);
            }
            writer.WriteArrayEnd(arrayStart);
        }
        protected static void WriteType_rayuayz(ref MessageWriter writer, (byte[], uint, byte[]) value)
        {
            writer.WriteStructureStart();
            writer.WriteArray(value.Item1);
            writer.WriteUInt32(value.Item2);
            writer.WriteArray(value.Item3);
        }
        protected static void WriteType_arayuayuz(ref MessageWriter writer, (byte[], uint, byte[], uint)[] value)
        {
            ArrayStart arrayStart = writer.WriteArrayStart(DBusType.Struct);
            foreach (var item in value)
            {
                WriteType_rayuayuz(ref writer, item);
            }
            writer.WriteArrayEnd(arrayStart);
        }
        protected static void WriteType_rayuayuz(ref MessageWriter writer, (byte[], uint, byte[], uint) value)
        {
            writer.WriteStructureStart();
            writer.WriteArray(value.Item1);
            writer.WriteUInt32(value.Item2);
            writer.WriteArray(value.Item3);
            writer.WriteUInt32(value.Item4);
        }
        protected static void WriteType_aay(ref MessageWriter writer, byte[][] value)
        {
            ArrayStart arrayStart = writer.WriteArrayStart(DBusType.Array);
            foreach (var item in value)
            {
                writer.WriteArray(item);
            }
            writer.WriteArrayEnd(arrayStart);
        }
    }
    class PropertyChanges<TProperties>
    {
        internal PropertyChanges(TProperties properties, string[] invalidated, string[] changed)
        	=> (Properties, Invalidated, Changed) = (properties, invalidated, changed);
        internal TProperties Properties { get; }
        internal string[] Invalidated { get; }
        internal string[] Changed { get; }
        internal bool HasChanged(string property) => Array.IndexOf(Changed, property) != -1;
        internal bool IsInvalidated(string property) => Array.IndexOf(Invalidated, property) != -1;
    }
}
