using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;

namespace EventPi.Abstractions;

public static class ContainerExtensions
{
    public static IServiceCollection WhenUnix(this IServiceCollection services, Func<IServiceCollection, IServiceCollection> configure)
    {
        return services.When(() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX), configure);
    }
    public static IServiceCollection When(this IServiceCollection services, Func<bool> predicate,  Func<IServiceCollection, IServiceCollection> configure)
    {
        if (predicate())
            return configure(services);
        return services;
    }
    public static IServiceCollection AddEventPiAbstractions(this IServiceCollection services, string wwwRoot, string contentPath) => 
        services.AddSingleton<IEnvironment, HostEnvironment>()
            .AddSingleton<IWebHostingEnv>( new WebHostEnv(wwwRoot, contentPath));
}