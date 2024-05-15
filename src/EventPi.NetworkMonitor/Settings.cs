using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

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