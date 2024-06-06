using System.ComponentModel;
using EventStore.Client;
using MicroPlumberd;
using MicroPlumberd.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using CliWrap;
using EventPi.Abstractions;
using EventPi.Services.Camera.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace EventPi.Services.Camera;

[JsonConverter(typeof(JsonParsableConverter<CameraResolution>))]
public readonly record struct CameraResolution(int Width, int Height) : IParsable<CameraResolution>
{
    public static readonly CameraResolution FullHd = new CameraResolution(1920, 1080);
    public override string ToString()
    {
        return $"{Width}x{Height}";
    }

    public static CameraResolution Parse(string s, IFormatProvider? provider)
    {
        var segments = s.Split('x');
        return new CameraResolution(int.Parse(segments[0]), int.Parse(segments[1]));
    }

    public static bool TryParse(string? s, out CameraResolution result) =>
        CameraResolution.TryParse(s, null, out result);

    public static bool TryParse(string? s, IFormatProvider? provider, out CameraResolution result)
    {
        if(s == null)         {
            result = default;
            return false;
        }
        var segments = s.Split('x');
        if(segments.Length != 2)
        {
            result = default;
            return false;
        }
        if(int.TryParse(segments[0], out var w) && int.TryParse(segments[1], out var h))
        {
            result = new CameraResolution(w, h);
            return true;
        }
        result = default;
        return false;
    }
}

public static class RpiCameraResolutions
{
    public static readonly CameraResolution V2FullHD = CameraResolution.FullHd;
    public static readonly CameraResolution RpiGlobalShutter = new CameraResolution(1456, 1088);
}

public enum Codec
{
    mjpeg
}

public static class ConfigurationExtensions
{
    public static CameraResolution GetCameraResolution(this IConfiguration configuration) => CameraResolution.TryParse(configuration.GetValue<string>("CameraResolution"), out var r) ? r : CameraResolution.FullHd;

    public static bool GetCameraAutostart(this IConfiguration configuration) => configuration.GetValue<bool>("CameraAutostart");
    public static string GetLibCameraPath(this IConfiguration configuration) => configuration.GetValue<string>("LibCameraPath") ?? LibCameraVid.DefaultPath;

    public static Uri GetLibcameraFullListenAddress(this IConfiguration configuration) => new Uri($"http://{configuration.GetLibCameraListenIp()}:{configuration.GetLibCameraListenPort()}");
    public static IPAddress GetLibCameraListenIp(this IConfiguration configuration) =>
        IPAddress.TryParse(configuration.GetValue<string>("LibCameraListenIp"), out var p) ? p : IPAddress.Loopback;
    public static int GetLibCameraListenPort(this IConfiguration configuration) => configuration.GetValue<int?>("LibCameraListenPort") ?? 6000;

    public static string GetLibCameraTuningPath(this IConfiguration configuration) => configuration.GetValue<string>("LibCameraTuningFilePath") ?? LibCameraVid.DefaultTuningFilePath;
}

public class LibCameraStarter(IConfiguration configuration, ILogger<LibCameraStarter> log, ILogger<LibCameraVid> logLibCameraVid, string grpcClientAddress) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if(!configuration.GetCameraAutostart()) return;
        
        var resolution = configuration.GetCameraResolution();
        var libCameraPath = configuration.GetLibCameraPath();
        var vid = new LibCameraVid(logLibCameraVid, libCameraPath);
        if(vid.KillAll()) await Task.Delay(1000);
        var p = await vid.Start(resolution, Codec.mjpeg, configuration.GetLibCameraTuningPath(), configuration.GetLibCameraListenIp(), configuration.GetLibCameraListenPort(), grpcClientAddress);
        log.LogInformation($"libcamera-vid started, pid: {p.ProcessId}");
    }
}
public class LibCameraVid(ILogger<LibCameraVid> logger, string? appName =null)
{
    public const string DefaultPath = "/usr/local/bin/rocketwelder-vid";
    public const string DefaultTuningFilePath = "/app/imx296.json";
    private readonly string _appName = appName ?? DefaultPath;
    private CommandTask<CommandResult>? _runningApp;
    private CancellationTokenSource? _cstForce;
    private CancellationTokenSource? _cstGrace;
    public bool IsRunning => _runningApp != null;
    public async Task Stop()
    {
        await _cstGrace.CancelAsync();
        await _runningApp.Task;
        _runningApp.Dispose();
        _runningApp = null;
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
    public async Task<CommandTask<CommandResult>> Start(CameraResolution resolution, Codec codec, string tuningFilePath, IPAddress? listenAddress = null, int listenPort = 6000, string grpcClientAddress = "127.0.0.1:8080")
    {
        if (_runningApp != null) throw new InvalidOperationException();
        if(!File.Exists(tuningFilePath)) throw new FileNotFoundException($"Tuning file not found at {tuningFilePath} !");

        _cstForce = new CancellationTokenSource();
        _cstGrace = new CancellationTokenSource();

        var name = Path.GetFileName(_appName);
        foreach(var i in  Process.GetProcessesByName(name))
            i.Kill();

        var address = listenAddress ?? IPAddress.Loopback;
        var cmd=  CliWrap.Cli.Wrap(_appName)
            .WithArguments(new string[]
            {
                "-t", "0", 
                "--width", resolution.Width.ToString(), 
                "--height", resolution.Height.ToString(), 
                "--codec", codec.ToString(), 
                "--inline", "--listen",
                "--awbgains","-1,-1", 
                "--metering","spot",
                "--frame-counter","1",
                "--tuning-file",tuningFilePath,
                "--saturation","0.0",
                "--grpc-client-address", grpcClientAddress,
                "-o", $"tcp://{address}:{listenPort}"
            });
        StringBuilder sb = new StringBuilder();
        StringBuilder err = new StringBuilder();
        _runningApp = cmd
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(sb))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(err))
            .ExecuteAsync(_cstForce.Token, _cstGrace.Token);
        _ = Task.Run(async () =>
        {
            var x = await _runningApp;
            logger.LogInformation($"{_appName} exited with code: {x.ExitCode}");
            if (!string.IsNullOrWhiteSpace(err.ToString()))
                logger.LogError(err.ToString());
            if (!string.IsNullOrEmpty(sb.ToString()))
                logger.LogInformation(sb.ToString());

        });
        
        
        return _runningApp;
    }
}

[CommandHandler]
public partial class CameraCommandHandler(IPlumber plumber, GrpcCppCameraProxy proxy)
{
    public async Task Handle(HostName hostName, SetCameraHistogramFilter cmd)
    {
        await proxy.ProcessAsync(cmd);
        var state = new CameraHistogramFilter()
        {
            Values = cmd.Values
        };
        await plumber.AppendState(state, hostName);
    }
    public async Task Handle(HostProfilePath hostProfilePath, DefineProfileCameraHistogramFilter cmd)
    {
        var state = new CameraProfileHistogramFilter()
        {
            Points = cmd.Points
        };
        await plumber.AppendState(state, hostProfilePath);
    }
    public async Task Handle(HostProfilePath hostProfilePath, DefineProfileCameraParameters cmd)
    {
        var ev = new CameraProfile()
        {
            Brightness = cmd.Brightness,
            Contrast = cmd.Contrast,
            Shutter = cmd.Shutter,
            Sharpness = cmd.Sharpness,
            DigitalGain = cmd.DigitalGain,
            AnalogueGain = cmd.AnalogueGain,
            BlueGain = cmd.BlueGain,
            RedGain = cmd.RedGain,
            ExposureLevel = cmd.ExposureLevel,
            HdrMode = cmd.HdrMode,
            AutoHistogramEnabled = cmd.AutoHistogramEnabled
        };
        
        await plumber.AppendState(ev, hostProfilePath);
    }

    public async Task Handle(HostName hostName, SetCameraParameters cmd)
    {
        await proxy.ProcessAsync(cmd);
        var ev = new CameraParametersState()
        {
            Brightness = cmd.Brightness,
            Contrast = cmd.Contrast,
            Shutter = cmd.Shutter,
            Sharpness = cmd.Sharpness,
            DigitalGain = cmd.DigitalGain,
            AnalogueGain = cmd.AnalogueGain,
            BlueGain = cmd.BlueGain,
            RedGain = cmd.RedGain,
            ExposureLevel = cmd.ExposureLevel,
            HdrMode = cmd.HdrMode,
            AutoHistogramEnabled = cmd.AutoHistogramEnabled
        };
        await plumber.AppendState(ev, hostName);
    }


}