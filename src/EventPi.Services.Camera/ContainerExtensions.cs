using MicroPlumberd.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace EventPi.Services.Camera;

public static class ContainerExtensions
{
    public static IServiceCollection AddCameraConfiguration(this IServiceCollection services)
    {
        services.AddScoped<CameraCommandHandler>();
        services.AddCommandHandler<CameraCommandHandler>();
        return services;
    }
}