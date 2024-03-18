using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventPi.EventStore;

internal class MetadataDictionaryConverter : JsonConverter<MetadataDictionary>
{
    public override MetadataDictionary? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, MetadataDictionary value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var i in value.Items)
        {
            writer.WritePropertyName(i.Key);
            JsonSerializer.Serialize(writer, i.Value, i.Value.GetType(), options);
        }

        writer.WriteEndObject();
    }
}