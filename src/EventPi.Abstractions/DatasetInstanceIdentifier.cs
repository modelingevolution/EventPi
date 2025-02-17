using ProtoBuf;
using System.Text.Json.Serialization;
using MicroPlumberd;

namespace EventPi.Abstractions;

[JsonConverter(typeof(JsonParsableConverter<DatasetInstanceIdentifier>))]
[ProtoContract]
public readonly record struct DatasetInstanceIdentifier : IParsable<DatasetInstanceIdentifier>
{
    [ProtoMember(1)]
    public DatasetIdentifier Id { get; init; }

    [ProtoMember(2)]
    public long Version { get; init; }

    public DatasetInstanceIdentifier(DatasetIdentifier id, long version)
    {
        Id = id;
        Version=version;
    }
    public static bool TryParse(string? input, IFormatProvider? formatProvider, out DatasetInstanceIdentifier result)
    {
        try
        {
            result = Parse(input!, formatProvider);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
    public static implicit operator Guid(DatasetInstanceIdentifier instance)
    {
        return instance.ToString().ToGuid();
    }
    public override string ToString()
    {
        return $"{Id.ToString()}.{Version.ToString()}";
    }
    public static DatasetInstanceIdentifier Parse(string input, IFormatProvider? formatProvider = null)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentNullException(nameof(input), "Input string cannot be null or empty.");
        }

        try
        {
            int lastIndex = input.LastIndexOf('.');
            DatasetIdentifier datasetId = DatasetIdentifier.Parse(input.Remove(lastIndex));
            long version = long.Parse(input.Substring(lastIndex + 1));
            return new DatasetInstanceIdentifier(datasetId, version);

        }
        catch (Exception ex) when (ex is not FormatException)
        {
            throw new FormatException("Failed to parse input string", ex);
        }
    }

 
}
