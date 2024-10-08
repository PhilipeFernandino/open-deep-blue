using System;
using System.Linq;
using UnityEngine;

namespace Core.ItemSystem
{
    [Serializable]
    public class Item
    {
        [field: SerializeField] public ItemSO ItemData { get; private set; }

        [field: SerializeField] public long Timestamp { get; set; }
        [field: SerializeField] public int Amount { get; set; }


        public string Name => ItemData.Name;

        public ItemRarity Rarity => ItemData.Rarity;

        public ItemCategory Category => ItemData.Category;

        public Sprite Icon => ItemData.Icon;

        public bool IsEquipable => ItemData.IsEquipable;

        public override string ToString()
        {
            return $"[{Name}] - \n" +
                $"Rarity: {Rarity}\n" +
                $"Category: {Category}\n" +
                $"IsEquipable: {IsEquipable}";
        }
    }
}
