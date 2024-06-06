using System.Collections.Concurrent;
using EventPi.Services.Camera.Ui;

using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace EventPi.Services.Camera;

public static class ContainerExtensions
{
    public static IServiceCollection AddCameraConfigurationUi(this IServiceCollection services)
    {
        services.AddTransient<CameraControlsVm>();
        services.AddTransient<WeldingRecognitionVm>();
        services.AddScoped<CameraControlVmRegister>();
        services.AddScoped<WeldingRecognitionVmRegister>();
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
public class WeldingRecognitionVmRegister
{
    private readonly ConcurrentDictionary<string, WeldingRecognitionVm> _index = new();
    private readonly IServiceProvider _serviceProvider;

    public WeldingRecognitionVm Get(string path) =>
        _index.GetOrAdd(path, x => _serviceProvider.GetRequiredService<WeldingRecognitionVm>());
    public WeldingRecognitionVmRegister(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
}