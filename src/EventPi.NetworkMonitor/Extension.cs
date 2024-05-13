namespace EventPi.NetworkMonitor;

#pragma warning disable CS1998
static class Extension
{
    public static async Task<T> FirstOrDefault<T>(this IAsyncEnumerable<T> items, Predicate<T>? predicate = null)
    {
        predicate ??= x => true;
        await foreach (var i in items)
        {
            if(predicate(i)) return i;
        }

        return default;
    }
}