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
            var hash = HashCode.Combine(Name, Rarity);
            return hash;
        }

        public bool Equals(ItemSO other)
        {
            var isEqual = other != null
                && GetType() == other.GetType()
                && Name == other.Name
                && Rarity == other.Rarity
                && Category == other.Category
                && IsEquipable == other.IsEquipable;

            return isEqual;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ItemSO);
        }

        public override string ToString()
        {
            return $"(Name = {Name}, Rarity = {Rarity}, Category = {Category}, IsEquippable = {IsEquipable})";
        }
    }
}
