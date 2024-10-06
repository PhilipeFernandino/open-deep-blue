using Coimbra;
using Coimbra.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.ItemSystem
{
    public class InventoryDatabase : Actor, IInventoryService
    {
        private HashSet<Item> _items;

        public List<Item> Items => _items.ToList();

        public List<Item> Filter(ItemCategory? category = null, ItemRarity? rarity = null)
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
            }).ToList();
        }

        public void AddItems(params Item[] items)
        {
            foreach (var item in items)
            {
                if (_items.TryGetValue(item, out Item dbItem))
                {
                    dbItem.Amount += item.Amount;
                    dbItem.Timestamp = DateTime.Now.Ticks;
                }
            }
        }

        public void AddItems(List<Item> items)
        {

        }

        protected override void OnInitialize()
        {
            _items = new();

            ServiceLocator.Set<IInventoryService>(this);
        }

    }

    [DynamicService]
    public interface IInventoryService : IService
    {
        public List<Item> Items { get; }
        public List<Item> Filter(ItemCategory? category = null, ItemRarity? rarity = null);
        public void AddItems(params Item[] items);
        public void AddItems(List<Item> items);
    }
}
