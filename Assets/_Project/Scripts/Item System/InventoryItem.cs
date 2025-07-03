using System;
using System.Linq;
using UnityEngine;

namespace Core.ItemSystem
{
    [Serializable]
    public record InventoryItem(ItemSO ItemData, long Timestamp, int Amount, bool IsFavorite)
    {
        [field: SerializeField] public ItemSO ItemData { get; private set; } = ItemData;

        [field: SerializeField] public long Timestamp { get; set; } = Timestamp;
        [field: SerializeField] public int Amount { get; set; } = Amount;
        [field: SerializeField] public bool IsFavorite { get; set; } = IsFavorite;

        public string Name => ItemData.Name;

        public ItemRarity Rarity => ItemData.Rarity;

        public ItemCategory Category => ItemData.Category;

        public Sprite Icon => ItemData.Icon;

        public bool IsEquipable => ItemData.IsEquipable;
    }
}
