using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;

namespace EventPi.Abstractions;

public static class ContainerExtensions
{
    public static Guid Xor(this Guid a, Guid b)
    {

        byte[] bytes1 = a.ToByteArray();
        byte[] bytes2 = b.ToByteArray();
        byte[] result = new byte[16];

        for (int i = 0; i < 16; i++)
        {
            result[i] = (byte)(bytes1[i] ^ bytes2[i]);
        }

        return new Guid(result);

    }
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