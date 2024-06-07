using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventPi.Services.Camera;

public class LibCameraProcess(IConfiguration configuration, 
    IServiceProvider sp, ILogger<LibCameraProcess> log,
    string grpcClientAddress)
{
    public async Task Restart()
    {
        var resolution = configuration.GetCameraResolution();
        var libCameraPath = configuration.GetLibCameraPath();
        var vid = new LibCameraVid(sp.GetRequiredService<ILogger<LibCameraVid>>(), libCameraPath);
        if (vid.KillAll()) await Task.Delay(1000);
        var p = await vid.Start(resolution, Codec.mjpeg, configuration.GetLibCameraTuningPath(), configuration.GetLibCameraListenIp(), configuration.GetLibCameraVideoListenPort(), configuration.GetLibcameraGrpcFullListenAddress(), grpcClientAddress);
        log.LogInformation($"libcamera-vid started, pid: {p}");
    }
}
public class LibCameraStarter(IConfiguration configuration, LibCameraProcess proc) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if(!configuration.GetCameraAutostart()) return;
        
        await proc.Restart();
    }
}