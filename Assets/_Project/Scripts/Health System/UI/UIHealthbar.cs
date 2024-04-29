using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.HealthSystem
{
    public class UIHealthbar : MonoBehaviour
    {
        [SerializeField] private Image _healthbar;

        private Transform _targetTranform;
        private Action _releaseCallback;

        private IHealth _health;
        private Vector3 _offset;
        private bool _updatePosition = false;
        private bool _healthHasZeroed = false;

        public void Setup(IHealth health, Transform targetTransform, Vector3 offset,
            bool updatePosition, Action healthZeroedCallback, bool startDisabled = true)
        {
            if (startDisabled)
            {
                gameObject.SetActive(false);
            }

            _healthHasZeroed = false;

            _healthbar.fillAmount = 1f;

            _health = health;
            _offset = offset;
            _targetTranform = targetTransform;
            _updatePosition = updatePosition;
            _releaseCallback = healthZeroedCallback;

            transform.position = _targetTranform.position + _offset;

            health.HealthZeroed += HealthZeroedEventHandler;
            health.HealthChanged += HealthChangedEventHandler;
        }

        private void HealthZeroedEventHandler()
        {
            _healthHasZeroed = true;
            gameObject.SetActive(false);
            _releaseCallback();
        }

        private void HealthChangedEventHandler(HealthChangedData e)
        {

            Debug.Log($"{GetType()} - {e.HealthNewValue} / {_health.MaxHealth}");

            _healthbar.fillAmount = e.HealthNewValue / _health.MaxHealth;

            if (!_healthHasZeroed && !gameObject.activeSelf)
            {
                transform.position = _targetTranform.position + _offset;
                gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (_updatePosition)
            {
                transform.position = _targetTranform.position + _offset;
            }
        }
    }
}