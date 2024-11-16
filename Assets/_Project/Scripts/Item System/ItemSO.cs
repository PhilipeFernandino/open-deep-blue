using System;
using System.Linq;
using UnityEngine;

namespace Core.ItemSystem
{
    [CreateAssetMenu(menuName = "Core/Item")]
    public class ItemSO : ScriptableObject, IEquatable<ItemSO>
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

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Rarity);
        }

        public bool Equals(ItemSO other)
        {
            return
                other != null
                && GetType() == other.GetType()
                && Name == other.Name
                && Rarity == other.Rarity;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ItemSO);
        }

    }
}
