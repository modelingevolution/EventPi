using System.ComponentModel.DataAnnotations;
using MicroPlumberd;
using ProtoBuf;

namespace EventPi.Services.Camera;

[ProtoContract]
public class SetCameraParameters : IId<Guid>
{
   
    [ProtoMember(1)]
    
    public Guid Id { get; set; } = Guid.NewGuid();

    [ProtoMember(2)]
    [Range(1,40000)]
    public int Shutter { get; set; }

    [ProtoMember(3)]
    [Range(0, 10)]
    public float AnologueGain { get; set; }

    [ProtoMember(4)]
    [Range(0, 10)]
    public float DigitalGain { get; set; }

    [ProtoMember(5)]
    [Range(0, 10)]
    public float Contrast { get; set; }

    [ProtoMember(6)]
    [Range(0, 1)]
    public float Sharpness { get; set; }

    [ProtoMember(7)]
    [Range(-1,1)]
    public float Brightness { get; set; }

    [ProtoMember(8)]
    [Range(-1, 10)]
    public float BlueGain { get; set; }

    [ProtoMember(9)]
    [Range(-1, 10)]
    public float RedGain { get; set; }
}