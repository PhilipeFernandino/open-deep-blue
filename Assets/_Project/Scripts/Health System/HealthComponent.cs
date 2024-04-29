using System;
using UnityEngine;

namespace Core.HealthSystem
{
    public class HealthComponent : MonoBehaviour, IHealth
    {
        private HealthData _healthData;

        private float _health;

        public event Action<AttackedData> Attacked;
        public event Action<HealthChangedData> HealthChanged;
        public event Action HealthZeroed;

        public bool IsAlive { get; private set; } = true;

        public float MaxHealth => _healthData.BaseHealth;

        public float Health
        {
            get => _health;
            set
            {
                float healthNewValue = Mathf.Clamp(value, 0, MaxHealth);

                if (_health != healthNewValue)
                {
                    if (healthNewValue <= 0)
                    {
                        IsAlive = false;
                        HealthZeroed?.Invoke();
                    }

                    float healthCurrentValue = _health;
                    _health = healthNewValue;

                    HealthChanged?.Invoke(new(healthCurrentValue, healthNewValue));
                }
            }
        }

        public void Hurt(Attack attackData)
        {
            Debug.Log($"{gameObject.name}.{GetType()} - Hurt with Attack Data = {attackData}");

            float currentHealth = Health;

            Health -= attackData.Damage;
            Attacked?.Invoke(new AttackedData(Health - currentHealth, AttackType.Damage, transform.position));
        }

        public void Setup(HealthData healthData)
        {
            _healthData = healthData;
            _health = _healthData.BaseHealth;
        }
    }
}