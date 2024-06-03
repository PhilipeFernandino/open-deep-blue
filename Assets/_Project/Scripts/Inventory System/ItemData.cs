using System;
using UnityEngine;

namespace Core.InventorySystem
{
    [CreateAssetMenu(menuName = "Item System/Item Data")]
    public class ItemData : ScriptableObject, IEquatable<ItemData>
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public int Rarity { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }

        [field: SerializeField] public int MaxStack { get; private set; }

        [field: SerializeField] public bool CanSell { get; private set; }
        [field: SerializeField] public int SellPrice { get; private set; }

        [field: SerializeField] public Equipment HoldableItem { get; private set; }

        public bool IsStackable => MaxStack > 1;


        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Rarity);
        }

        public bool Equals(ItemData other)
        {
            return
                other != null
                && GetType() == other.GetType()
                && Name == other.Name
                && Rarity == other.Rarity;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ItemData);
        }

    }
}
