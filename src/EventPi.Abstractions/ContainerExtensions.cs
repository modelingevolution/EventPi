using Microsoft.Extensions.DependencyInjection;

namespace EventPi.Abstractions;

public static class ContainerExtensions
{
    public static IServiceCollection AddEventPiAbstractions(this IServiceCollection services, string wwwRoot, string contentPath) => 
        services.AddSingleton<IEnvironment, HostEnvironment>()
            .AddSingleton<IWebHostingEnv>( new WebHostEnv(wwwRoot, contentPath));
}