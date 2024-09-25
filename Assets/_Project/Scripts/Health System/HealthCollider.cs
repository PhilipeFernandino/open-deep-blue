using System.Collections;
using TNRD;
using UnityEngine;

namespace Core.HealthSystem
{
    [RequireComponent(typeof(Collider2D))]
    public class HealthCollider : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IHealth> _health;

        public IHealth Health => _health.Value;
    }
}