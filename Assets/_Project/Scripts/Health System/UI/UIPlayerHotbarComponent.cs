using Core.Util;
using UnityEngine;

namespace Core.HealthSystem
{
    [RequireComponent(typeof(UIHealthbar))]
    public class UIPlayerHotbarComponent : Singleton<UIPlayerHotbarComponent>
    {
        private UIHealthbar _uiHealthbar;

        public void Setup(HealthComponent healthComponent)
        {
            _uiHealthbar.Setup(healthComponent, startDisabled: false);
        }

        private void Awake()
        {
            _uiHealthbar = GetComponent<UIHealthbar>();
        }
    }
}