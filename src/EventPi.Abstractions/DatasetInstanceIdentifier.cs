using ProtoBuf;
using System.Text.Json.Serialization;
using MicroPlumberd;

namespace EventPi.Abstractions;

[JsonConverter(typeof(JsonParsableConverter<DatasetInstanceIdentifier>))]
[ProtoContract]
public readonly record struct DatasetInstanceIdentifier : IParsable<DatasetInstanceIdentifier>, IComparable<DatasetInstanceIdentifier>
{
    [ProtoMember(1)]
    public DatasetIdentifier Id { get; init; }

    [ProtoMember(2)]
    public long DatasetVersion { get; init; }

    [ProtoMember(3)]
    public long AnnotationVersion { get; init; }

    public DatasetInstanceIdentifier(DatasetIdentifier id, long datasetVersion, long annotationVersion)
    {
        Id = id;
        DatasetVersion=datasetVersion;
        AnnotationVersion =annotationVersion;
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
        return $"{Id.ToString()}.{DatasetVersion.ToString()}.{AnnotationVersion.ToString()}";
    }
    public static DatasetInstanceIdentifier Parse(string input, IFormatProvider? formatProvider = null)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentNullException(nameof(input), "Input string cannot be null or empty.");
        }

        try
        {
            var splited = input.Split('.');
            DatasetIdentifier datasetId = DatasetIdentifier.Parse(splited[0]);
            long datasetVersion = long.Parse(splited[1]);
            long annotationVersion = long.Parse(splited[2]);
            return new DatasetInstanceIdentifier(datasetId, datasetVersion,annotationVersion);

        }
        catch (Exception ex) when (ex is not FormatException)
        {
            throw new FormatException("Failed to parse input string", ex);
        }
    }


    public int CompareTo(DatasetInstanceIdentifier other)
    {
        var idComparison = Id.CompareTo(other.Id);
        if (idComparison != 0) return idComparison;
        var datasetVersionComparison = DatasetVersion.CompareTo(other.DatasetVersion);
        if (datasetVersionComparison != 0) return datasetVersionComparison;
        return AnnotationVersion.CompareTo(other.AnnotationVersion);
    }
}
