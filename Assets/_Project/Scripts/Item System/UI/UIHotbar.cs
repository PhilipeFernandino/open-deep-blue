using Core.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.ItemSystem
{
    public class UIHotbar : UIDynamicCanvas, ISelectHandler, IDeselectHandler
    {
        [Header("References")]
        [SerializeField] private UIInventory _uiInventory;
        [SerializeField] private UISelectItem _uiSelectItem;
        [SerializeField] private UIItemActions _itemActions;

        [SerializeField] private HotbarDatabase _hotbarDatabase;

        [SerializeField] private List<UIInventoryItem> _slots = new();

        private ItemAction[] _itemInventoryActions = { ItemAction.Unequip };

        private UIInventoryItem ActiveItem => _uiSelectItem.SelectedItem;

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
            _uiSelectItem.SelectItem(activeItem);

            if (activate)
            {
                Activate();
            }
        }

        private void SetupSlot(HotbarUpdateEventArgs e)
        {
            _uiSelectItem.Deselect();
            _slots[e.Index].Setup(e.Item);
        }

        private void Start()
        {
            _uiInventory.ItemActionRaised += InventoryItemActionRaisedEventHandler;
            _itemActions.ItemActionRaised += HotbarItemActionRaisedEventHandler;

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
                if (uiItem.HasItem)
                {
                    _itemActions.Setup(uiItem, _itemInventoryActions, true);
                }
            }
            else
            {
                _hotbarDatabase.SetupItem(ActiveItem.Item, index);
            }

        }

        private void InventoryItemActionRaisedEventHandler(ItemActionRaisedEvent e)
        {
            if (e.Action == ItemAction.Equip)
            {
                Setup(e.Item, true);
            }
        }

        private void HotbarItemActionRaisedEventHandler(ItemActionRaisedEvent e)
        {
            if (e.Action == ItemAction.Unequip)
            {
                _hotbarDatabase.RemoveItem(e.Item.Item);
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