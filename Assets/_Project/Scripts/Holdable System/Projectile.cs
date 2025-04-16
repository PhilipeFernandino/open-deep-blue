using Core.CameraSystem;
using Core.HealthSystem;
using System.Collections;
using UnityEngine;

namespace Core.HoldableSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private ProjectileAttributes _attributes;

        private Rigidbody2D _rb2D;

        private RangedWeaponAttributes _weaponAttributes;
        private Vector2 _velocity;

        public void Setup(RangedWeaponAttributes weaponAttributes, Vector2 direction)
        {
            Debug.Log(direction);

            _rb2D = GetComponent<Rigidbody2D>();

            _weaponAttributes = weaponAttributes;
            _velocity = ((_weaponAttributes.Speed + _attributes.Speed) * direction.normalized) / 50;
            transform.up = direction.normalized;
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log($"{collision} - enter");

            GameObject gameObject = collision.gameObject;
            TryHurt(gameObject);
        }

        protected void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"{collision} - enter");

            GameObject gameObject = collision.gameObject;
            TryHurt(gameObject);
        }


        protected void TryHurt(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out HealthCollider healthCollider))
            {
                uint damage = _attributes.Damage + _weaponAttributes.Damage;
                uint knockback = _attributes.Knockback + _weaponAttributes.Knockback;
                var attack = new Attack(
                        damage,
                        AttackType.Damage,
                        knockback,
                        transform.position);

                if (healthCollider.Health.TryHurt(attack))
                {
                }
            }
        }


        private void FixedUpdate()
        {
            _rb2D.MovePosition(transform.position.XY() + _velocity);
        }
    }
}