using Coimbra;
using Coimbra.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.ItemSystem
{
    public class HotbarService : Actor, IHotbarService
    {
        [SerializeField] private Hotbar _hotbar;
        [SerializeField] private HotbarDatabase _hotbarDatabase;

        public event Action<InventoryItem> ItemSelected
        {
            add => _hotbar.ItemSelected += value;
            remove => _hotbar.ItemSelected -= value;
        }

        public event Action<HotbarUpdateEventArgs> Updated
        {
            add => _hotbarDatabase.Updated += value;
            remove => _hotbarDatabase.Updated -= value;
        }

        public int SelectedHotbarIndex
        {
            get => _hotbar.SelectedHotbarIndex;
            set
            {
                _hotbar.SelectedHotbarIndex = value;
            }
        }

        public List<InventoryItem> Items => _hotbarDatabase.Items;

        public void SetupItem(InventoryItem item, int index) => _hotbarDatabase.SetupItem(item, index);

        public void RemoveItem(InventoryItem item) => _hotbarDatabase.RemoveItem(item);

        protected override void OnInitialize()
        {
            base.OnInitialize();
            ServiceLocator.Set<IHotbarService>(this);
        }
    }

    [DynamicService]
    public interface IHotbarService : IService
    {
        public event Action<InventoryItem> ItemSelected;

        public event Action<HotbarUpdateEventArgs> Updated;

        public int SelectedHotbarIndex { get; set; }

        public List<InventoryItem> Items { get; }

        public void SetupItem(InventoryItem item, int index);

        public void RemoveItem(InventoryItem item);

    }
}