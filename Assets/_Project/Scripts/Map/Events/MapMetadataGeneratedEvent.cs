using Coimbra.Services.Events;

namespace Core.Map
{
    public readonly partial struct MapMetadataGeneratedEvent : IEvent
    {
        public readonly MapMetadata MapMetadata;

        public MapMetadataGeneratedEvent(MapMetadata mapMetadata)
        {
            MapMetadata = mapMetadata;
        }
    }
}