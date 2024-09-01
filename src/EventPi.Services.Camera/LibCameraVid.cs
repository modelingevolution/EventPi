﻿using System.Diagnostics;
using System.Net;
using System.Text;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace EventPi.Services.Camera;

public class LibCameraVid(ILogger<LibCameraVid> logger, string? appName =null)
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
        string grpcListenAddress = "127.0.0.1:6500", string shmName = "default", int? cameraNr = null)
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
        if (cameraNr.HasValue && cameraNr.Value > 0)
            args.AddRange(["--camera", cameraNr.Value.ToString()]);

        if (transport == VideoTransport.Shm)
            args.AddRange(["--shm", shmName]);
        else
            args.AddRange(["--listen", "-o", $"tcp://{address}:{listenPort}" ]);
        
        var cmd=  CliWrap.Cli.Wrap(_appName)
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