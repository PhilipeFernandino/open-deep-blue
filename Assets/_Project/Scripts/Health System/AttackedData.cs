using UnityEngine;

namespace Core.HealthSystem
{
    public readonly struct AttackedData
    {
        public readonly float HealthDifference;
        public readonly AttackType AttackType;
        public readonly Vector3 Position;

        public AttackedData(float healthDifference, AttackType attackType, Vector3 position)
        {
            HealthDifference = healthDifference;
            AttackType = attackType;
            Position = position;
        }
    }
}