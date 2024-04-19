using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace EventPi.Services.Camera.Tests;

public partial class TestAppHost : IDisposable
{
    public IHost Host { get; private set; }
    private readonly ITestOutputHelper logger;

    public TestAppHost(ITestOutputHelper logger)
    {
        this.logger = logger;
    }

    public virtual TestAppHost Configure(Action<IServiceCollection>? configure = null)
    {
        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureLogging(x => x.SetMinimumLevel(LogLevel.Trace)
                .AddDebug())
            .ConfigureServices(services =>
            {
                configure(services);
            })
            .Build();

        return this;
    }

    public void Dispose()
    {
        Host?.Dispose();
    }


    public async Task<IServiceProvider> StartAsync()
    {
        await Host.StartAsync();
        await Task.Delay(1000);
        return Host.Services;
    }
}