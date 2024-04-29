using UnityEngine;

namespace Core.HealthSystem
{
    public class InitHealthComponent : MonoBehaviour
    {
        [SerializeField] private HealthData _healthData;

        private void Awake()
        {
            GetComponent<HealthComponent>().Setup(_healthData);
        }
    }
}
