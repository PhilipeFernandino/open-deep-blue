using Coimbra;
using System;
using UnityEngine;

namespace Core.InventorySystem
{
    public class DraggableItemSlot : MonoBehaviour
    {
        private DraggableItem _dragabbleItem;

        public DraggableItem DraggableItem
        {
            get => _dragabbleItem;
            private set
            {
                if (_dragabbleItem == value)
                {
                    return;
                }


                if (value == null)
                {
                    _dragabbleItem.transform.SetParent(null, false);
                    _dragabbleItem = null;
                }
                else
                {
                    _dragabbleItem = value;
                    _dragabbleItem.DragStarting += DraggableItemDragStarted;
                    _dragabbleItem.transform.SetParent(transform, false);
                }

                Changed?.Invoke(value);
            }
        }

        public bool HasItem => DraggableItem != null;

        public event Action<DraggableItem> Changed;

        public void Setup(DraggableItem draggableItem)
        {
            DraggableItem = draggableItem;
        }

        public bool TryEmpty(out DraggableItem draggableItem)
        {
            if (HasItem)
            {
                draggableItem = DraggableItem;
                DraggableItem = null;
                return true;
            }

            draggableItem = null;
            return false;
        }

        public int TrySetOrAddToItemStack(ItemData itemData, int amount, Transform parentWhenDragging)
        {
            if (HasItem)
            {
                return DraggableItem.TryStack(itemData, amount);
            }
            else
            {
                (DraggableItem draggableItem, int added) = ScriptableSettings.Get<DraggableItemFactory>()
                    .Create(itemData, amount, parentWhenDragging, transform);

                DraggableItem = draggableItem;
                return added;
            }
        }

        public (ItemData itemData, int took) TryTakeFromItemStack(int amount)
        {
            if (HasItem)
            {
                int took = DraggableItem.TryTakeFromStack(amount);
                return (DraggableItem.ItemData, took);
            }
            else
            {
                return (null, 0);
            }
        }

        private void DraggableItemDragStarted()
        {
            TryEmpty(out _);
        }
    }
}