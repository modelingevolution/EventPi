using System.Diagnostics;

namespace EventPi.Services.Camera;

public record FeaturePerformanceInfo
{
    private readonly Stopwatch _sw = new();

    public FeaturePerformanceInfo()
    {
        this.Counter = Counter;
        _sw.Start();
    }

    public ulong Counter { get; private set; }

    public TimeSpan AveragePeriod => _sw.Elapsed/Counter;
    public void Increment()
    {
        Counter += 1;
    }
}