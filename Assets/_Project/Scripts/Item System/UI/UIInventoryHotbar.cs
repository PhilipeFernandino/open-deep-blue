using UnityEngine;

namespace Core.ItemSystem
{
    public class UIInventoryHotbar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UIInventory _uiInventory;
        [SerializeField] private UISelectItem _uiSelectItem;
        [SerializeField] private UIItemActions _itemActions;

        [SerializeField] private HotbarDatabase _hotbarDatabase;
        [SerializeField] private UIHotbarRenderer _hotbarRenderer;

        private ItemAction[] _itemInventoryActions = { ItemAction.Unequip };

        private UIInventoryItem ActiveItem => _uiSelectItem.SelectedItem;

        public void Activate() => _hotbarRenderer.Activate();
        public void Deactivate() => _hotbarRenderer.Deactivate();

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
            _hotbarRenderer.SetupSlot(e);
        }

        private void Start()
        {
            _uiInventory.ItemActionRaised += InventoryItemActionRaisedEventHandler;
            _itemActions.ItemActionRaised += HotbarItemActionRaisedEventHandler;

            _hotbarRenderer.SlotClicked += ItemClickedEventHandler;
            _hotbarDatabase.Updated += SetupSlot;
        }

        private void ItemClickedEventHandler((UIInventoryItem uiItem, int index) obj)
        {
            if (ActiveItem == null)
            {
                if (obj.uiItem.HasItem)
                {
                    _itemActions.Setup(obj.uiItem, _itemInventoryActions, true);
                }
            }
            else
            {
                _hotbarDatabase.SetupItem(ActiveItem.Item, obj.index);
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
    }
}