using MicroPlumberd;

namespace EventPi.Abstractions;

public static class MetadataExtensions
{
    public static T StreamId<T>(this Metadata m) where T:IParsable<T>
    {
        int index = m.SourceStreamId.IndexOf('-');
        string id = m.SourceStreamId.Substring(index + 1);
        return T.Parse(id,null);
    }
}