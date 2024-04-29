using Coimbra;
using Coimbra.Services;
using UnityEngine;
using UnityEngine.Pool;

namespace Core.HealthSystem.UI
{
    public class UIHitManager : Actor, IUIHitService
    {
        [SerializeField] private UIHit _uiHitPrefab;
        [SerializeField] private UIHitColorData _hitColorData;
        [SerializeField] private Vector3 _hitUIPositionOffset;

        private ObjectPool<UIHit> _uiHitPool;

        public void Subscribe(IHealth health)
        {
            health.Attacked += AttackedEventHandler;
        }

        public void Unsubscribe(IHealth health)
        {
            health.Attacked -= AttackedEventHandler;
        }

        private void AttackedEventHandler(AttackedData e)
        {
            var uiHit = _uiHitPool.Get();
            uiHit.transform.position = e.Position;

            Vector3 position = e.Position + _hitUIPositionOffset;

            uiHit.Setup(
                position,
                _hitColorData.GetColor(e.AttackType),
                Mathf.Abs(e.HealthDifference),
                () => _uiHitPool.Release(uiHit));
        }

        protected override void OnInitialize()
        {
            _uiHitPool = new(
                () => Instantiate(_uiHitPrefab, transform),
                defaultCapacity: 20,
                maxSize: 40);

            ServiceLocator.Set<IUIHitService>(this);
        }
    }

    [DynamicService]
    public interface IUIHitService : IService
    {
        public void Subscribe(IHealth health);
    }
}