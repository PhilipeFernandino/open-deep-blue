using System.Collections.Generic;
using UnityEngine;

namespace Core.InventorySystem
{
    public class HotbarManager : MonoBehaviour
    {
        [SerializeField] private Transform _hotbarSlotsParent;
        [SerializeField] private Transform _inventoryHotbarSlotsParent;

        private List<HotbarItemSlot> _hotbarSlots;
        private List<DraggableItemSlot> _inventoryHotbarSlots;

        private void Awake()
        {
            _hotbarSlots = new(_hotbarSlotsParent.childCount);
            _inventoryHotbarSlots = new(_inventoryHotbarSlotsParent.childCount);

            GetSlots(_hotbarSlots, _hotbarSlotsParent);
            GetSlots(_inventoryHotbarSlots, _inventoryHotbarSlotsParent);

            for (int i = 0; i < _inventoryHotbarSlots.Count; i++)
            {
                int index = i;
                _inventoryHotbarSlots[i].Changed += (DraggableItem draggable) => SlotChangedEventHandler(draggable, index);
            }

            void GetSlots<T>(List<T> slots, Transform slotsParent)
            {
                slots.Clear();

                foreach (Transform t in slotsParent)
                {
                    if (t.TryGetComponent(out T slot))
                    {
                        slots.Add(slot);
                    }
                }

            }
        }

        private void SlotChangedEventHandler(DraggableItem draggable, int index)
        {
            if (draggable != null)
            {
                _hotbarSlots[index].Setup(draggable.Item);
            }
            else
            {
                _hotbarSlots[index].Empty();
            }
        }
    }
}