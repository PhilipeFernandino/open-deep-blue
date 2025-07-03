using System.Collections;
using UnityEngine;

namespace Core.HoldableSystem
{
    [CreateAssetMenu(menuName = "Core/Equipment/Weapon Attributes")]
    public class WeaponAttributes : EquipmentAttributes
    {
        [field: SerializeField] public ushort Damage { get; private set; }
        [field: SerializeField] public ushort Knockback { get; private set; }
        [field: SerializeField] public ushort CriticalChance { get; private set; }
    }
}