using System.Diagnostics;
using System.Text;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace EventPi.Services.Camera;

public class OpenVidCam(ILogger<OpenVidCam> logger, string _appName)
{
    private CommandTask<BufferedCommandResult>? _runningApp;
    private CancellationTokenSource? _cstForce;
    private CancellationTokenSource? _cstGrace;
    public bool KillAliens()
    {
        var name = Path.GetFileName(_appName);
        bool killed = false;
        var cPid = Process.GetCurrentProcess().Id;
        foreach (var i in Process.GetProcessesByName(name))
        {
            if (ProcessUtils.Utils.GetParentProcessId(i.Id) != cPid)
                i.Kill();
            killed = true;
        }
        return killed;
    }
    public async Task<int> Start(Resolution resolution, 
        string shmName = "default",
        int? cameraNr = null)
    {
        if (_runningApp != null) throw new InvalidOperationException();

        _cstForce = new CancellationTokenSource();
        _cstGrace = new CancellationTokenSource();


        List<string> args = new List<string>
        {
            $"--Camera={(cameraNr ?? 0)}",
            $"--Width={resolution.Width}",
            $"--Height={resolution.Height}",
            $"--StreamName={shmName}"
        };
        
        var cmd = CliWrap.Cli.Wrap(_appName)
            .WithArguments(args);

        logger.LogInformation(string.Join(' ', args.Prepend(_appName)));
        _runningApp = cmd
            .ExecuteBufferedAsync(Encoding.UTF8,
                Encoding.UTF8,
                _cstForce.Token,
                _cstGrace.Token);
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
        });


        return _runningApp.ProcessId;
    }
}