
using System;

namespace Core.Map
{
    [Serializable]
    public struct TileDefinition
    {
        public Tile TileType;
        public TileProperties TileProperties;
        public ushort MaxHitPoints;

        public override string ToString()
        {
            return $"({nameof(TileType)} = {TileType}, {nameof(TileProperties)}: {TileProperties}, {nameof(MaxHitPoints)}: {MaxHitPoints})";
        }
    }
}