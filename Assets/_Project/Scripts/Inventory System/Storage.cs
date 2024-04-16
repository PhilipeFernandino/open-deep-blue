using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.InventorySystem
{
    [Serializable]
    public class Storage
    {
        [SerializeField] private List<Item> _items = new();

        public List<Item> Retrieve()
        {
            return _items;
        }

        public void Store(List<Item> items)
        {
            _items.Clear();
            _items = items;
        }
    }
}