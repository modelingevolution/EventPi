using MicroPlumberd;
using ProtoBuf;

namespace EventPi.Events.MachineWork;

[ProtoContract]
[OutputStream("Device")]
public class StopWorkingOnMachine
{
    public StopWorkingOnMachine()
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