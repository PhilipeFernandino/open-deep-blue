using Coimbra;
using Coimbra.Services;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.ItemSystem
{
    public class InventoryDatabase : Actor, IInventoryService
    {
        private HashSet<Item> _items;

        public IEnumerable<Item> Items => _items.ToList();

        public IEnumerable<Item> Filter(ItemCategory? category = null, ItemRarity? rarity = null)
        {
            return _items.Where((x) =>
            {
                if (category.HasValue && x.Category != category.Value)
                {
                    return false;
                }

                if (rarity.HasValue && x.Rarity != rarity.Value)
                {
                    return false;
                }

                return true;
            });
        }

        public void AddItems(params Item[] items)
        {
            AddItems(items);
        }

        public void AddItems(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                if (_items.TryGetValue(item, out Item dbItem))
                {
                    dbItem.Amount += item.Amount;
                    dbItem.Timestamp = DateTime.Now.Ticks;
                }
                else
                {
                    _items.Add(item);
                }
            }
        }

        protected override void OnInitialize()
        {
            _items = new();

            ServiceLocator.Set<IInventoryService>(this);
        }

        [Button]
        private void LogItems()
        {
            foreach (var item in _items)
            {
                Debug.Log(item.ToString());
            }
        }

    }

    [DynamicService]
    public interface IInventoryService : IService
    {
        public IEnumerable<Item> Items { get; }
        public IEnumerable<Item> Filter(ItemCategory? category = null, ItemRarity? rarity = null);
        public void AddItems(params Item[] items);
        public void AddItems(IEnumerable<Item> items);
    }
}
