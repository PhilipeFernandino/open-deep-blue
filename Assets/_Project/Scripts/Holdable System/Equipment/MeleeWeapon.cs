using Coimbra.Services;
using Core.HealthSystem;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Core.HoldableSystem
{
    public class MeleeWeapon : Equipment
    {
        public WeaponAttributes Attributes => (WeaponAttributes)_attributes;

        protected override void UseBehavior(Vector2 position)
        {

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
                int damage = Attributes.Damage;
                var attack = new Attack(
                        damage,
                        AttackType.Damage,
                        Attributes.Knockback,
                        transform.position);

                if (healthCollider.Health.TryHurt(attack))
                {
                    BlinkTime();
                    _cameraService.ShakeCamera(_attributes.ShakeSettings);
                }
            }
        }

        protected async void BlinkTime()
        {
            Time.timeScale = _attributes.TimeScaleOnBlink;
            await UniTask.Delay(TimeSpan.FromSeconds(_attributes.BlinkSeconds), true);
            Time.timeScale = 1;
        }
    }
}