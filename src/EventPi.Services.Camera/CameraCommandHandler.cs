using System.ComponentModel;
using EventStore.Client;
using MicroPlumberd;
using MicroPlumberd.Services;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Net;
using EventPi.Abstractions;
using EventPi.Services.Camera.Contract;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;
using Microsoft.AspNetCore.Builder;

namespace EventPi.Services.Camera;



public static class CameraConfiguration
{

    public static Resolution GetCameraResolution(this IConfiguration configuration) => Resolution.TryParse(configuration.GetValue<string>(CameraResolutionKey), out var r) ? r : Resolution.FullHd;

    public static bool IsCameraAutostart(this IConfiguration configuration) => configuration.GetValue<bool>(CameraAutostartKey);
    public static string GetLibCameraPath(this IConfiguration configuration) => configuration.GetValue<string>(LibCameraPathKey) ?? LibCameraVid.DefaultPath;

    public static string GetCameraSimulatorPath(this IConfiguration configuration) =>
        configuration.GetValue<string>(CameraSimulatorPathKey) ?? "cam-simulator";
    public static string GetLibcameraGrpcFullListenAddress(this IConfiguration configuration, int nr = 0) => $"{configuration.GetLibCameraListenIp()}:{configuration.GetLibCameraGrpcListenPort(nr)}";

    public static int GetLibCameraCameraCount(this IConfiguration configuration) => configuration.GetValue<int?>("LibCameraCount") ?? 1;
    public static IPAddress GetLibCameraListenIp(this IConfiguration configuration) =>
        IPAddress.TryParse(configuration.GetValue<string>(LibCameraListenIpKey), out var p) ? p : IPAddress.Loopback;
    public static int GetLibCameraVideoListenPort(this IConfiguration configuration) => configuration.GetValue<int?>(LibCameraVideoListenPortKey) ?? 6000;
    public static int GetLibCameraGrpcListenPort(this IConfiguration configuration, int nr=0) => (configuration.GetValue<int?>(LibCameraGrpcListenPortKey) ?? 6500) + nr;
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

    public async Task Handle(CameraAddress addr, SetCameraParameters cmd)
    {
        if(addr.CameraNumber.HasValue) 
            await proxy.ProcessAsync(cmd, addr.CameraNumber.Value);
        else
            await proxy.ProcessAsync(cmd, -1);
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
        await plumber.AppendState(ev, addr);
    }


}