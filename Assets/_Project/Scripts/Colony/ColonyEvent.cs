using System;
using Coimbra.Services.Events;

namespace Core.Train
{
    public partial struct ColonyEvent : IEvent
    {
        public ColonyEventType EventType;

        public ColonyEvent(ColonyEventType eventType)
        {
            EventType = eventType;
        }
    }
}