using Core.Util;
using UnityEngine;

namespace Core.HealthSystem.UI
{
    [RequireComponent(typeof(HealthComponent))]
    public class UIHealthbarComponent : MonoBehaviour
    {
        [SerializeField] private Vector3 _offset;
        [SerializeField] private bool _updatePosition;
        [SerializeField] private Transform _followTransform;

        [SerializeField] private bool _subscribeOnAwake = false;
        [SerializeField] private bool _startDisabled = true;

        private HealthComponent _healthComponent;

        public void Setup()
        {
            ServiceLocatorUtilities.GetServiceAssert<IUIHealthbarService>()
                 .Subscribe(
                 _healthComponent,
                 _followTransform,
                 _offset,
                 _updatePosition,
                 _startDisabled);
        }

        private void Awake()
        {
            _healthComponent = GetComponent<HealthComponent>();

            if (_subscribeOnAwake)
            {
                Setup();
            }
        }
    }
}