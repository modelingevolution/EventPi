using EventPi.Advertiser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace EventPi;

public static class ContainerExtensions
{
    public static IServiceCollection AddAdvertiser(this IServiceCollection services, string serviceName, int port)
    {
        services.AddHostedService<RpiAdvertiseService>(sp =>
            new RpiAdvertiseService(sp.GetRequiredService<ILogger<RpiAdvertiseService>>(), serviceName, port));
        return services;
        
    }
}