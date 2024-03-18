using System.Reflection;
using ModelingEvolution.Plumberd;
using ModelingEvolution.Plumberd.EventStore;
using ModelingEvolution.Plumberd.Serialization;


namespace EventPi.EventStore;

public static class EventStoreExtensions
{
    private static readonly IMetadataSerializer StaticMetadataSerializer = new StaticMetadataSerializer();

    public static IStream GetStaticStream<T>(this IEventStore _eventStore, Guid id) where T : IEvent
    {
        var eventType = typeof(T);
        var name = eventType.GetCustomAttribute<StreamAttribute>()?.Category ?? eventType.Namespace.LastSegment('.');
        return _eventStore.GetStream(name, id, null, StaticMetadataSerializer);
    }
}