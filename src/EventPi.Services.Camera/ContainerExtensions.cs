using MicroPlumberd.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using EventStore.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MicroPlumberd;

namespace EventPi.Services.Camera;


public static class ContainerExtensions
{
    public static IEndpointRouteBuilder MapCamera(this IEndpointRouteBuilder builder)
    {
        builder.MapGrpcService<GrpcFrameFeaturesService>();
        return builder;
    }
    public static IServiceCollection AddCameraConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<GrpcCppCameraProxy>();
        services.AddSingleton<WeldingRecognitionService>();
        services.AddSingleton<FrameFeatureAccessor>();
        services.AddSingleton<FeaturePerformanceInfo>();
        services.AddSingleton<CameraProfileConfigurationModel>( );
        services.AddSingleton<GrpcFrameFeaturesService>();
        services.AddSingleton<WeldingRecognitionCommandHandler>();
        services.AddSingleton<WeldingRecognitionModel>();
        services.AddSingleton<CameraCommandHandler>();
        services.AddCommandHandler<CameraCommandHandler>();
        services.AddCommandHandler<WeldingRecognitionCommandHandler>();
        services.AddEventHandler<CameraProfileConfigurationModel>(false, FromStream.Start);
        services.AddEventHandler<WeldingRecognitionModel>(false, FromStream.Start); // TODO: Should be relative end -1!!!
        return services;
    }
}