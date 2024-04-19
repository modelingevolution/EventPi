using System.Collections.Concurrent;
using MicroPlumberd;

namespace EventPi.Services.Camera.Tests;

public class InMemoryModelStore
{
    public record Item(Metadata Metadata, object Event);

    public readonly SortedList<int, Item> Index = new();
    public readonly ConcurrentDictionary<Guid, List<Item>> IndexById = new();

    private int _i = -1;
    public void Given(Metadata m, object evt)
    {
        var i = new Item(m, evt);
        Index.Add(Interlocked.Increment(ref _i), i);
        IndexById.GetOrAdd(m.Id, x => new()).Add(i);
    }

    public async Task<T?> FindLast<T>(Guid id)
    {
        for (int i = 0; i < 100; i++)
            if (!IndexById.ContainsKey(id))
                await Task.Delay(100);
        return IndexById[id]
            .Where(x => x.Event is T)
            .Select(x => x.Event)
            .OfType<T>()
            .Reverse()
            .FirstOrDefault();
    }
}