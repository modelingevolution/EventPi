using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

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