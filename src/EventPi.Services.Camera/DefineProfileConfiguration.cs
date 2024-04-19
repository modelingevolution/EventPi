using System.ComponentModel.DataAnnotations;
using MicroPlumberd;
using ProtoBuf;

namespace EventPi.Services.Camera;

[ProtoContract]
public class DefineProfileConfiguration : IId<Guid>
{
    [ProtoMember(1)] public Guid Id { get; set; } = Guid.NewGuid();

    [ProtoMember(2)] public int Shutter { get; set; }
    [ProtoMember(3)] public double AnalogGain { get; set; }
    [ProtoMember(4)] public double DigitalGain { get; set; }
    [ProtoMember(5)] public double Brightness { get; set; }
    [ProtoMember(6)] public double Contrast { get; set; }
    [ProtoMember(7)] public double Sharpness { get; set; }
    [ProtoMember(8)] public int CameraId { get; set; }
    [ProtoMember(9)] public string Hostname{ get; set; }
    [ProtoMember(10)] public string Profile{ get; set; }
}