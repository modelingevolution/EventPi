using MicroPlumberd.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace EventPi.Services.Camera;

public static class ContainerExtensions
{
    public static IServiceCollection AddCameraConfiguration(this IServiceCollection services)
    {
        services.AddCommandHandler<CameraProfileConfigurationCommandHandler>();
        return services;
    }
}