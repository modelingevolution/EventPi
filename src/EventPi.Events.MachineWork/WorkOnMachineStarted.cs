﻿using ModelingEvolution.Plumberd;
using ModelingEvolution.Plumberd.EventStore;
using ProtoBuf;

namespace EventPi.Events.MachineWork;

[Stream("Device")]
[ProtoContract]
public class WorkOnMachineStarted : IEvent
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