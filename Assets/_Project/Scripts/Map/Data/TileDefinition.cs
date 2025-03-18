
using System;

namespace Core.Map
{
    [Serializable]
    public struct TileDefinition
    {
        public Tile TileType;
        public TileProperties TileProperties;
        public ushort MaxHitPoints;
    }
}