using EventPi.Abstractions;
using EventPi.EventStore;
using Microsoft.Extensions.DependencyInjection;
using ModelingEvolution.Plumberd;
using ModelingEvolution.Plumberd.EventStore;

namespace EventPi.EventStore;



class EventStoreStream : IEventStoreStream
{
    private readonly IEventStore _eventStore;

    public EventStoreStream(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }


    public async Task Write<T>(Guid id, T e) where T : IEvent
    {
        await _eventStore.GetStaticStream<T>(id)
            .Append(e, MetadataDictionary.Create(typeof(T)));
    }
}

public static class ContainerExtensions
{
    public static IServiceCollection AddEventStoreStream(this IServiceCollection services)
    {
        services.AddSingleton<IEventStoreStream, EventStoreStream>();
        return services;
    }
}