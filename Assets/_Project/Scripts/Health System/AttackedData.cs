using UnityEngine;

namespace Core.HealthSystem
{
    public readonly struct AttackedData
    {
        public readonly float HealthDifference;
        public readonly Attack Attack;

        public AttackedData(float healthDifference, Attack attack)
        {
            HealthDifference = healthDifference;
            Attack = attack;
        }

        public override string ToString()
        {
            return $"({nameof(HealthDifference)} = {HealthDifference}, {nameof(Attack)} = {Attack})";
        }
    }
}