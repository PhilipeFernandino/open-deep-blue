using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Map
{
    public record Map(MapMetadata Metadata, Tilemap Tilemap)
    {
        public MapMetadata Metadata { get; private set; } = Metadata;
        public Tilemap Tilemap { get; private set; } = Tilemap;
    }
}