using Coimbra.Listeners;
using Core.HealthSystem;
using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

namespace Core.Units.Bosses.Ant
{
    public class AntPaw : MonoBehaviour
    {
        [SerializeField] private HealthComponent _health;
        [SerializeField] private Collider2D _hitterCollider;

        [SerializeField] private TriggerEnter2DListener _collisionListener;

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

        private void Start()
        {
            _collisionListener.OnTrigger += CollisionTriggered;
        }

        private void CollisionTriggered(Trigger2DListenerBase sender, Collider2D other)
        {
            Debug.Log(other);
            if (other.gameObject.TryGetComponent(out HealthCollider healthCollider))
            {
                healthCollider.Health.TryHurt(new Attack(500f, AttackType.Damage));
            }
        }
    }
}