using MicroPlumberd.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using EventPi.Abstractions;
using EventStore.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MicroPlumberd;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EventPi.Services.Camera;


public static class ContainerExtensions
{
    public static IEndpointRouteBuilder MapCamera(this IEndpointRouteBuilder builder)
    {
        builder.MapGrpcService<GrpcFrameFeaturesService>();
        return builder;
    }
    public static IServiceCollection AddCameraConfiguration(this IServiceCollection services, bool disableAutostart = false)
    {
        services.AddSingleton<GrpcCppCameraProxy>();
        services.AddSingleton<WeldingRecognitionService>();
        services.AddHostedService<WeldingRecognitionService>();
        services.AddSingleton<FrameFeatureAccessor>();
        services.AddSingleton<FeaturePerformanceInfo>();
        services.AddSingleton<CameraProfileConfigurationModel>( );
        services.AddSingleton<GrpcFrameFeaturesService>();
        services.AddSingleton<WeldingRecognitionCommandHandler>();
        services.AddSingleton<WeldingRecognitionModel>();
        services.AddSingleton<CameraCommandHandler>();
        services.AddCommandHandler<CameraCommandHandler>();
        services.AddCommandHandler<WeldingRecognitionCommandHandler>();
        services.AddStateEventHandler<CameraProfileConfigurationModel>();
        services.AddStateEventHandler<WeldingRecognitionModel>();
        if(!disableAutostart)
            services.WhenUnix(services => services.AddHostedService<LibCameraStarter>());
        services.AddSingleton<CameraSimulatorProcess>();
        services.AddSingleton<LibCameraProcess>(sp => new LibCameraProcess(sp.GetRequiredService<IConfiguration>(), sp,
            sp.GetRequiredService<ILogger<LibCameraProcess>>()));

        return services;
    }
}