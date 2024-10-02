using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using EventPi.Abstractions;
using MicroPlumberd;
using ModelingEvolution.VideoStreaming;

namespace EventPi.Services.Camera;

public class AiCameraConfigurationProvider(IPlumber plumber, IEnvironment env)
{
    record Item(AiModelConfiguration Handler, IAsyncDisposable Subscription);
    private readonly ConcurrentDictionary<int, Item> _index = new();
    public AiModelConfiguration Get(VideoAddress va)
    {
        var item = _index.GetOrAdd(va.CameraNumber ?? 0, x =>
        {
            string streamName = AiModelConfigurationState.StreamName(env.HostName, x);
               var handler = new AiModelConfiguration() { VideoAddress = va };
               var sub = plumber.SubscribeStateEventHandler(handler, 
                   streamName, 
                   FromRelativeStreamPosition.End-1,
                   ensureOutputStreamProjection: false);
               return new Item(handler,sub.GetAwaiter().GetResult());
           });
        return item.Handler;
    }

    public async Task Save(VideoAddress va, AiModelConfigurationState configuration)
    {
        string streamId = $"{env.HostName}/{va.CameraNumber ?? 0}";
        await plumber.AppendState(configuration, streamId);
    }
    
}
