using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Map
{
    public record Map(MapMetadata Metadata, TileBase[] TileBases, TileBase[] FloorTileBases)
    {
        public MapMetadata Metadata { get; private set; } = Metadata;
        public TileBase[] TileBases { get; private set; } = TileBases;
        public TileBase[] FloorTileBases { get; private set; } = FloorTileBases;
    }
}