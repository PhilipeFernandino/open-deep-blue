using Cysharp.Threading.Tasks;
using PrimeTween;
using System;
using UnityEngine;

namespace Core.Units
{
    [RequireComponent(typeof(Collider2D))]
    public class Equipment : MonoBehaviour
    {
        [Tooltip("How many times the equipment can be used per second")]
        [SerializeField] private float _usesPerSecond;
        [SerializeField] private float _angleOffset;

        [SerializeField] private TweenSettings<Vector3> _rotTweenSettings;

        private Collider2D _collider;
        private SpriteRenderer _spriteRenderer;

        private bool _upSwingDirection = true;
        private bool _canUse = true;

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

            _spriteRenderer.enabled = false;
        }

        private async UniTask Use(Vector2 worldPosition)
        {
            _spriteRenderer.enabled = true;
            _collider.enabled = true;

            Vector2 dir = transform.parent.position.XY() - worldPosition;
            float angle = Vector2.SignedAngle(Vector3.right, dir) + 90f;

            Debug.Log($"{dir}, {angle}");


            float startAngle = angle - _angleOffset;
            float targetAngle = angle + _angleOffset;

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

            _spriteRenderer.enabled = false;
            _collider.enabled = false;
        }
    }
}