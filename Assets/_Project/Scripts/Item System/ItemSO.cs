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
    }
}
