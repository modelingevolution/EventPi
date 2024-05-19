using MicroPlumberd.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace EventPi.Services.Camera;

public static class ContainerExtensions
{
    public static IServiceCollection AddCameraConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<GrpcCppCameraProxy>();
        services.AddSingleton<WeldingRecognitionService>();
        services.AddSingleton<GrpcFrameFeaturesService>();
        services.AddScoped<CameraCommandHandler>();
        services.AddCommandHandler<CameraCommandHandler>();
        return services;
    }
}