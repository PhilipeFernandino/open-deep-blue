using System.Collections;
using UnityEngine;

namespace Core.HoldableSystem
{
    [CreateAssetMenu(menuName = "Core/Equipment/Projectile Attributes")]
    public class ProjectileAttributes : ScriptableObject
    {
        [field: SerializeField] public uint Damage { get; private set; }
        [field: SerializeField] public uint Knockback { get; private set; }
        [field: SerializeField] public uint CriticalChance { get; private set; }
        [field: SerializeField] public uint Speed { get; private set; }

    }
}