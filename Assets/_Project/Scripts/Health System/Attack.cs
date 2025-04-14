using UnityEngine;

namespace Core.HealthSystem
{
    public readonly struct Attack
    {
        public readonly float Damage;
        public readonly AttackType AttackType;
        public readonly float Knockback;
        public readonly Vector2 SourcePosition;

        public Attack(float damage, AttackType attackType, float knockback, Vector2 sourcePosition)
        {
            Damage = damage;
            AttackType = attackType;
            Knockback = knockback;
            SourcePosition = sourcePosition;
        }

        public override string ToString()
        {
            return $"({nameof(Damage)} = {Damage}, {nameof(AttackType)} = {AttackType}, {nameof(Knockback)} = {Knockback})";
        }
    }
}
