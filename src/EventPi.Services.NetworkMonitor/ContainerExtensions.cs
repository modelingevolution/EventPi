using MicroPlumberd.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventPi.Services.NetworkMonitor;

public static class ContainerExtensions
{
    public static IServiceCollection AddNetworkManager(this IServiceCollection services)
    {
        services.AddBackgroundServiceIfMissing<NetworkManagerListener>();
        services.AddSingleton<NetworkManagerCommandHandler>();
        services.AddCommandHandler<NetworkManagerCommandHandler>();
        return services;
    }
}