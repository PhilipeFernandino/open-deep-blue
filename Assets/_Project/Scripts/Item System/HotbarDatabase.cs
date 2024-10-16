using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.ItemSystem
{
    public class HotbarDatabase : MonoBehaviour
    {
        private List<InventoryItem> _items;

        public List<InventoryItem> Items => _items;

        public event Action<HotbarUpdateEventArgs> Updated;

        public void SetupItem(InventoryItem item, int index)
        {
            _items[index] = item;
            Updated?.Invoke(new(item, index));
        }

        private void Awake()
        {
            _items = new(10);

            for (int i = 0; i < _items.Capacity; i++)
            {
                _items.Add(null);
            }
        }
    }

    public record HotbarUpdateEventArgs(InventoryItem Item, int Index)
    {
        public InventoryItem Item { get; private set; } = Item;
        public int Index { get; private set; } = Index;
    }
}