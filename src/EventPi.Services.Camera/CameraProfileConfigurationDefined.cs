using MicroPlumberd;

namespace EventPi.Services.Camera;

[OutputStream("CameraConfiguration")]
public record CameraProfileConfigurationDefined
{
public string Hostname{ get; set; }
public int Shutter { get; set; }
public double AnalogGain { get; set; }
public double DigitalGain { get; set; }
public double Brightness { get; set; }
public double Contrast { get; set; }
public double Sharpness { get; set; }
public int CameraId { get; set; }
public string Profile { get; set; }
}