using Coimbra;
using Core.CameraSystem;
using Core.HealthSystem;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Core.HoldableSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private ProjectileAttributes _attributes;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Rigidbody2D _rb2D;

        private RangedWeaponAttributes _weaponAttributes;
        private Vector2 _velocity;
        private TimeSpan _ttl;

        private Action<Projectile> _releaseCallback;
        private CancellationTokenSource _ttlCts;

        public void Create(RangedWeaponAttributes weaponAttributes, Action<Projectile> releaseCallback, int ttlSeconds = 10)
        {
            _weaponAttributes = weaponAttributes;
            _releaseCallback = releaseCallback;
            _ttl = TimeSpan.FromSeconds(ttlSeconds);
        }

        public void Setup(Vector2 direction)
        {
            _velocity = ((_weaponAttributes.Speed + _attributes.Speed) * direction.normalized);
            transform.up = direction.normalized;
            SetActiveState(true);

            _ttlCts?.Cancel();
            _ttlCts?.Dispose();
            _ttlCts = new();

            TTLTask().Forget();
        }

        private async UniTaskVoid TTLTask()
        {
            var token = _ttlCts.Token;
            bool wasCancelled = await UniTask.Delay(_ttl, cancellationToken: token).SuppressCancellationThrow();

            if (!wasCancelled)
            {
                Release();
            }
        }

        private void SetActiveState(bool state)
        {
            enabled = state;
            _spriteRenderer.enabled = state;
            _rb2D.simulated = state;
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log($"{collision} - enter");

            GameObject target = collision.gameObject;
            TryHurt(target);
        }

        protected void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"{collision} - enter");

            GameObject target = collision.gameObject;
            TryHurt(target);
        }

        private void Release()
        {
            if (!enabled)
                return;

            _releaseCallback?.Invoke(this);
            SetActiveState(false);
            _ttlCts.Cancel();
        }

        protected void TryHurt(GameObject target)
        {
            if (!enabled)
            {
                return;
            }

            if (target.TryGetComponent(out HealthCollider healthCollider))
            {
                uint damage = _attributes.Damage + _weaponAttributes.Damage;
                uint knockback = _attributes.Knockback + _weaponAttributes.Knockback;
                var attack = new Attack(
                        damage,
                        AttackType.Damage,
                        knockback,
                        transform.position);

                healthCollider.Health.TryHurt(attack);
            }

            Release();
        }

        private void Awake()
        {
            _rb2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            _rb2D.MovePosition(transform.position.XY() + _velocity * Time.fixedDeltaTime);
        }
    }
}