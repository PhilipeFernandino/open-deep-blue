namespace Core.HealthSystem
{
    public readonly struct Attack
    {
        public readonly float Damage;
        public readonly AttackType AttackType;

        public Attack(float damage, AttackType attackType)
        {
            Damage = damage;
            AttackType = attackType;
        }
    }
}
