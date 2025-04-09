using Coimbra.Services;
using Core.HealthSystem;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Core.HoldableSystem
{
    public class AttackEquipment : Equipment
    {
        public AttackEquipmentAttributes Attributes => (AttackEquipmentAttributes)_attributes;

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
                int damage = UnityEngine.Random.Range(Attributes.Damage.Min, Attributes.Damage.Max);

                if (healthCollider.Health.TryHurt(new Attack(damage, AttackType.Damage)))
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