using Core.HealthSystem;
using Cysharp.Threading.Tasks;
using PrimeTween;
using System;
using System.Threading;
using UnityEngine;

namespace Core.Units
{
    [RequireComponent(typeof(Collider2D))]
    public class Equipment : MonoBehaviour
    {
        [Tooltip("How many times the equipment can be used per second")]
        [SerializeField] private float _usesPerSecond;
        [SerializeField] private float _angleOffset;
        [SerializeField] private float _disableSRDelaySeconds = 0.1f;

        [SerializeField] private TweenSettings<Vector3> _rotTweenSettings;

        private Collider2D _collider;
        private SpriteRenderer _spriteRenderer;

        private bool _upSwingDirection = true;
        private bool _canUse = true;

        private TimeSpan _disableSRDelay;
        private CancellationTokenSource _disableSR_Cts;

        private TimeSpan UseInterval => TimeSpan.FromSeconds(1f / _usesPerSecond);

        public async void TryUse(Vector2 worldPosition)
        {
            if (_canUse)
            {
                Use(worldPosition).Forget();

                _canUse = false;
                await UniTask.Delay(UseInterval);
                _canUse = true;
            }
        }

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            _disableSRDelay = TimeSpan.FromSeconds(_disableSRDelaySeconds);
            _spriteRenderer.enabled = false;
        }

        private async UniTask Use(Vector2 worldPosition)
        {
            _disableSR_Cts?.Cancel();
            _disableSR_Cts = new CancellationTokenSource();

            _spriteRenderer.enabled = true;
            _collider.enabled = true;

            Vector2 dir = transform.parent.position.XY() - worldPosition;
            float angle = Vector2.SignedAngle(Vector3.right, dir) + 90f;

            Debug.Log($"{dir}, {angle}");


            float startAngle = angle - _angleOffset;
            float targetAngle = angle + _angleOffset;

            // Juice it up 
            if (_upSwingDirection)
            {
                (startAngle, targetAngle) = (targetAngle, startAngle);
            }

            _upSwingDirection = !_upSwingDirection;

            _rotTweenSettings.startValue = new Vector3(0f, 0f, startAngle);
            _rotTweenSettings.endValue = new Vector3(0f, 0f, targetAngle);

            await Tween.EulerAngles(
                transform.parent,
                _rotTweenSettings);

            _collider.enabled = false;

            DisableSpriteRendererTask().Forget();
        }

        private async UniTask DisableSpriteRendererTask()
        {
            CancellationToken token = _disableSR_Cts.Token;
            await UniTask.Delay(_disableSRDelay, cancellationToken: token).SuppressCancellationThrow();

            if (!token.IsCancellationRequested)
            {
                _spriteRenderer.enabled = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log($"{collision} - enter");

            if (collision.TryGetComponent(out IHealth health))
            {
                health.Hurt(new Attack(50f, AttackType.Damage));
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Debug.Log($"{collision} - exit");
        }
    }
}