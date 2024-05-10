using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

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