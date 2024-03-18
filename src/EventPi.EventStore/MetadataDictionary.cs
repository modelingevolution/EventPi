using ModelingEvolution.Plumberd;
using ModelingEvolution.Plumberd.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventPi.EventStore;

[JsonConverter(typeof(MetadataDictionaryConverter))]
internal class MetadataDictionary : IMetadata
{
    public MetadataDictionary()
    {
        Items = new List<Pair>();
    }

    public IList<Pair> Items { get; }

    public ILink Link(string destinationCategory)
    {
        throw new NotImplementedException();
    }

    public IMetadataSchema Schema { get; }

    public object this[MetadataProperty property]
    {
        get => Items.First(x => x.Key == property.Name).Value;
        set
        {
            var item = Items.FirstOrDefault(x => x.Key == property.Name);
            if (item == null)
                Items.Add(new Pair(property.Name, value));
            else throw new NotSupportedException();
        }
    }

    public static Guid DefaultSessionId = Guid.NewGuid();
    public static Guid DefaultUserId = Guid.Empty;
    public static MetadataDictionary Create(Type type,
        Guid? correlationId = null,
        Guid? causationId = null,
        int hop = 1,
        Guid? userId = null,
        Guid? sessionId = null,
        string processingUnit = null,
        Version version = null,
        DateTimeOffset? created = null)
    {
        var cid = correlationId ?? Guid.NewGuid();
        var r = new MetadataDictionary()
            .Add("$correlationId", cid)
            .Add("$causationId", causationId ?? cid)
            .Add("Hop", hop)
            .Add("UserId", userId ?? DefaultUserId)
            .Add("SessionId", sessionId ?? DefaultSessionId)
            .Add("Type", type.AssemblyQualifiedName.ToString())
            .Add("ProcessingUnit", processingUnit ?? "-")
            .Add("Version", version?.ToString() ?? "0.0.0.0")
            .Add("Created", created ?? DateTimeOffset.Now);

        return r;
    }

    public MetadataDictionary Add(string key, object value)
    {
        Items.Add(new Pair(key, value));
        return this;
    }

    public record Pair(string Key, object Value);
}