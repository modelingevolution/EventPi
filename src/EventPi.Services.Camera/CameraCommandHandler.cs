using System.ComponentModel;
using EventStore.Client;
using MicroPlumberd;
using MicroPlumberd.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Text;
using CliWrap;
using EventPi.Abstractions;
using EventPi.Services.Camera.Contract;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace EventPi.Services.Camera;



public static class CameraConfiguration
{

    public static Resolution GetCameraResolution(this IConfiguration configuration) => Resolution.TryParse(configuration.GetValue<string>(CameraResolutionKey), out var r) ? r : Resolution.FullHd;

    public static bool IsCameraAutostart(this IConfiguration configuration) => configuration.GetValue<bool>(CameraAutostartKey);
    public static string GetLibCameraPath(this IConfiguration configuration) => configuration.GetValue<string>(LibCameraPathKey) ?? LibCameraVid.DefaultPath;

    public static string GetCameraSimulatorPath(this IConfiguration configuration) =>
        configuration.GetValue<string>(CameraSimulatorPathKey) ?? "cam-simulator";
    public static string GetLibcameraGrpcFullListenAddress(this IConfiguration configuration) => $"{configuration.GetLibCameraListenIp()}:{configuration.GetLibCameraGrpcListenPort()}";
    public static IPAddress GetLibCameraListenIp(this IConfiguration configuration) =>
        IPAddress.TryParse(configuration.GetValue<string>(LibCameraListenIpKey), out var p) ? p : IPAddress.Loopback;
    public static int GetLibCameraVideoListenPort(this IConfiguration configuration) => configuration.GetValue<int?>(LibCameraVideoListenPortKey) ?? 6000;
    public static int GetLibCameraGrpcListenPort(this IConfiguration configuration) => configuration.GetValue<int?>(LibCameraGrpcListenPortKey) ?? 6500;
    public const string CameraResolutionKey = "CameraResolution";

    public const string CameraSimulatorPathKey = "CameraSimulatorPath";
    public const string CameraAutostartKey = "CameraAutostart";
    public const string LibCameraPathKey = "LibCameraPath";
    public const string LibCameraListenIpKey = "LibCameraListenIp";
    public const string LibCameraVideoListenPortKey = "LibCameraVideoListenPort";
    public const string LibCameraGrpcListenPortKey = "LibCameraGrpcListenPort";
    public const string LibCameraTuningFilePathKey = "LibCameraTuningFilePath";
    public static string GetLibCameraTuningPath(this IConfiguration configuration) => configuration.GetValue<string>(LibCameraTuningFilePathKey) ?? LibCameraVid.DefaultTuningFilePath;
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
    public async Task<int> Start(Resolution resolution, VideoCodec codec, string tuningFilePath,
        VideoTransport transport, IPAddress? listenAddress = null, int listenPort = 6000,
        string grpcListenAddress = "127.0.0.1:6500", string shmName = "default")
    {
        if (_runningApp != null) throw new InvalidOperationException();
        if(!File.Exists(tuningFilePath)) throw new FileNotFoundException($"Tuning file not found at {tuningFilePath} !");

        _cstForce = new CancellationTokenSource();
        _cstGrace = new CancellationTokenSource();

        
        var address = listenAddress ?? IPAddress.Loopback;
        List<string> args = new List<string>
        {
            "-t", "0", 
            "--width", resolution.Width.ToString(), 
            "--height", resolution.Height.ToString(),
            "--codec", codec == VideoCodec.Mjpeg ? "yuv420" : "h264",
            "--inline", 
            "--awbgains","-1,-1",
            "--info-text","\"\"",
            "--bind-listen-port",grpcListenAddress,
            "--metering","spot",
            "--tuning-file",tuningFilePath,
            "--saturation","0.0",
            "-g", "25"
        };
        if (transport == VideoTransport.Shm)
            args.AddRange(["--shm", shmName]);
        else
            args.AddRange(["--listen", "-o", $"tcp://{address}:{listenPort}" ]);
        
        var cmd=  CliWrap.Cli.Wrap(_appName)
            .WithArguments(args);
       
        logger.LogInformation(string.Join(' ', args.Prepend(_appName)));
        var ret = cmd
            .ExecuteBufferedAsync(Encoding.UTF8, 
                Encoding.UTF8,
                _cstForce.Token, 
                _cstGrace.Token);
        _ = Task.Run(async () =>
        {
            try
            {
                var x = await ret;
                logger.LogInformation($"{_appName} exited with code: {x.ExitCode}");
                if (!string.IsNullOrWhiteSpace(x.StandardError))
                    logger.LogError(x.StandardError);
                if (!string.IsNullOrEmpty(x.StandardOutput))
                    logger.LogInformation(x.StandardOutput);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error at capturing results about " + _appName);
            }
        });
        
        
        return ret.ProcessId;
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