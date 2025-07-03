using Coimbra.Services.Events;
using System;

namespace Core.Train
{
    public readonly partial struct ColonyEvent : IEvent
    {
        public readonly ColonyEventType EventType;

        public ColonyEvent(ColonyEventType eventType)
        {
            EventType = eventType;
        }
    }
}