using System;
using System.Linq;

namespace Core.Map
{
    public struct TileInstance
    {
        public Tile TileType;
        public ushort CurrentHitPoints;

        public static TileInstance None => new(Tile.None, 0);

        public TileInstance(Tile tileType, ushort currentHitPoints)
        {
            TileType = tileType;
            CurrentHitPoints = currentHitPoints;
        }

        public static explicit operator TileInstance(TileDefinition tile)
        {
            return new TileInstance(tile.TileType, tile.MaxHitPoints);
        }

        public override string ToString()
        {
            return $"({nameof(TileType)} = {TileType}, {nameof(CurrentHitPoints)}: {CurrentHitPoints})";
        }
    }
}