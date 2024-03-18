using System.Text;
using System.Text.Json;
using ModelingEvolution.Plumberd.Metadata;
using ModelingEvolution.Plumberd.Serialization;

namespace EventPi.EventStore;

class StaticMetadataSerializer : IMetadataSerializer
{
    private readonly JsonSerializerOptions _options;

    public StaticMetadataSerializer()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new JsonTimeSpanConverter());
        _options.Converters.Add(new MetadataDictionaryConverter());
    }

    public byte[] Serialize(IMetadata m)
    {
        if (m is MetadataDictionary e)
        {
            var str = JsonSerializer.Serialize(e, _options);

            return Encoding.UTF8.GetBytes(str);
        }

        throw new InvalidOperationException("Unsupported scenario.");
    }

    public IMetadata Deserialize(ReadOnlyMemory<byte> data)
    {
        throw new NotImplementedException();
    }

    public IMetadata Deserialize(byte[] data)
    {
        throw new NotImplementedException();
    }

    public IMetadataSchema Schema { get; }
}