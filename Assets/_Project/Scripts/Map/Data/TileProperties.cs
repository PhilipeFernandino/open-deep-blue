using System;
using System.Linq;

namespace Core.Map
{
    [Flags]
    public enum TileProperties
    {
        None = 0,
        IsDestructable = 1 << 1,
        IsLootDropable = 1 << 2,
        IsEventTrigger = 1 << 3,
        IsDynamic = 1 << 4,
        IsInteractable = 1 << 5,
        IsWalkable = 1 << 6
    }
}