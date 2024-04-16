using NaughtyAttributes;
using UnityEngine;

namespace Core.InventorySystem.Tests
{
    public class InventoryManagerTest : MonoBehaviour
    {
        [SerializeField] private ItemData _itemData;
        [SerializeField] private int _stackToAdd;

        [Button]
        private void AddItems()
        {
            if (Application.isPlaying)
            {
                var inventoryManager = FindObjectOfType<InventoryManager>();
                inventoryManager.AddItemsToPlayerInventory(_itemData, _stackToAdd, out int _);
            }
        }
    }
}