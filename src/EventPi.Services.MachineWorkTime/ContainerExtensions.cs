using EventPi.Services.MachineWorkTime;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace EventPi;

public static class ContainerExtensions
{
    public static IServiceCollection AddMachineWorkTime(this IServiceCollection services)
    {
        services.AddSingleton<IRfidHandler, RfidHandler>();
        services.AddSingleton<RfidState>();
        return services;
    }
}