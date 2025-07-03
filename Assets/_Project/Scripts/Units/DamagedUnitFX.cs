using Core.HealthSystem;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TNRD;
using UnityEngine;

namespace Core.Units
{
    public class DamagedUnitFX : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [SerializeField] private Material _damagedMaterial;
        [SerializeField] private float _damagedFXDuration = 0.3f;
        [SerializeField] private SerializableInterface<IHealth> _health;

        private Material _originalMaterial;
        private CancellationTokenSource _damagedFX_Cts;

        public IHealth Health
        {
            get => _health.Value;
            set => _health.Value = value;
        }

        private void Awake()
        {
            if (Health == null)
            {
                Health = GetComponent<IHealth>();
            }
        }

        private void Start()
        {
            _originalMaterial = _spriteRenderer.material;
            Health.Attacked += AttackedEventHandler;
        }

        private void AttackedEventHandler(AttackedData data)
        {
            _spriteRenderer.material = _damagedMaterial;
            RevertMaterialTask();
        }

        private async void RevertMaterialTask()
        {
            _damagedFX_Cts?.Cancel();
            _damagedFX_Cts = new();

            CancellationToken token = _damagedFX_Cts.Token;
            await UniTask.Delay(TimeSpan.FromSeconds(_damagedFXDuration), cancellationToken: token).SuppressCancellationThrow();

            if (!token.IsCancellationRequested)
            {
                _spriteRenderer.material = _originalMaterial;
            }
        }
    }
}