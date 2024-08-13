using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

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