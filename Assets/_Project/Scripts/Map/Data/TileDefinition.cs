
using System;

namespace Core.Map
{
    [Serializable]
    public struct TileDefinition
    {
        public Tile TileType;
        public TileProperties TileProperties;
        public ushort MaxHitPoints;

        public readonly bool IsWalkable => TileProperties.HasFlag(TileProperties.IsWalkable);
        public readonly bool IsDestructable => TileProperties.HasFlag(TileProperties.IsDestructable);
        public readonly bool IsEventTrigger => TileProperties.HasFlag(TileProperties.IsEventTrigger);
        public readonly bool IsDynamic => TileProperties.HasFlag(TileProperties.IsDynamic);
        public readonly bool IsInteractable => TileProperties.HasFlag(TileProperties.IsInteractable);

        public override string ToString()
        {
            return $"({nameof(TileType)} = {TileType}, {nameof(TileProperties)}: {TileProperties}, {nameof(MaxHitPoints)}: {MaxHitPoints})";
        }
    }
}