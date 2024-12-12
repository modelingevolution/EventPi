using System.Diagnostics;
using System.Net;
using System.Security;
using System.Text;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;
using ModelingEvolution.VideoStreaming;

namespace EventPi.Services.Camera;

interface IProcessUtils
{
    int GetParentProcessId(int pid);
}
static class ProcessUtils
{
    public static IProcessUtils Utils { get; private set; }
    static ProcessUtils()
    {
#if WINDOWS
        Utils = new WinProcessUtils();
#else
        Utils = new LinuxProcessUtils();
#endif

    }
}
class LinuxProcessUtils : IProcessUtils
{
    public int GetParentProcessId(int pid)
    {

        string statFilePath = $"/proc/{pid}/stat";

        if (!File.Exists(statFilePath))
        {
            throw new FileNotFoundException($"Stat file for process {pid} does not exist.");
        }

        string statFileContent = File.ReadAllText(statFilePath);
        string[] statFileParts = statFileContent.Split(' ');

        // The 4th field is the parent process ID (PPID)
        int parentProcessId = int.Parse(statFileParts[3]);
        return parentProcessId;
    }
}

public interface ILibCameraVidProcess
{
    Task Stop(int cameraNr = 0);
    Task<int> Start(Resolution resolution, VideoCodec codec, string tuningFilePath,
        VideoTransport transport, 
        IPAddress? listenAddress = null, 
        int listenPort = 6000,
        string grpcListenAddress = "127.0.0.1:6500", 
        string shmName = "default", 
        int? cameraNr = null);

}

public class LibCameraVidProcess(ILogger<LibCameraVidProcess> logger, string? appName =null) : ILibCameraVidProcess
{
    public const string DefaultPath = "/usr/local/bin/rocketwelder-vid";
    public const string DefaultTuningFilePath = "/app/imx296.json";
    private readonly string _appName = appName ?? DefaultPath;
    private CommandTask<BufferedCommandResult>? _runningApp;
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

    public bool KillAliens()
    {
        var name = Path.GetFileName(_appName);
        bool killed = false;
        var cPid = Process.GetCurrentProcess().Id;
        foreach (var i in Process.GetProcessesByName(name))
        {
            if(ProcessUtils.Utils.GetParentProcessId(i.Id) != cPid)
                i.Kill();
            killed = true;
        }
        return killed;
    }

    record ProcSlot(CancellationTokenSource Force, CancellationTokenSource Grace, int PID);
    private ProcSlot?[] CameraProcMap = new ProcSlot[16];
    public Task Stop(int cameraNr = 0)
    {
        var slot = CameraProcMap[cameraNr];
        if (slot == null) return Task.CompletedTask;
        
        try { slot.Grace.Cancel(); } catch { }
        try { slot.Force.Cancel(); } catch { }

        CameraProcMap[cameraNr] = null!;
        return Task.CompletedTask;
    }

    public async Task<int> Start(Resolution resolution, VideoCodec codec, string tuningFilePath,
        VideoTransport transport, 
        IPAddress? listenAddress = null, 
        int listenPort = 6000,
        string grpcListenAddress = "127.0.0.1:6500", 
        string shmName = "default", 
        int? cameraNr = null)
    {
        if (_runningApp != null) throw new InvalidOperationException();
        if(!File.Exists(tuningFilePath)) throw new FileNotFoundException($"Tuning file not found at {tuningFilePath} !");

        _cstForce = new CancellationTokenSource();
        _cstGrace = new CancellationTokenSource();

        
        var args = Args(resolution, codec, tuningFilePath, transport, listenAddress, listenPort, grpcListenAddress, shmName, cameraNr);

        var cmd=  CliWrap.Cli.Wrap(_appName)
            .WithArguments(args);
       
        logger.LogInformation(string.Join(' ', args.Prepend(_appName)));
        _runningApp = cmd
            .ExecuteBufferedAsync(Encoding.UTF8, 
                Encoding.UTF8,
                _cstForce.Token, 
                _cstGrace.Token);
        
        CameraProcMap[cameraNr ?? 0] = new ProcSlot(_cstForce,_cstGrace, _runningApp.ProcessId);
        _ = Task.Run(async () =>
        {
            try
            {
                var x = await _runningApp;
                
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
            CameraProcMap[cameraNr ?? 0] = null!;
        });
        

        return _runningApp.ProcessId;
    }

    public static string[] Args(Resolution resolution, 
        VideoCodec codec, 
        string tuningFilePath, 
        VideoTransport transport,
        IPAddress? listenAddress = null, 
        int listenPort = 6000, 
        string grpcListenAddress = "127.0.0.1:6500",
        string shmName = "default",
        int? cameraNr = null)
    {
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
        if (cameraNr.HasValue && cameraNr.Value > 0)
            args.AddRange(["--camera", cameraNr.Value.ToString()]);

        if (transport.HasFlag(VideoTransport.Shm))
            args.AddRange(["--shm", shmName]);
        else
            args.AddRange(["--listen", "-o", $"tcp://{address}:{listenPort}" ]);
        return args.ToArray();
    }
}
