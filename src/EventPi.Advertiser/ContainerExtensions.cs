using EventPi.Advertiser.Receiver;
using EventPi.Advertiser.Sender;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace EventPi;

public static class ContainerExtensions
{
    public static IServiceCollection AddLocalDiscoveryService(this IServiceCollection container, params ServiceName[] services)
    {
        foreach (var service in services)
            container.AddSingleton<IServiceName>(service);

        container.TryAddSingleton<ILocalDiscoveryService, LocalDiscoveryService>();
        return container;
    }
    public static IServiceCollection AddAdvertiser(this IServiceCollection container,params ServiceInfo[] services)
    {
        foreach (var service in services)
            container.AddSingleton<IServiceInfo>(service);

        container.TryAddSingleton<IServiceProfileEnricher, WifiAndEthernetEnricher>();
        container.AddSingleton<AdvertiserService>();
        container.AddHostedService<AdvertiserService>();
        return container;

    }
    public static IServiceCollection AddAdvertiser(this IServiceCollection services, string serviceName, int port)
    {
        return services.AddAdvertiser(new ServiceInfo(serviceName, port));

    }
}