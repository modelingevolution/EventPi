using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

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