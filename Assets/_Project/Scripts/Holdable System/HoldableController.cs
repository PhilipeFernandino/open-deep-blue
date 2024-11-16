using Coimbra;
using Coimbra.Services;
using Core.HoldableSystem;
using Core.ItemSystem;
using Core.Utils;
using UnityEngine;

namespace Core.HoldableSystem
{
    public class HoldableController : MonoBehaviour
    {
        [SerializeField] private Transform _equipmentParent;

        private Holdable _holdable = null;
        private HoldableDatabase _holdableDb;

        private IHotbarService _hotbarService;

        public void SetEquipment(IHoldable holdable)
        {
            if (_holdable != null)
            {
                _holdable.Dispose(true);
            }

            Debug.Log("instantiating");
            _holdable = Instantiate((Holdable)holdable, _equipmentParent);
        }

        public void TrySetItem(ItemSO item)
        {
            Debug.Log($"try set ite {item} {_holdableDb}");

            if (item != null && _holdableDb.Holdables.TryGetValue(item, out TNRD.SerializableInterface<IHoldable> value))
            {
                Debug.Log(value.Value.ToString());
                SetEquipment(value.Value);
            }
        }

        public (bool success, HoldableUseEffect effect) TryUse(Vector2 worldPosition)
        {
            if (_holdable == null)
            {
                return (false, HoldableUseEffect.None);
            }

            return _holdable.TryUse(worldPosition);
        }

        private void Awake()
        {
            _holdableDb = ScriptableSettings.Get<HoldableDatabase>();
        }

        private void Start()
        {
            _hotbarService = ServiceLocatorUtilities.GetServiceAssert<IHotbarService>();
            _hotbarService.ItemSelected += HotbarItemSelected;
        }

        private void HotbarItemSelected(InventoryItem item)
        {
            Debug.Log(item);
            if (item != null)
            {
                TrySetItem(item.ItemData);
            }
        }
    }
}