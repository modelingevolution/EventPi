using MicroPlumberd;
using ProtoBuf;

namespace EventPi.Events.MachineWork;

[OutputStream("Device")]
[ProtoContract]
public record WorkOnMachineStarted
{
    public WorkOnMachineStarted()
    {
        this.Id =  Guid.NewGuid();
    }

    [ProtoMember(1)]
    public Guid Id { get; set; }

    [ProtoMember(2)]

    public string CardNumber { get; set; }

    [ProtoMember(3)]

    public string DeviceSN { get; set; }
}