using Core.HealthSystem;
using System.Collections;
using UnityEngine;

namespace Core.Units.Bosses.Ant
{
    [RequireComponent(typeof(BoxCollider2D))]

    public class AntPaw : MonoBehaviour
    {
        private BoxCollider2D _boxCollider;

        public BoxCollider2D Collider => _boxCollider;

        public bool ColliderEnabled
        {
            get => Collider.enabled;

            set => Collider.enabled = value;
        }

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log($"[{gameObject.name}] - {collision} - enter");

            if (collision.TryGetComponent(out IHealth health))
            {
                health.Hurt(new Attack(500f, AttackType.Damage));
            }
        }
    }
}