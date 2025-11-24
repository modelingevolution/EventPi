using MicroPlumberd;
using ProtoBuf;
using EventPi.Abstractions;
namespace EventPi.Events.MachineWork;

[OutputStream("Device")]
[ProtoContract]
public record WorkOnMachineStopped : ICloneable<WorkOnMachineStopped>
{
    public WorkOnMachineStopped()
    {
        this.Id =  Guid.NewGuid();
    }

    [ProtoMember(1)]
    public Guid Id { get; set; }

    [ProtoMember(2)]

    public string StartedWithCard { get; set; }

    [ProtoMember(3)]

    public string DeviceSN { get; set; }

    [ProtoMember(4)]
    public DateTimeOffset? When { get; set; }

    [ProtoMember(5)]
    public TimeSpan Duration { get; set; }

    [ProtoMember(6)]

    public string SwipedWithCard { get; set; }
    WorkOnMachineStopped ICloneable<WorkOnMachineStopped>.Clone() => this with { Id = Guid.NewGuid() };
}