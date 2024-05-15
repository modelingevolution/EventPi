using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

class NetworkManagerObject
{
    internal NetworkManagerService Service { get; }
    internal ObjectPath Path { get; }
    protected Tmds.DBus.Protocol.Connection Connection => Service.Connection;
    protected NetworkManagerObject(NetworkManagerService service, ObjectPath path)
        => (Service, Path) = (service, path);
    protected MessageBuffer CreateGetPropertyMessage(string @interface, string property)
    {
        var writer = this.Connection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: Service.Destination,
            path: Path,
            @interface: "org.freedesktop.DBus.Properties",
            signature: "ss",
            member: "Get");
        writer.WriteString(@interface);
        writer.WriteString(property);
        return writer.CreateMessage();
    }
    protected MessageBuffer CreateGetAllPropertiesMessage(string @interface)
    {
        var writer = this.Connection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: Service.Destination,
            path: Path,
            @interface: "org.freedesktop.DBus.Properties",
            signature: "s",
            member: "GetAll");
        writer.WriteString(@interface);
        return writer.CreateMessage();
    }
    protected ValueTask<IDisposable> WatchPropertiesChangedAsync<TProperties>(string @interface, MessageValueReader<PropertyChanges<TProperties>> reader, Action<Exception?, PropertyChanges<TProperties>> handler, bool emitOnCapturedContext, ObserverFlags flags)
    {
        var rule = new MatchRule
        {
            Type = MessageType.Signal,
            Sender = Service.Destination,
            Path = Path,
            Interface = "org.freedesktop.DBus.Properties",
            Member = "PropertiesChanged",
            Arg0 = @interface
        };
        return this.Connection.AddMatchAsync(rule, reader,
            (Exception? ex, PropertyChanges<TProperties> changes, object? rs, object? hs) => ((Action<Exception?, PropertyChanges<TProperties>>)hs!).Invoke(ex, changes),
            this, handler, emitOnCapturedContext, flags);
    }
    internal ValueTask<IDisposable> WatchSignalAsync<TArg>(string sender, string @interface, ObjectPath path, string signal, MessageValueReader<TArg> reader, Action<Exception?, TArg> handler, bool emitOnCapturedContext, ObserverFlags flags)
    {
        var rule = new MatchRule
        {
            Type = MessageType.Signal,
            Sender = sender,
            Path = path,
            Member = signal,
            Interface = @interface
        };
        return this.Connection.AddMatchAsync(rule, reader,
            (Exception? ex, TArg arg, object? rs, object? hs) => ((Action<Exception?, TArg>)hs!).Invoke(ex, arg),
            this, handler, emitOnCapturedContext, flags);
    }
    internal ValueTask<IDisposable> WatchSignalAsync(string sender, string @interface, ObjectPath path, string signal, Action<Exception?> handler, bool emitOnCapturedContext, ObserverFlags flags)
    {
        var rule = new MatchRule
        {
            Type = MessageType.Signal,
            Sender = sender,
            Path = path,
            Member = signal,
            Interface = @interface
        };
        return this.Connection.AddMatchAsync<object>(rule, (Message message, object? state) => null!,
            (Exception? ex, object v, object? rs, object? hs) => ((Action<Exception?>)hs!).Invoke(ex), this, handler, emitOnCapturedContext, flags);
    }
    protected static Dictionary<ObjectPath, Dictionary<string, Dictionary<string, VariantValue>>> ReadMessage_aeoaesaesv(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        return ReadType_aeoaesaesv(ref reader);
    }
    protected static (ObjectPath, Dictionary<string, Dictionary<string, VariantValue>>) ReadMessage_oaesaesv(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        var arg0 = reader.ReadObjectPath();
        var arg1 = ReadType_aesaesv(ref reader);
        return (arg0, arg1);
    }
    protected static (ObjectPath, string[]) ReadMessage_oas(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        var arg0 = reader.ReadObjectPath();
        var arg1 = reader.ReadArrayOfString();
        return (arg0, arg1);
    }
    protected static ObjectPath[] ReadMessage_ao(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        return reader.ReadArrayOfObjectPath();
    }
    protected static ObjectPath ReadMessage_o(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        return reader.ReadObjectPath();
    }
    protected static (ObjectPath, ObjectPath) ReadMessage_oo(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        var arg0 = reader.ReadObjectPath();
        var arg1 = reader.ReadObjectPath();
        return (arg0, arg1);
    }
    protected static (ObjectPath, ObjectPath, Dictionary<string, VariantValue>) ReadMessage_ooaesv(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        var arg0 = reader.ReadObjectPath();
        var arg1 = reader.ReadObjectPath();
        var arg2 = reader.ReadDictionaryOfStringToVariantValue();
        return (arg0, arg1, arg2);
    }
    protected static Dictionary<string, string> ReadMessage_aess(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        return ReadType_aess(ref reader);
    }
    protected static (string, string) ReadMessage_ss(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        var arg0 = reader.ReadString();
        var arg1 = reader.ReadString();
        return (arg0, arg1);
    }
    protected static uint ReadMessage_u(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        return reader.ReadUInt32();
    }
    protected static Dictionary<string, uint> ReadMessage_aesu(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        return ReadType_aesu(ref reader);
    }
    protected static ObjectPath[] ReadMessage_v_ao(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("ao");
        return reader.ReadArrayOfObjectPath();
    }
    protected static bool ReadMessage_v_b(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("b");
        return reader.ReadBool();
    }
    protected static uint ReadMessage_v_u(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("u");
        return reader.ReadUInt32();
    }
    protected static ObjectPath ReadMessage_v_o(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("o");
        return reader.ReadObjectPath();
    }
    protected static string ReadMessage_v_s(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("s");
        return reader.ReadString();
    }
    protected static uint[] ReadMessage_v_au(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("au");
        return reader.ReadArrayOfUInt32();
    }
    protected static Dictionary<string, VariantValue> ReadMessage_v_aesv(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("a{sv}");
        return reader.ReadDictionaryOfStringToVariantValue();
    }
    protected static uint[][] ReadMessage_v_aau(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("aau");
        return ReadType_aau(ref reader);
    }
    protected static Dictionary<string, VariantValue>[] ReadMessage_v_aaesv(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("aa{sv}");
        return ReadType_aaesv(ref reader);
    }
    protected static string[] ReadMessage_v_as(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("as");
        return reader.ReadArrayOfString();
    }
    protected static int ReadMessage_v_i(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("i");
        return reader.ReadInt32();
    }
    protected static (uint, uint) ReadMessage_uu(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        var arg0 = reader.ReadUInt32();
        var arg1 = reader.ReadUInt32();
        return (arg0, arg1);
    }
    protected static ulong ReadMessage_v_t(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("t");
        return reader.ReadUInt64();
    }
    protected static long ReadMessage_v_x(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("x");
        return reader.ReadInt64();
    }
    protected static (Dictionary<string, Dictionary<string, VariantValue>>, ulong) ReadMessage_aesaesvt(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        var arg0 = ReadType_aesaesv(ref reader);
        var arg1 = reader.ReadUInt64();
        return (arg0, arg1);
    }
    protected static (uint, uint, uint) ReadMessage_uuu(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        var arg0 = reader.ReadUInt32();
        var arg1 = reader.ReadUInt32();
        var arg2 = reader.ReadUInt32();
        return (arg0, arg1, arg2);
    }
    protected static (uint, uint) ReadMessage_v_ruuz(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("(uu)");
        return ReadType_ruuz(ref reader);
    }
    protected static byte[] ReadMessage_v_ay(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("ay");
        return reader.ReadArrayOfByte();
    }
    protected static byte ReadMessage_v_y(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("y");
        return reader.ReadByte();
    }
    protected static (byte[], uint, byte[])[] ReadMessage_v_arayuayz(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("a(ayuay)");
        return ReadType_arayuayz(ref reader);
    }
    protected static (byte[], uint, byte[], uint)[] ReadMessage_v_arayuayuz(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("a(ayuayu)");
        return ReadType_arayuayuz(ref reader);
    }
    protected static byte[][] ReadMessage_v_aay(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        reader.ReadSignature("aay");
        return ReadType_aay(ref reader);
    }
    protected static (ObjectPath, Dictionary<string, VariantValue>) ReadMessage_oaesv(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        var arg0 = reader.ReadObjectPath();
        var arg1 = reader.ReadDictionaryOfStringToVariantValue();
        return (arg0, arg1);
    }
    protected static (bool, string[]) ReadMessage_bas(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        var arg0 = reader.ReadBool();
        var arg1 = reader.ReadArrayOfString();
        return (arg0, arg1);
    }
    protected static bool ReadMessage_b(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        return reader.ReadBool();
    }
    protected static Dictionary<string, Dictionary<string, VariantValue>> ReadMessage_aesaesv(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        return ReadType_aesaesv(ref reader);
    }
    protected static Dictionary<string, VariantValue> ReadMessage_aesv(Message message, NetworkManagerObject _)
    {
        var reader = message.GetBodyReader();
        return reader.ReadDictionaryOfStringToVariantValue();
    }
    protected static uint[][] ReadType_aau(ref Reader reader)
    {
        List<uint[]> list = new();
        ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Array);
        while (reader.HasNext(arrayEnd))
        {
            list.Add(reader.ReadArrayOfUInt32());
        }
        return list.ToArray();
    }
    protected static Dictionary<string, VariantValue>[] ReadType_aaesv(ref Reader reader)
    {
        List<Dictionary<string, VariantValue>> list = new();
        ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Array);
        while (reader.HasNext(arrayEnd))
        {
            list.Add(reader.ReadDictionaryOfStringToVariantValue());
        }
        return list.ToArray();
    }
    protected static (uint, uint) ReadType_ruuz(ref Reader reader)
    {
        return (reader.ReadUInt32(), reader.ReadUInt32());
    }
    protected static (byte[], uint, byte[])[] ReadType_arayuayz(ref Reader reader)
    {
        List<(byte[], uint, byte[])> list = new();
        ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
        while (reader.HasNext(arrayEnd))
        {
            list.Add(ReadType_rayuayz(ref reader));
        }
        return list.ToArray();
    }
    protected static (byte[], uint, byte[]) ReadType_rayuayz(ref Reader reader)
    {
        return (reader.ReadArrayOfByte(), reader.ReadUInt32(), reader.ReadArrayOfByte());
    }
    protected static (byte[], uint, byte[], uint)[] ReadType_arayuayuz(ref Reader reader)
    {
        List<(byte[], uint, byte[], uint)> list = new();
        ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
        while (reader.HasNext(arrayEnd))
        {
            list.Add(ReadType_rayuayuz(ref reader));
        }
        return list.ToArray();
    }
    protected static (byte[], uint, byte[], uint) ReadType_rayuayuz(ref Reader reader)
    {
        return (reader.ReadArrayOfByte(), reader.ReadUInt32(), reader.ReadArrayOfByte(), reader.ReadUInt32());
    }
    protected static byte[][] ReadType_aay(ref Reader reader)
    {
        List<byte[]> list = new();
        ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Array);
        while (reader.HasNext(arrayEnd))
        {
            list.Add(reader.ReadArrayOfByte());
        }
        return list.ToArray();
    }
    protected static Dictionary<ObjectPath, Dictionary<string, Dictionary<string, VariantValue>>> ReadType_aeoaesaesv(ref Reader reader)
    {
        Dictionary<ObjectPath, Dictionary<string, Dictionary<string, VariantValue>>> dictionary = new();
        ArrayEnd dictEnd = reader.ReadDictionaryStart();
        while (reader.HasNext(dictEnd))
        {
            var key = reader.ReadObjectPath();
            var value = ReadType_aesaesv(ref reader);
            dictionary[key] = value;
        }
        return dictionary;
    }
    protected static Dictionary<string, Dictionary<string, VariantValue>> ReadType_aesaesv(ref Reader reader)
    {
        Dictionary<string, Dictionary<string, VariantValue>> dictionary = new();
        ArrayEnd dictEnd = reader.ReadDictionaryStart();
        while (reader.HasNext(dictEnd))
        {
            var key = reader.ReadString();
            var value = reader.ReadDictionaryOfStringToVariantValue();
            dictionary[key] = value;
        }
        return dictionary;
    }
    protected static Dictionary<string, string> ReadType_aess(ref Reader reader)
    {
        Dictionary<string, string> dictionary = new();
        ArrayEnd dictEnd = reader.ReadDictionaryStart();
        while (reader.HasNext(dictEnd))
        {
            var key = reader.ReadString();
            var value = reader.ReadString();
            dictionary[key] = value;
        }
        return dictionary;
    }
    protected static Dictionary<string, uint> ReadType_aesu(ref Reader reader)
    {
        Dictionary<string, uint> dictionary = new();
        ArrayEnd dictEnd = reader.ReadDictionaryStart();
        while (reader.HasNext(dictEnd))
        {
            var key = reader.ReadString();
            var value = reader.ReadUInt32();
            dictionary[key] = value;
        }
        return dictionary;
    }
    protected static void WriteType_aesaesv(ref MessageWriter writer, Dictionary<string, Dictionary<string, Variant>> value)
    {
        ArrayStart arrayStart = writer.WriteDictionaryStart();
        foreach (var item in value)
        {
            writer.WriteDictionaryEntryStart();
            writer.WriteString(item.Key);
            writer.WriteDictionary(item.Value);
        }
        writer.WriteDictionaryEnd(arrayStart);
    }
    protected static void WriteType_aau(ref MessageWriter writer, uint[][] value)
    {
        ArrayStart arrayStart = writer.WriteArrayStart(DBusType.Array);
        foreach (var item in value)
        {
            writer.WriteArray(item);
        }
        writer.WriteArrayEnd(arrayStart);
    }
    protected static void WriteType_aaesv(ref MessageWriter writer, Dictionary<string, Variant>[] value)
    {
        ArrayStart arrayStart = writer.WriteArrayStart(DBusType.Array);
        foreach (var item in value)
        {
            writer.WriteDictionary(item);
        }
        writer.WriteArrayEnd(arrayStart);
    }
    protected static void WriteType_ruuz(ref MessageWriter writer, (uint, uint) value)
    {
        writer.WriteStructureStart();
        writer.WriteUInt32(value.Item1);
        writer.WriteUInt32(value.Item2);
    }
    protected static void WriteType_arayuayz(ref MessageWriter writer, (byte[], uint, byte[])[] value)
    {
        ArrayStart arrayStart = writer.WriteArrayStart(DBusType.Struct);
        foreach (var item in value)
        {
            WriteType_rayuayz(ref writer, item);
        }
        writer.WriteArrayEnd(arrayStart);
    }
    protected static void WriteType_rayuayz(ref MessageWriter writer, (byte[], uint, byte[]) value)
    {
        writer.WriteStructureStart();
        writer.WriteArray(value.Item1);
        writer.WriteUInt32(value.Item2);
        writer.WriteArray(value.Item3);
    }
    protected static void WriteType_arayuayuz(ref MessageWriter writer, (byte[], uint, byte[], uint)[] value)
    {
        ArrayStart arrayStart = writer.WriteArrayStart(DBusType.Struct);
        foreach (var item in value)
        {
            WriteType_rayuayuz(ref writer, item);
        }
        writer.WriteArrayEnd(arrayStart);
    }
    protected static void WriteType_rayuayuz(ref MessageWriter writer, (byte[], uint, byte[], uint) value)
    {
        writer.WriteStructureStart();
        writer.WriteArray(value.Item1);
        writer.WriteUInt32(value.Item2);
        writer.WriteArray(value.Item3);
        writer.WriteUInt32(value.Item4);
    }
    protected static void WriteType_aay(ref MessageWriter writer, byte[][] value)
    {
        ArrayStart arrayStart = writer.WriteArrayStart(DBusType.Array);
        foreach (var item in value)
        {
            writer.WriteArray(item);
        }
        writer.WriteArrayEnd(arrayStart);
    }
}