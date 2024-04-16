using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Core.InventorySystem.Tests
{
    public class StorageTest : MonoBehaviour
    {
        [SerializeField] private List<Item> items = new();
        [SerializeField] private InventoryManager _inventoryManager;

        private Storage _storage;

        [Button]
        public void Store()
        {
            _storage.Store(items);
        }

        [Button]
        public void OpenInventory()
        {
            _inventoryManager.OpenInventoryWithStorage(_storage);
        }

        [Button]
        public void CloseInventory()
        {
            _inventoryManager.CloseInventory();
        }

        private void Awake()
        {
            _storage = new Storage();
        }
    }
}