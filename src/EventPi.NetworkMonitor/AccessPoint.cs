using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

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