using ModelingEvolution.Plumberd;
using ModelingEvolution.Plumberd.EventStore;
using ProtoBuf;

namespace EventPi.Events.MachineWork;

[Stream("Device")]
[ProtoContract]
public class WorkOnMachineStopped : IEvent
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
}