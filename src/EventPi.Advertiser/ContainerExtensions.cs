using EventPi.Advertiser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace EventPi;

interface IServiceInfo
{
    string ServiceName { get; }
    int Port { get; }
};

public record ServiceInfo(string ServiceName, int Port) : IServiceInfo;
public static class ContainerExtensions
{
    public static IServiceCollection AddAdvertiser(this IServiceCollection container,params ServiceInfo[] services)
    {
        foreach (var service in services)
            container.AddSingleton<IServiceInfo>(service);
        
        
        container.AddHostedService<AdvertiserService>();
        return container;

    }
    public static IServiceCollection AddAdvertiser(this IServiceCollection services, string serviceName, int port)
    {
        return services.AddAdvertiser(new ServiceInfo(serviceName, port));

    }
}