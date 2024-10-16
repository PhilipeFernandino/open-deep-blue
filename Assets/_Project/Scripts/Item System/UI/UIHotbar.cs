using Core.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.ItemSystem
{
    public class UIHotbar : UIDynamicCanvas, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private UIInventory _uiInventory;
        [SerializeField] private HotbarDatabase _hotbarDatabase;

        [SerializeField] private List<UIInventoryItem> _slots = new();

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

        private void SetupSlot(HotbarUpdateEventArgs e)
        {
            _slots[e.Index].Setup(e.Item);
        }

        private void Start()
        {
            _uiInventory.ItemActionRaised += ItemActionRaisedEventHandler;

            for (int i = 0; i < _slots.Count; i++)
            {
                int index = i;
                _slots[i].Clicked += (item) => ItemClickedEventHandler(item, index);
            }

            _hotbarDatabase.Updated += SetupSlot;
        }

        private void ItemClickedEventHandler(UIInventoryItem uiItem, int index)
        {
            if (ActiveItem == null)
            {
                return;
            }

            _hotbarDatabase.SetupItem(ActiveItem.Item, index);

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