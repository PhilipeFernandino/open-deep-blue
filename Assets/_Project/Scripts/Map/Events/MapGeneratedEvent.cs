using Coimbra.Services.Events;

namespace Core.Map
{
    public readonly partial struct MapGeneratedEvent : IEvent
    {
        public readonly Map Map;

        public MapGeneratedEvent(Map map)
        {
            Map = map;
        }
    }
}