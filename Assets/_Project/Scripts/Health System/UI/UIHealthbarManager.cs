using Coimbra;
using Coimbra.Services;
using UnityEngine;
using UnityEngine.Pool;

namespace Core.HealthSystem.UI
{
    public class UIHealthbarManager : Actor, IUIHealthbarService
    {
        [SerializeField] private UIHealthbar _uiHealthbar;

        private ObjectPool<UIHealthbar> _uiHealthbarPool;

        public void Subscribe(IHealth health, Transform transform, Vector3 offset, bool updatePosition, bool startDisabled = true)
        {
            var uiHealthbar = _uiHealthbarPool.Get();

            uiHealthbar.Setup(
                health,
                transform,
                offset,
                updatePosition,
                () => _uiHealthbarPool.Release(uiHealthbar),
                startDisabled);
        }

        protected override void OnInitialize()
        {
            _uiHealthbarPool = new(
                () => Instantiate(_uiHealthbar, transform),
                defaultCapacity: 20,
                maxSize: 30);

            ServiceLocator.Set<IUIHealthbarService>(this);
        }
    }

    [DynamicService]
    public interface IUIHealthbarService : IService
    {
        public void Subscribe(IHealth health, Transform transform, Vector3 offset, bool updatePosition, bool startDisabled = true);
    }
}