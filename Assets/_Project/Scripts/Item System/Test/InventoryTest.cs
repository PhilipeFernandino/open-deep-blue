using Core.Utils;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Core.ItemSystem.Test
{
    public class InventoryTest : MonoBehaviour
    {
        [SerializeField] private List<InventoryItem> _itemsToAdd = new();

        private IInventoryService _inventoryService;

        private void Start()
        {
            _inventoryService = ServiceLocatorUtilities.GetServiceAssert<IInventoryService>();
            Debug.Log(_inventoryService.GetType().Name);

            _inventoryService.AddItems(_itemsToAdd);
        }

        [Button]
        public void AddItems()
        {
            _inventoryService.AddItems(_itemsToAdd);
        }
    }
}