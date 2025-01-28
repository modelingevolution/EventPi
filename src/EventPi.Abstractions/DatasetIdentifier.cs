using ProtoBuf;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace EventPi.Abstractions;

[JsonConverter(typeof(JsonParsableConverter<DatasetIdentifier>))]
[ProtoContract]
public readonly record struct DatasetIdentifier : IParsable<DatasetIdentifier>
{
    [ProtoMember(1)]
    public Guid Id { get; init; }

    public DatasetIdentifier()
    {
        Id = Guid.NewGuid();
    }
    public DatasetIdentifier(Guid id)
    {
        Id = id;
    }

    public static DatasetIdentifier Parse(string s, IFormatProvider? provider = null)
    {
        if (string.IsNullOrEmpty(s)) throw new ArgumentNullException(nameof(s));

        return new DatasetIdentifier(Guid.Parse(s, provider));
    }
    
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out DatasetIdentifier result)
    {
        result = default;
        if (string.IsNullOrEmpty(s)) return false;

        result = new DatasetIdentifier(Guid.Parse(s, provider));
        return true;
    }

    public override string ToString()
    {
        return Id.ToString();
    }

    public static implicit operator Guid(DatasetIdentifier addr)
    {
        return addr.Id;
    }
    public static implicit operator DatasetIdentifier(Guid addr)
    {
        return new DatasetIdentifier(addr);
    }
}