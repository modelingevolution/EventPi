namespace EventPi.Services.Camera;

public record CameraConfigurationProfileApplied
{
    public string ProfileName { get; set; }
}

// SetCameraParameters -> CommandHandler ->grpc do kamery / jesli ok to -> CameraParametersApplied - to jest event
//                                                        / jesli nie to -> CameraConfigurationFailed - to jest FaultException od typu - TO NIE EVENT