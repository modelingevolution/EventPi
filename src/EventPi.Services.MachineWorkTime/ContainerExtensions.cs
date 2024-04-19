using System.Runtime.CompilerServices;
using EventPi.Services.MachineWorkTime;
using Microsoft.Extensions.DependencyInjection;
[assembly: InternalsVisibleTo("EventPi.Services.MachineWorkTime.Tests")]
// ReSharper disable once CheckNamespace
namespace EventPi;

public static class ContainerExtensions
{
    public static IServiceCollection AddMachineWorkTime(this IServiceCollection services)
    {
        services.AddSingleton<IRfidHandler, RfidHandler>();
        services.AddSingleton<IRfidState,RfidState>();
        return services;
    }
}