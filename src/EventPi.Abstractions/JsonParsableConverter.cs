using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventPi.Abstractions;

public class JsonParsableConverter<T> : JsonConverter<T> where T:IParsable<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        return T.Parse(str, null);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}