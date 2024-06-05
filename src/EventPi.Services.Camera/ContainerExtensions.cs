using MicroPlumberd.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

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
        services.AddSingleton<GrpcFrameFeaturesService>();
        services.AddScoped<CameraCommandHandler>();
        services.AddCommandHandler<CameraCommandHandler>();
        return services;
    }
}