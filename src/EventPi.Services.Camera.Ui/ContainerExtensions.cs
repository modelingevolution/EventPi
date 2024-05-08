using EventPi.Services.Camera.Ui;
using MicroPlumberd.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace EventPi.Services.Camera;

public static class ContainerExtensions
{
    public static IServiceCollection AddCameraConfigurationUi(this IServiceCollection services)
    {
        services.AddSingleton<CameraControlsVm>();
        return services;
    }
}