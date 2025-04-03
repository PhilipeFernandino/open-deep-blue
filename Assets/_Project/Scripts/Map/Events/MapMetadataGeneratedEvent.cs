using Coimbra.Services.Events;

namespace Core.Map
{
    public readonly partial struct MapMetadataGeneratedEvent : IEvent
    {
        public readonly MapMetadata Map;

        public MapMetadataGeneratedEvent(MapMetadata map)
        {
            MapMetadata = map;
        }
    }
}