using Coimbra;
using Coimbra.Services;
using Core.HoldableSystem;
using Core.ItemSystem;
using Core.Util;
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

            _holdable = Instantiate((Holdable)holdable, _equipmentParent);
        }

        public void TrySetItem(ItemSO item)
        {
            if (item == null || !item.IsEquipable)
            {
                Debug.Log($"{GetType()} - Non equippable selected {item}");
                return;
            }

            Debug.Log($"{GetType()} - Trying to set {item} from {_holdableDb}");

            if (_holdableDb.TryGetHoldable(item, out var holdable))
            {
                Debug.Log($"{GetType()} - Found holdable {holdable}");
                SetEquipment(holdable);
            }
            else
            {
                Debug.LogError($"{GetType()} - Item is equippable but holdable not found");
            }
        }

        public (bool success, HoldableUseEffect effect) TryUse(Vector2 worldPosition)
        {
            Debug.Log($"{GetType()} Try use: {_holdable}");

            if (_holdable == null)
            {
                return (false, HoldableUseEffect.None);
            }

            return _holdable.TryUse(worldPosition);
        }

        private void Awake()
        {
            _holdableDb = ScriptableSettings.GetOrFind<HoldableDatabase>();
        }

        private void Start()
        {
            _hotbarService = ServiceLocatorUtilities.GetServiceAssert<IHotbarService>();
            _hotbarService.ItemSelected += HotbarItemSelected;
        }

        private void HotbarItemSelected(InventoryItem item)
        {
            Debug.Log($"{GetType()} - Item Selected: {item}");
            if (item != null)
                TrySetItem(item.ItemData);
        }
    }
}