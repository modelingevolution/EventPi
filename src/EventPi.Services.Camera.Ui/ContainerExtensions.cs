using System.Collections.Concurrent;
using EventPi.Services.Camera.Ui;
using MicroPlumberd.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace EventPi.Services.Camera;

public static class ContainerExtensions
{
    public static IServiceCollection AddCameraConfigurationUi(this IServiceCollection services)
    {
        services.AddTransient<CameraControlsVm>();
        services.AddScoped<CameraControlVmRegister>();
        return services;
    }

  
}
class CameraControlVmRegister
{
    private readonly ConcurrentDictionary<string, CameraControlsVm> _index = new();
    private readonly IServiceProvider _serviceProvider;

    public CameraControlsVm Get(string path) =>
        _index.GetOrAdd(path, x => _serviceProvider.GetRequiredService<CameraControlsVm>());
    public CameraControlVmRegister(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
}