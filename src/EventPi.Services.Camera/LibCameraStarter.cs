using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventPi.Services.Camera;

public class LibCameraProcess(IConfiguration configuration, 
    IServiceProvider sp, ILogger<LibCameraProcess> log)
{
    public async Task Start(VideoCodec? codec = null, VideoTransport? vt = null, Resolution? res = null, bool killAll = true, string? shmName = null)
    {
        var transport = vt ?? VideoTransport.Shm;
        var resolution = res ?? configuration.GetCameraResolution();
        var libCameraPath = configuration.GetLibCameraPath();
        var videoCodec = codec ?? VideoCodec.Mjpeg;

        var vid = new LibCameraVid(sp.GetRequiredService<ILogger<LibCameraVid>>(), libCameraPath);
        if (killAll && vid.KillAll()) 
            await Task.Delay(1000);
        
        var p = await vid.Start(resolution, videoCodec, 
            configuration.GetLibCameraTuningPath(), 
            transport,
            configuration.GetLibCameraListenIp(),
            configuration.GetLibCameraVideoListenPort(),
            configuration.GetLibcameraGrpcFullListenAddress(), shmName ?? "default");
        log.LogInformation($"libcamera-vid started, pid: {p}");
    }
}
public class LibCameraStarter(IConfiguration configuration, LibCameraProcess proc) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if(!configuration.IsCameraAutostart()) return;
        
        await proc.Start();
    }
}