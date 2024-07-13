using Core.HealthSystem;
using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

namespace Core.Units.Bosses.Ant
{
    public class AntPaw : MonoBehaviour
    {
        [SerializeField] private HealthComponent _health;
        [SerializeField] private BoxCollider2D _hitterCollider;

        public float Health => _health.Health;

        public void SetIsRaised(bool value)
        {
            _health.SetIsTakingDamage(!value);
            IsHitterEnabled = !value;
        }

        #region Collider Region
        public bool IsHitterEnabled
        {
            get => _hitterCollider.enabled;
            set => _hitterCollider.enabled = value;
        }
        #endregion

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"[{gameObject.name} - {GetType()}]: Collider: {collision} - enter");

            if (collision.gameObject.TryGetComponent(out IHealth health))
            {
                health.TryHurt(new Attack(500f, AttackType.Damage));
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            Debug.Log($"[{gameObject.name} - {GetType()}]: Collision = {collider} - enter");

            if (collider.gameObject.TryGetComponent(out IHealth health))
            {
                health.TryHurt(new Attack(500f, AttackType.Damage));
            }
        }
    }
}