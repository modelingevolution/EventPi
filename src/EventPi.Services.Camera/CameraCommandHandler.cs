using System.ComponentModel;
using EventStore.Client;
using MicroPlumberd;
using MicroPlumberd.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Net;
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
    public static CameraResolution GetCameraResolution(this IConfiguration configuration) => configuration.GetValue<CameraResolution?>("CameraResolution") ?? CameraResolution.FullHd;
    public static bool GetCameraAutostart(this IConfiguration configuration) => configuration.GetValue<bool>("CameraAutostart");
    public static string GetLibCameraPath(this IConfiguration configuration) => configuration.GetValue<string>("LibCameraPath") ?? LibCameraVid.DefaultPath;

    public static IPAddress GetLibCameraListenIp(this IConfiguration configuration) =>
        IPAddress.TryParse(configuration.GetValue<string>("LibCameraListenIp"), out var p) ? p : IPAddress.Loopback;
    public static int GetLibCameraListenPort(this IConfiguration configuration) => configuration.GetValue<int?>("LibCameraListenPort") ?? 6000;
}

public class LibCameraStarter(IConfiguration configuration, ILogger<LibCameraStarter> log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if(!configuration.GetCameraAutostart()) return;

        var resolution = configuration.GetCameraResolution();
        var libCameraPath = configuration.GetLibCameraPath();
        var vid = new LibCameraVid(libCameraPath);
        if(vid.KillAll()) await Task.Delay(1000);
        var p = await vid.Start(resolution, Codec.mjpeg, configuration.GetLibCameraListenIp(), configuration.GetLibCameraListenPort());
        log.LogInformation($"libcamera-vid started, pid: {p.ProcessId}");
    }
}
public class LibCameraVid(string? appName =null)
{
    public const string DefaultPath = "/usr/bin/libcamera-vid";
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
    public async Task<CommandTask<CommandResult>> Start(CameraResolution resolution, Codec codec, IPAddress? listenAddress = null, int listenPort = 6000)
    {
        if (_runningApp != null) throw new InvalidOperationException();

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
                "-o", $"tcp://{address}:{listenPort}"
            });
        _runningApp = cmd.ExecuteAsync(_cstForce.Token, _cstGrace.Token);
        
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

        };
        await plumber.AppendState(ev, hostName);
    }


}