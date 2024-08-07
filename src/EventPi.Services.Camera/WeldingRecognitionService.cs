﻿using EventPi.Abstractions;
using EventPi.Pid;
using EventPi.Services.Camera.Contract;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using ModelingEvolution.VideoStreaming;
using System.Drawing;
using System.Diagnostics;

namespace EventPi.Services.Camera;

public class WeldingRecognitionService : IPartialYuvFrameHandler, IDisposable
{
    private readonly ILogger<WeldingRecognitionService> _logger;
    private readonly CancellationTokenSource _cts;
    private readonly GrpcCppCameraProxy _proxy;
    private readonly Channel<SetCameraParameters> _channel;
    private readonly CircularBuffer<int> _bufferBrightPixels;
    private readonly CircularBuffer<int> _bufferDarkPixels;
    
    private readonly WeldingRecognitionModel _model;
    private readonly WeldingRecognitionProvider _profileProvider;

    public bool IsWelding { get; private set; }

    public int BrightOffset { get; set; }
    public int DarkOffset { get;  set; }


    public ICameraParametersReadOnly CurrentAppliedProfile { get; private set; }


    public int Every => 1;



    public WeldingRecognitionService(WeldingRecognitionProvider profileProvider,  ILogger<WeldingRecognitionService> logger,GrpcCppCameraProxy proxy,  WeldingRecognitionModel model, CameraProfileConfigurationModel cameraModel)
    {
        CurrentAppliedProfile = new CameraProfile();
        _cts = new CancellationTokenSource();
        _logger = logger;
        _model = model;
        _profileProvider = profileProvider;

        _proxy = proxy;
        

        _bufferBrightPixels = new CircularBuffer<int>(3);
        _bufferDarkPixels = new CircularBuffer<int>(3);
        _channel = Channel.CreateBounded<SetCameraParameters>(new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropOldest });
        _ = Task.Factory.StartNew(OnSendCommand, TaskCreationOptions.LongRunning);
       
    }

    private async Task OnSendCommand()
    {
        try
        {
            while (!_cts.IsCancellationRequested)
            {
                var cmd = await _channel.Reader.ReadAsync(_cts.Token);
                await _proxy.ProcessAsync(cmd);
            }
        }
        catch (OperationCanceledException) { }
    }

   
    private DarkBrightPixels _count0;
    private DarkBrightPixels _count1;
    private readonly Rectangle _area;
    private volatile int isRunning = 0;

    public void Handle(YuvFrame frame, YuvFrame? prv, ulong seq, CancellationToken token, object st)
    {
        
        // Making sure we are not processing in parallel.
        if(Interlocked.Increment(ref isRunning) > 1)
        {
            Interlocked.Decrement(ref isRunning);
            Debug.WriteLine("Skipping welding profile detection");
            return;
        }

        Rectangle area = new Rectangle(0, 0, frame.Info.Width, frame.Info.Height);
        area.Inflate(-400, -400);

        var areaSizeInPixels = area.Size.Width * area.Size.Height;
         
        var count = frame.CountPixelsOutsideRange(20, 200, area);

        if(prv == null)
        {
            _count0 = count;
            Interlocked.Decrement(ref isRunning);
            return;
        }
        
        if ((frame.Metadata.FrameNumber & 0x1UL) == 1UL)
            _count1 = count; // it this is frame nr. 1,3,5,7 ...
        else _count0 = count; // for frame nr. 2,4,6, ...

        var px = DarkBrightPixels.Min(_count0, _count1);
        //Debug.WriteLine(px);

        _bufferBrightPixels.AddLast(px.BrightPixels);
        _bufferDarkPixels.AddLast(px.DarkPixels);

        if (!_model.DetectionEnabled)
        {
            Interlocked.Decrement(ref isRunning);
            return;
        }

        if (!IsWelding)
        {
            if (_bufferBrightPixels.Average() > _model.WeldingBound*0.01*areaSizeInPixels)
            {
                _logger.LogInformation("Welding detected");
                IsWelding = true;

                var camParams = new SetCameraParameters();
                camParams.CopyFrom(_profileProvider.Welding.Profile);
                CurrentAppliedProfile = camParams;
                _channel.Writer.TryWrite(camParams);
            }
        }
        else
        {
            // It must be welding
            if (_bufferDarkPixels.Average() > _model.NonWeldingBound*0.01*areaSizeInPixels)
            {
                _logger.LogInformation("Welding not detected");
                _logger.LogInformation($"OnDetectWelding: {px}");
                IsWelding = false;

                var camParams = new SetCameraParameters();
                camParams.CopyFrom(_profileProvider.Default.Profile);
                CurrentAppliedProfile = camParams;
                _channel.Writer.TryWrite(camParams);
            }
        }
        Interlocked.Decrement(ref isRunning);
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
public static class YuvFrameWeldingRecognitionUtils
{
    public static unsafe DarkBrightPixels CountPixelsOutsideRange(this YuvFrame frame, int low, int high)
    {
        int darkPixels = 0;
        int brightPixels = 0;
        var end = frame.Data + frame.Info.Width*frame.Info.Height;
        for (byte* p = frame.Data; p != end; p += 1)
        {
            var value = *p;
            if (value > high)
                brightPixels++;
            
            if (value < low)
                darkPixels++;
        }
        return new DarkBrightPixels(darkPixels, brightPixels);
    }
    public static unsafe DarkBrightPixels CountPixelsOutsideRange(this YuvFrame frame, int low, int high, Rectangle r)
    {
        int darkPixels = 0;
        int brightPixels = 0;

        for (int ix = 0; ix < r.Width; ix++)
            for (int iy = 0; iy < r.Height; iy++)
            {
                var value = frame.Y(ix + r.X, iy + r.Y);

                if (value > high)
                    brightPixels++;

                if (value < low)
                    darkPixels++;
            }
        return new DarkBrightPixels(darkPixels, brightPixels);
    }
}
public readonly record struct DarkBrightPixels(int DarkPixels, int BrightPixels)
{
    public static DarkBrightPixels Min(DarkBrightPixels a, DarkBrightPixels b)
    {
        return new DarkBrightPixels(Math.Min(a.DarkPixels, b.DarkPixels), Math.Min(a.BrightPixels, b.BrightPixels));
    }
}