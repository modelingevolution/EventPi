using System.Collections.Concurrent;
using EventPi.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EventPi.Services.NetworkMonitor.Ui;

public static class ContainerExtensions
{
    public static IServiceCollection AddNetworkManagerUi(this IServiceCollection services) =>
        services
            .AddSingleton(typeof(VmHostRegister<>))
            .AddTransient<WirelessStationsVm>()
            .AddTransient<WirelessProfilesVm>()
            .AddTransient<WirelessConnectionVm>();
}

internal class VmHostRegister<T>(IServiceProvider serviceProvider)
{
    private readonly ConcurrentDictionary<HostName, T> _index = new();
    public T Get(HostName name) => _index.GetOrAdd(name, x => serviceProvider.GetRequiredService<T>());
}