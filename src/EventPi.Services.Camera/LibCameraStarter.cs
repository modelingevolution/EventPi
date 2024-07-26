using CliWrap.Buffered;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace EventPi.Services.Camera;

public class CameraSimulatorProcess : IDisposable
{
    record RunningProc(CliWrap.CommandTask<BufferedCommandResult> Command, CancellationTokenSource ForceCts, CancellationTokenSource GraceCts);
    private readonly List<CancellationTokenSource> _tokenCancellationSources = new();
    private readonly string? _appName;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CameraSimulatorProcess> _logger;

    private readonly ConcurrentDictionary<string, RunningProc> _running = new();
    public CameraSimulatorProcess(IConfiguration configuration, ILogger<CameraSimulatorProcess> logger)
    {
        _configuration = configuration;
        _logger = logger;
        this._appName = _configuration.GetCameraSimulatorPath();

    }

    public bool KillAll()
    {
        var name = Path.GetFileName(_appName);
        bool killed = false;
        foreach (var i in Process.GetProcessesByName(name))
        {
            i.Kill();
            killed = true;
        }
        return killed;
    }
    public void Stop(string file)
    {
        if(_running.TryGetValue(file, out var r)){
            r.GraceCts.Cancel();
            r.ForceCts.Cancel();
        }
    }
    public async Task Start(string file, string? streamName = null)
    {
        streamName ??= "default";

        if (!File.Exists(file))
            throw new FileNotFoundException($"File: {file} does not exists.");

        var cstForce= new CancellationTokenSource();
        var cstGrace =  new CancellationTokenSource();
        _tokenCancellationSources.Add(cstForce);

        if (!File.Exists(_appName))
            throw new FileNotFoundException("cam-simulator not found");

        IEnumerable<string> args = [file, streamName];
        var cmd = CliWrap.Cli.Wrap(_appName)
            .WithArguments(args);

        _logger.LogInformation(string.Join(' ', args.Prepend(_appName)));
        var ret = cmd
            .ExecuteBufferedAsync(Encoding.UTF8,
                Encoding.UTF8,
                cstForce.Token,
                cstGrace.Token);
        _running.TryAdd(file, new RunningProc(ret, cstForce, cstGrace));
        _ = Task.Run(async () =>
        {
            try
            {
                var x = await ret;
                _running.TryRemove(file, out var _);
                _logger.LogInformation($"{_appName} exited with code: {x.ExitCode}");
                if (!string.IsNullOrWhiteSpace(x.StandardError))
                    _logger.LogError(x.StandardError);
                if (!string.IsNullOrEmpty(x.StandardOutput))
                    _logger.LogInformation(x.StandardOutput);
                _tokenCancellationSources.Remove(cstForce);
            }
            catch(OperationCanceledException ex)
            {
                _logger.LogInformation("Camera simulation canceled." + _appName);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error at capturing results about " + _appName);
            }
        });
    }

    public void Dispose()
    {
        foreach(var i in _tokenCancellationSources) 
            i.Cancel();
        _tokenCancellationSources.Clear();
    }
}
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