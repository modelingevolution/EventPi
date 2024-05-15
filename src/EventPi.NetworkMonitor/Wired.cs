using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

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