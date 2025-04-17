using Core.CameraSystem;
using Core.Level;
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
        protected CancellationTokenSource _disableSRCts;

        protected ICameraService _cameraService;
        protected IGridService _gridService;

        protected TimeSpan ReloadInterval => TimeSpan.FromSeconds(1f / _attributes.UsesPerSecond);

        protected abstract void UseBehavior(Vector2 worldPosition);

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

        protected virtual void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            _disableSRDelay = TimeSpan.FromSeconds(_attributes.DisableSRDelaySeconds);
            _spriteRenderer.enabled = false;
        }

        protected virtual void Start()
        {
            _cameraService = ServiceLocatorUtilities.GetServiceAssert<ICameraService>();
            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
        }

        protected async UniTask Use(Vector2 worldPosition)
        {
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

            // FIX
            bool wasDestroyed = await Tween.EulerAngles(
                transform.parent,
                rotTweenSettings).ToUniTask(cancellationToken: destroyCancellationToken).SuppressCancellationThrow();

            if (!wasDestroyed)
            {
                _collider.enabled = false;
                DisableSpriteRendererTask().Forget();
            }
        }

        protected async UniTask DisableSpriteRendererTask()
        {
            _disableSRCts?.Cancel();
            _disableSRCts?.Dispose();
            _disableSRCts = new();

            var token = _disableSRCts.Token;
            bool wasCancelled = await UniTask.Delay(_disableSRDelay, cancellationToken: token).SuppressCancellationThrow();

            if (!wasCancelled)
            {
                Debug.LogWarning("something went wrong");
                _spriteRenderer.enabled = false;
            }
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            Debug.Log($"{GetType()} - {collision} - exit");
        }
    }
}