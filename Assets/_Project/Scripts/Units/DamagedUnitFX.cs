using Core.HealthSystem;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Core.Units
{
    public class DamagedUnitFX : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [SerializeField] private Material _damagedMaterial;
        [SerializeField] private float _damagedFXDuration = 0.3f;

        private Material _originalMaterial;
        private IHealth _health;
        private CancellationTokenSource _damagedFX_Cts;

        private void Awake()
        {
            _health = GetComponent<IHealth>();
        }

        private void Start()
        {
            _originalMaterial = _spriteRenderer.material;
            _health.Attacked += AttackedEventHandler;
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