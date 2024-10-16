using System;
using System.Linq;
using UnityEngine;

namespace Core.ItemSystem
{
    [CreateAssetMenu(menuName = "Core/Item")]
    public class ItemSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public ItemRarity Rarity { get; private set; }
        [field: SerializeField] public ItemCategory Category { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public bool IsEquipable { get; private set; }

        public void Setup(string name, ItemRarity rarity, ItemCategory category, Sprite icon, bool isEquipable)
        {
            Name = name;
            Rarity = rarity;
            Category = category;
            Icon = icon;
            IsEquipable = isEquipable;
        }
    }
}
