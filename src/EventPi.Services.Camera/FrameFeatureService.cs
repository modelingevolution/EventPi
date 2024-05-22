using System.Diagnostics.Metrics;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventPi.Services.Camera;

public class FrameFeatureService(FrameFeatureAccessor accessor, FeaturePerformanceInfo counter, ILogger<FrameFeatureService> logger) : IHostedService
{
    private record Subscriber(Channel<FrameImageInfo> Channel, Func<FrameImageInfo, Task> Func);
    private CancellationTokenSource? _cancellationTokenSource = null;
    private FrameImageInfo _prv;
    private readonly List<Subscriber> _subscribers = new();
    private object _lock = new object();
    public event Func<FrameImageInfo, Task> FrameInfoCalculated
    {
        add
        {
            lock (_lock)
            {
                var ch = Channel.CreateUnbounded<FrameImageInfo>();
                var sub = new Subscriber(ch, value);
                _subscribers.Add(sub);
                var cts = _cancellationTokenSource;
                if (cts != null)
                    StartReader(ch, value, cts.Token);
            }
        }
        remove
        {
            throw new NotImplementedException();
        }
    }

    private void StartReader(Channel<FrameImageInfo> channel, Func<FrameImageInfo, Task> value, CancellationToken token = default)
    {
        _ = Task.Factory.StartNew(async () =>
        {

            await foreach (var i in channel.Reader.ReadAllAsync(token))
            {
                logger.LogInformation($"DarkPixels: {i.DarkPixels}, BrightPixels: {i.BrightPixels}");
                await value(i);
            }

        });
    }
    public async Task StartReading(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var value = accessor.FrameFeatures;
            if (!value.DarkPixels.Equals(_prv.DarkPixels) || !value.BrightPixels.Equals(_prv.BrightPixels))
            {
                foreach (var i in _subscribers)
                    i.Channel.Writer.TryWrite(value);
                _prv = value;
            }

            counter.Increment();
            await Task.Delay(16, cancellationToken); // 30 frames per second * 2 = 1/60s = 16ms
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting FrameFeatureService...");
        lock (_lock)
        {
            if (_cancellationTokenSource != null)
                throw new InvalidOperationException("You cannot invoke start when already running.");
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            foreach (var i in _subscribers)
            {
                StartReader(i.Channel, i.Func, _cancellationTokenSource.Token);
            }
        }

      
        _ = Task.Factory.StartNew(async () => await StartReading(_cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
        logger.LogInformation("FrameFeatureService reader started...");
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _cancellationTokenSource.CancelAsync();
        accessor.Dispose();
        _cancellationTokenSource = null;
    }
}