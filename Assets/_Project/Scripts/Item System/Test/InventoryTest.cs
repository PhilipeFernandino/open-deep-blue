using Core.Utils;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Core.ItemSystem.Test
{
    public class InventoryTest : MonoBehaviour
    {
        [SerializeField] private List<Item> _itemsToAdd = new();

        private IInventoryService _inventoryService;

        private void Start()
        {
            _inventoryService = ServiceLocatorUtilities.GetServiceAssert<IInventoryService>();
        }

        [Button]
        public void AddItems()
        {
            _inventoryService.AddItems(_itemsToAdd);
        }
    }
}