using Coimbra;
using Coimbra.Services;
using NaughtyAttributes;
using System;
using UnityEngine;

namespace Core.ItemSystem
{
    public class Hotbar : Actor
    {
        [Header("References")]
        [SerializeField, Required] private UIHotbarRenderer _renderer;
        [SerializeField, Required] private HotbarDatabase _database;
        [SerializeField, Required] private UISelectItem _hotbarSelectedItem;

        private int _selectedHotbarIndex = 0;
        private int _hotbarSlots;

        public event Action<InventoryItem> ItemSelected;

        public int SelectedHotbarIndex
        {
            get => _selectedHotbarIndex;
            set
            {
                if (value >= _hotbarSlots)
                {
                    _selectedHotbarIndex = 0;
                }
                else if (value < 0)
                {
                    _selectedHotbarIndex = _hotbarSlots - 1;
                }
                else
                {
                    _selectedHotbarIndex = value;
                }

                UIInventoryItem uiItem = _renderer.GetSlot(_selectedHotbarIndex);

                _hotbarSelectedItem.SelectItem(uiItem);
                ItemSelected?.Invoke(uiItem.Item);
            }
        }

        protected override void OnInitialize()
        {
            _hotbarSlots = _renderer.SlotCount;
            _database.Updated += _renderer.SetupSlot;
        }
    }
}