using System;
using UnityEngine;

namespace Core.InventorySystem
{
    [Serializable]
    public class Item
    {
        [SerializeField] private int _stack;
        [field: SerializeField] public ItemData ItemData { get; private set; }

        public int MaxStack => ItemData.MaxStack;
        public int StackCurrentCapacity => ItemData.MaxStack - Stack;
        public int Stack
        {
            get => _stack;
            private set
            {
                if (_stack == value)
                {
                    return;
                }

                _stack = value;
                StackChanged?.Invoke();
            }
        }

        public event Action StackChanged;

        public Item(ItemData itemData)
        {
            ItemData = itemData;
        }

        public int TryStack(ItemData itemData, int amount)
        {
            if (ItemData.Equals(itemData))
            {
                int added = Mathf.Clamp(amount, 0, StackCurrentCapacity);
                Stack += added;
                return added;
            }

            return 0;
        }

        public int TryTakeFromStack(int amount)
        {
            int totalTaken = Mathf.Clamp(amount, 0, Stack);
            Stack -= totalTaken;
            return totalTaken;
        }

        public void HoldItem()
        {

        }
    }
}