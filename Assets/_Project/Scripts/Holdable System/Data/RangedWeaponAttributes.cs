using UnityEngine;

namespace Core.HoldableSystem
{
    [CreateAssetMenu(menuName = "Core/Equipment/Ranged Weapon Attributes")]
    public class RangedWeaponAttributes : WeaponAttributes
    {
        [field: SerializeField] public uint Speed { get; private set; }
    }
}