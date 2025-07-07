using Coimbra.Services.Events;

namespace Core.RL
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