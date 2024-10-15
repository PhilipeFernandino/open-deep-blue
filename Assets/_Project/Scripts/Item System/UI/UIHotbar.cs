using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.ItemSystem
{
    public class UIHotbar : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private UIInventory _uiInventory;

        [SerializeField] private List<UIInventoryItem> _slots = new();

        private List<InventoryItem> _items = new();
        private UIInventoryItem _activeItem;

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void Setup(UIInventoryItem item, bool activate = false)
        {
            _activeItem = item;

            if (activate)
            {
                Activate();
            }
        }

        private void Start()
        {
            _uiInventory.ItemActionRaised += ItemActionRaisedEventHandler;

            for (int i = 0; i < _slots.Count; i++)
            {
                int index = i;
                _slots[i].Clicked += (item) => ItemClickedEventHandler(item, index);
            }

            Deactivate();
        }

        private void ItemClickedEventHandler(UIInventoryItem uiItem, int index)
        {
            Debug.Log($"hotbar slot clicked {uiItem}, {index}", this);

            if (_activeItem == null)
            {
                return;
            }

            _slots[index].Setup(_activeItem.Item);
        }

        private void ItemActionRaisedEventHandler(ItemActionRaisedEvent e)
        {
            if (e.Action == ItemAction.Equip)
            {
                Setup(e.Item, true);
            }
        }

        public void OnSelect(BaseEventData eventData)
        {

        }

        public void OnDeselect(BaseEventData eventData)
        {

        }
    }
}