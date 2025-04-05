using System;

namespace Core.Map
{
    public struct TileInstance : IEquatable<TileInstance>
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

        public static bool operator ==(TileInstance a, TileInstance b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(TileInstance a, TileInstance b)
        {
            return !Equals(a, b);
        }

        public override string ToString()
        {
            return $"({nameof(TileType)} = {TileType}, {nameof(CurrentHitPoints)}: {CurrentHitPoints})";
        }

        public bool Equals(TileInstance other)
        {
            return GetType() == other.GetType()
                && TileType == other.TileType
                && CurrentHitPoints == other.CurrentHitPoints;
        }
    }
}