using System.Collections;
using UnityEngine;

namespace Core.ItemSystem
{
    public class UIHotbar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UIHotbarRenderer _renderer;
        [SerializeField] private HotbarDatabase _database;
        [SerializeField] private UISelectItem _hotbarSelectedItem;


        private int _selectedHotbarIndex = 0;
        private int _hotbarSlots;

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

                _hotbarSelectedItem.SelectItem(_renderer.GetSlot(_selectedHotbarIndex));
            }
        }

        private void Start()
        {
            _hotbarSlots = _renderer.SlotCount;
            _database.Updated += _renderer.SetupSlot;
        }
    }
}