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
        if (services.Length == 0)
            container.AddSingleton<IServiceName>((ServiceName)"iot.www");

        foreach (var service in services)
            container.AddSingleton<IServiceName>(service);

        container.TryAddSingleton<ILocalDiscoveryService, LocalDiscoveryService>();
        return container;
    }
    public static IServiceCollection AddAdvertiser(this IServiceCollection container,params ServiceInfo[] services)
    {
        foreach (var service in services)
            container.AddSingleton<ServiceInfo>(service);

        container.TryAddSingleton<IServiceProfileEnricher, WifiAndEthernetEnricher>();
        container.AddSingleton<AdvertiserService>();
        container.AddHostedService<AdvertiserService>();
        return container;

    }
    public static IServiceCollection AddAdvertiser(this IServiceCollection services, string? serviceName = "iot.www") => services.AddAdvertiser(new ServiceInfo("http", serviceName ?? throw new ArgumentNullException(nameof(serviceName)), null));
    public static IServiceCollection AddAdvertiser(this IServiceCollection services, int port) => services.AddAdvertiser(new ServiceInfo("http", "iot.www",port));

    public static IServiceCollection AddAdvertiser(this IServiceCollection services, string schema, string serviceName, int? port=null) => services.AddAdvertiser(new ServiceInfo(schema, serviceName, port));
}