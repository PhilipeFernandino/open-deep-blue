using Core.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.ItemSystem
{
    public class UIHotbar : UIDynamicCanvas, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private UIInventory _uiInventory;

        [SerializeField] private List<UIInventoryItem> _slots = new();

        private List<InventoryItem> _items = new();
        private UIInventoryItem _activeItem;

        public UIInventoryItem ActiveItem
        {
            get => _activeItem;
            private set
            {
                if (_activeItem == value)
                {
                    return;
                }

                if (_activeItem != null)
                {
                    _activeItem.SetHighlight(false);

                }

                if (value != null)
                {
                    value.SetHighlight(true);
                }

                _activeItem = value;
            }
        }


        public void Activate()
        {
            ShowSelf();
        }

        public void Deactivate()
        {
            HideSelf();
        }

        public void Setup(UIInventoryItem activeItem, bool activate = false)
        {
            ActiveItem = activeItem;

            if (activate)
            {
                Activate();
            }
        }

        public void SetupSlot(InventoryItem item, int index)
        {
            _slots[index].Setup(item);
        }

        private void Start()
        {
            _uiInventory.ItemActionRaised += ItemActionRaisedEventHandler;

            for (int i = 0; i < _slots.Count; i++)
            {
                int index = i;
                _slots[i].Clicked += (item) => ItemClickedEventHandler(item, index);
            }
        }

        private void ItemClickedEventHandler(UIInventoryItem uiItem, int index)
        {
            if (ActiveItem == null)
            {
                return;
            }

            SetupSlot(ActiveItem.Item, index);

            ActiveItem = null;
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