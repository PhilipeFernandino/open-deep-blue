using System;

namespace Core.HealthSystem
{
    public interface IHealth
    {
        public float Health { get; }
        public float MaxHealth { get; }

        public bool TryHurt(Attack attackData);

        public event Action<HealthChangedData> HealthChanged;
        public event Action<AttackedData> Attacked;
        public event Action HealthZeroed;
    }
}