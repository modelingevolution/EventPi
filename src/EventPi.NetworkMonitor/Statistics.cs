using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

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