using UnityEngine;

namespace Core.HealthSystem
{
    [RequireComponent(typeof(HealthComponent))]
    public class UIInjectPlayerHealthbar : MonoBehaviour
    {
        private HealthComponent _healthComponent;

        private void Start()
        {
            _healthComponent = GetComponent<HealthComponent>();
            UIPlayerHotbarComponent.Instance.Setup(_healthComponent);
        }
    }
}