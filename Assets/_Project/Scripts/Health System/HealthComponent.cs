using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Core.HealthSystem
{
    public class HealthComponent : MonoBehaviour, IHealth
    {
        private HealthData _healthData;

        private float _health;
        private bool _isTakingDamage = true;

        public event Action<AttackedData> Attacked;
        public event Action<HealthChangedData> HealthChanged;
        public event Action HealthZeroed;

        private CancellationTokenSource _iframeCTS;

        public bool IsAlive { get; private set; } = true;

        public float MaxHealth => _healthData.BaseHealth;

        public float Health
        {
            get => _health;
            private set
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

        public bool TryHurt(Attack attackData)
        {
            Debug.Log($"[{gameObject.name} {GetType()}] - Attack Data = {attackData}, taking damage: {_isTakingDamage}");

            if (!_isTakingDamage)
            {
                return false;
            }

            float currentHealth = Health;

            Health -= attackData.Damage;
            Attacked?.Invoke(new AttackedData(Health - currentHealth, attackData));
            return true;
        }

        public void Setup(HealthData healthData)
        {
            _healthData = healthData;
            _health = _healthData.BaseHealth;
        }

        public async UniTask AddIFrames(float duration)
        {
            _isTakingDamage = false;

            // Cancel any pending cts, get a new one and give the value token to the UniTask delay
            _iframeCTS?.Cancel();
            _iframeCTS = new CancellationTokenSource();

            var ct = _iframeCTS.Token;
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: ct).SuppressCancellationThrow();

            if (!ct.IsCancellationRequested)
            {
                _isTakingDamage = true;
            }
        }

        public void SetIsTakingDamage(bool value)
        {
            _iframeCTS?.Cancel();
            _isTakingDamage = value;
        }
    }
}