using Coimbra.Services.Events;
using Core.Units;

namespace Core.Train
{
    public readonly partial struct AntEvent : IEvent
    {
        public readonly AntEventType AntEventType;
        public readonly Ant Ant;

        public AntEvent(AntEventType antEventType, Ant ant)
        {
            AntEventType = antEventType;
            Ant = ant;
        }
    }
}