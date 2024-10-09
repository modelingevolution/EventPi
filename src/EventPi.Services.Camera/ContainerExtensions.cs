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
using ModelingEvolution.VideoStreaming;

namespace EventPi.Services.Camera;


public static class ContainerExtensions
{
    public static IEndpointRouteBuilder MapCamera(this IEndpointRouteBuilder builder)
    {
       // builder.MapGrpcService<GrpcFrameFeaturesService>();
        return builder;
    }
    public static IServiceCollection AddCameraConfiguration(this IServiceCollection services, IConfiguration config, bool disableAutostart = false)
    {
        services.AddSingleton<AiCameraConfigurationProvider>();
        services.AddSingleton<CameraManager>();
        services.AddTransient<WeldingRecognitionService>();
        

        services.AddSingleton<FrameFeatureAccessor>();
        services.AddSingleton<FeaturePerformanceInfo>();
        //services.AddSingleton<CameraProfileConfigurationModel>( );
       // services.AddSingleton<GrpcFrameFeaturesService>();
        services.AddSingleton<WeldingRecognitionCommandHandler>();
        services.AddTransient<WeldingRecognitionModel>();
        services.AddSingleton<CameraCommandHandler>();
        services.AddCommandHandler<CameraCommandHandler>();
        services.AddCommandHandler<WeldingRecognitionCommandHandler>();
        //services.AddStateEventHandler<CameraProfileConfigurationModel>();
        //services.AddStateEventHandler<WeldingRecognitionModel>();
        if(!disableAutostart)
            services.WhenUnix(services => services.AddHostedService<CameraStarter>());
        services.AddSingleton<CameraSimulatorProcess>();

        services.AddSingleton<LibCameraProcess>(sp => new LibCameraProcess(sp.GetRequiredService<IConfiguration>(), sp, sp.GetRequiredService<ILogger<LibCameraProcess>>()));
        services.AddSingleton<OpenVidCamProcess>(sp => new OpenVidCamProcess(sp.GetRequiredService<IConfiguration>(),
            sp.GetRequiredService<ILoggerFactory>(), 
            sp.GetRequiredService<ILogger<OpenVidCamProcess>>()));
        
        CameraSimulatorProcess.KillAll(config);
        return services;
    }
}