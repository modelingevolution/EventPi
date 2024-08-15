using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

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