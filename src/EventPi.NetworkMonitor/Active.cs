using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

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