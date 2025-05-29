using System;
using System.Linq;

namespace Core.Map
{
    [Flags]
    public enum TileProperties
    {
        None = 0,
        IsMinerable = 1 << 0,
        IsDestructable = 1 << 1,
        IsLootDropable = 1 << 2,
        IsBossSpawn = 1 << 3,
        IsChest = 1 << 4,
        IsInteractable = 1 << 5,
    }
}