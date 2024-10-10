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
using Microsoft.Extensions.Logging;

namespace EventPi.Services.Camera;



public static class CameraModule
{

    public static Resolution GetCameraResolution(this IConfiguration configuration) => Resolution.TryParse(configuration.GetValue<string>(CAMERA_RESOLUTION_KEY), out var r) ? r : Resolution.FullHd;

    public static bool IsCameraAutostart(this IConfiguration configuration) => configuration.GetValue<bool>(CAMERA_AUTOSTART_KEY);
    public static string GetLibCameraPath(this IConfiguration configuration) => configuration.GetValue<string>(LIB_CAMERA_PATH_KEY) ?? LibCameraVid.DefaultPath;
    public static string GetOpenVidCamPath(this IConfiguration configuration) => configuration.GetValue<string>(OPEN_VID_CAM_PATH_KEY) ?? LibCameraVid.DefaultPath;

    public static string GetCameraSimulatorPath(this IConfiguration configuration) =>
        configuration.GetValue<string>(CAMERA_SIMULATOR_PATH_KEY) ?? "cam-simulator";
    public static string GetLibcameraGrpcFullListenAddress(this IConfiguration configuration, int nr = 0) => $"{configuration.GetLibCameraListenIp()}:{configuration.GetLibCameraGrpcListenPort(nr)}";

    public static int GetLibCameraCameraCount(this IConfiguration configuration) => configuration.GetValue<int?>("LibCameraCount") ?? 1;
    public static IPAddress GetLibCameraListenIp(this IConfiguration configuration) =>
        IPAddress.TryParse(configuration.GetValue<string>(LIB_CAMERA_LISTEN_IP_KEY), out var p) ? p : IPAddress.Loopback;
    public static int GetLibCameraVideoListenPort(this IConfiguration configuration) => configuration.GetValue<int?>(LIB_CAMERA_VIDEO_LISTEN_PORT_KEY) ?? 6000;
    public static int GetLibCameraGrpcListenPort(this IConfiguration configuration, int nr=0) => (configuration.GetValue<int?>(LIB_CAMERA_GRPC_LISTEN_PORT_KEY) ?? 6500) + nr;
    public const string CAMERA_RESOLUTION_KEY = "CameraResolution";

    public const string CAMERA_SIMULATOR_PATH_KEY = "CameraSimulatorPath";
    public const string CAMERA_AUTOSTART_KEY = "CameraAutostart";
    public const string LIB_CAMERA_PATH_KEY = "LibCameraPath";
    public const string OPEN_VID_CAM_PATH_KEY = "OpenVidCamPath";
    public const string LIB_CAMERA_LISTEN_IP_KEY = "LibCameraListenIp";
    public const string LIB_CAMERA_VIDEO_LISTEN_PORT_KEY = "LibCameraVideoListenPort";
    public const string LIB_CAMERA_GRPC_LISTEN_PORT_KEY = "LibCameraGrpcListenPort";
    public const string LIB_CAMERA_TUNING_FILE_PATH_KEY = "LibCameraTuningFilePath";
    public static string GetLibCameraTuningPath(this IConfiguration configuration) => configuration.GetValue<string>(LIB_CAMERA_TUNING_FILE_PATH_KEY) ?? LibCameraVid.DefaultTuningFilePath;
}

[CommandHandler]
public partial class CameraCommandHandler(IPlumber plumber, CameraManager manager, ILogger<CameraCommandHandler> logger)
{
    public async Task Handle(HostName hostName, SetCameraHistogramFilter cmd)
    {
        await manager.ProcessAsync(cmd);
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

        if (addr.CameraNumber.HasValue)
        {
            logger.LogInformation($"Setting specific camera parameters: {addr}.");
            await manager.ProcessAsync(cmd, addr.CameraNumber.Value);
        }
        else
        {
            logger.LogInformation($"Setting all cameras parameters: {addr}.");
            await manager.ProcessAsync(cmd, -1);

        }


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
        await plumber.AppendState(ev, addr.ToString());
    }


}