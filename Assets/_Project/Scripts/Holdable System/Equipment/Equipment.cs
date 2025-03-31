using Core.CameraSystem;
using Core.Util;
using Cysharp.Threading.Tasks;
using PrimeTween;
using System;
using System.Threading;
using UnityEngine;

namespace Core.HoldableSystem
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class Equipment : Holdable
    {
        [SerializeField] protected EquipmentAttributes _attributes;

        protected Collider2D _collider;
        protected SpriteRenderer _spriteRenderer;

        protected bool _upSwingDirection = true;
        protected bool _canUse = true;

        protected TimeSpan _disableSRDelay;
        protected CancellationTokenSource _disableSR_Cts;

        protected ICameraService _cameraService;

        protected TimeSpan ReloadInterval => TimeSpan.FromSeconds(1f / _attributes.UsesPerSecond);

        protected abstract void UseBehavior(Vector2 position);

        public override (bool success, HoldableUseEffect effect) TryUse(Vector2 worldPosition)
        {
            if (_canUse)
            {
                Use(worldPosition).Forget();
                UseBehavior(worldPosition);
                AvailableTask().Forget();

                Vector2 addedVelocity = _attributes.UseEffect.AddedImpulseSpeed * (worldPosition - transform.parent.position.XY()).NormalizeExcess();
                return (true, _attributes.UseEffect.Create(addedVelocity));
            }

            return (false, _attributes.UseEffect);
        }

        protected async UniTaskVoid AvailableTask()
        {
            _canUse = false;
            await UniTask.Delay(ReloadInterval);
            _canUse = true;
        }

        protected void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            _disableSRDelay = TimeSpan.FromSeconds(_attributes.DisableSRDelaySeconds);
            _spriteRenderer.enabled = false;
        }

        protected void Start()
        {
            _cameraService = ServiceLocatorUtilities.GetServiceAssert<ICameraService>();
        }

        protected async UniTask Use(Vector2 worldPosition)
        {
            _disableSR_Cts?.Cancel();
            _disableSR_Cts = new CancellationTokenSource();

            _spriteRenderer.enabled = true;
            _collider.enabled = true;

            Vector2 dir = transform.parent.position.XY() - worldPosition;
            float angle = Vector2.SignedAngle(Vector3.right, dir) + 90f;

            Debug.Log($"{dir}, {angle}");


            float startAngle = angle - _attributes.AngleOffset;
            float targetAngle = angle + _attributes.AngleOffset;

            // Juice it up 
            if (_upSwingDirection)
            {
                (startAngle, targetAngle) = (targetAngle, startAngle);
            }

            _upSwingDirection = !_upSwingDirection;

            var rotTweenSettings = _attributes.RotTweenSettings;
            rotTweenSettings.startValue = new Vector3(0f, 0f, startAngle);
            rotTweenSettings.endValue = new Vector3(0f, 0f, targetAngle);

            transform.parent.eulerAngles = rotTweenSettings.startValue;

            await Tween.EulerAngles(
                transform.parent,
                rotTweenSettings);

            _collider.enabled = false;

            DisableSpriteRendererTask().Forget();
        }

        protected async UniTask DisableSpriteRendererTask()
        {
            CancellationToken token = _disableSR_Cts.Token;
            await UniTask.Delay(_disableSRDelay, cancellationToken: token).SuppressCancellationThrow();

            if (!token.IsCancellationRequested)
            {
                _spriteRenderer.enabled = false;
            }
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            Debug.Log($"{GetType()} - {collision} - exit");
        }
    }
}