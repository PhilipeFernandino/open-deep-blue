using Coimbra;
using Core.UI;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Core.InventorySystem
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private Transform _storageSlotsParent;
        [SerializeField] private Transform _inventoryHotbarSlotsParent;
        [SerializeField] private Transform _playerInventorySlotsParent;

        [SerializeField] private UIDynamicCanvas _storageCanvas;
        [SerializeField] private UIDynamicCanvas _playerInventoryCanvas;

        [SerializeField] private UIDynamicCanvas _hotbarCanvas;

        private List<DraggableItemSlot> _hotbarSlots;
        private List<DraggableItemSlot> _storageSlots;
        private List<DraggableItemSlot> _playerInventorySlots;

        private Storage _openStorage;

        private Dictionary<ItemData, int> _itemDatas;

        public void OpenInventory()
        {
            if (_playerInventoryCanvas.IsEnabled)
            {
                return;
            }

            _playerInventoryCanvas.ShowSelf();
            _hotbarCanvas.HideSelf();
        }

        public void OpenInventoryWithStorage(Storage storage)
        {
            if (_playerInventoryCanvas.IsEnabled && _storageCanvas.IsEnabled)
            {
                return;
            }

            _openStorage = storage;

            var items = storage.Retrieve();

            for (int i = 0; i < items.Count; i++)
            {
                AddItemsToSlots(_storageSlots, items[i].ItemData, items[i].Stack, out _);
            }

            OpenInventory();

            _storageCanvas.ShowSelf();
        }

        public void CloseInventory()
        {
            if (_storageCanvas.IsEnabled)
            {
                ReturnItemsToStorage();
                _openStorage = null;
                _storageCanvas.HideSelf();
            }

            _hotbarCanvas.ShowSelf();
            _playerInventoryCanvas.HideSelf();
        }

        public void AddItemsToPlayerInventory(ItemData itemData, int amount, out int totalStacked) => AddItemsToSlots(_playerInventorySlots, itemData, amount, out totalStacked);

        private void ReturnItemsToStorage()
        {
            var items = GetItemsFromSlots(_storageSlots, true);
            _openStorage.Store(items);
        }

        private List<Item> GetItemsFromSlots(List<DraggableItemSlot> slots, bool emptySlots)
        {
            var items = new List<Item>();

            for (int i = 0; i < slots.Count; i++)
            {
                if (emptySlots)
                {
                    if (slots[i].TryEmpty(out DraggableItem draggableItem))
                    {
                        items.Add(draggableItem.Item);
                        draggableItem.gameObject.Dispose(true);
                    }
                }
                else
                {
                    items.Add(slots[i].DraggableItem.Item);
                }
            }

            return items;
        }

        private void AddToSlot(DraggableItemSlot slot, ItemData itemData, int stack, out int totalAmountStacked)
        {
            totalAmountStacked = slot.TrySetOrAddToItemStack(itemData, stack, _playerInventoryCanvas.transform);
        }

        [Button]
        public void ReorganizePlayerInventory()
        {
            ReorganizeItems(_playerInventorySlots);
        }

        private void ReorganizeItems(List<DraggableItemSlot> slots)
        {
            _itemDatas.Clear();

            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].TryEmpty(out DraggableItem draggableItem))
                {
                    Debug.Log($"{GetType()} - To dict... {draggableItem.ItemData.Name}, {draggableItem.Stack}");

                    if (_itemDatas.ContainsKey(draggableItem.ItemData))
                    {
                        _itemDatas[draggableItem.ItemData] += draggableItem.Stack;
                    }
                    else
                    {
                        _itemDatas.Add(draggableItem.ItemData, draggableItem.Stack);
                    }

                    draggableItem.Dispose(false);
                }
            }

            foreach (var (itemData, stack) in _itemDatas)
            {
                Debug.Log($"{GetType()} - Adding... {itemData.Name}, {stack}");
                AddItemsToSlots(slots, itemData, stack, out int _);
            }
        }

        private void AddItemsToSlots(List<DraggableItemSlot> slots, ItemData itemData, int amount, out int totalStacked)
        {
            totalStacked = 0;
            int leftToStack = amount;

            // Looking for used slots first
            if (itemData.IsStackable)
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i].HasItem)
                    {
                        AddToSlot(slots[i], itemData, leftToStack, out int amountStacked);
                        totalStacked += amountStacked;
                        leftToStack -= amountStacked;

                        if (leftToStack == 0)
                        {
                            return;
                        }
                    }
                }
            }

            // Then empty slots
            for (int i = 0; i < slots.Count; i++)
            {
                if (!slots[i].HasItem)
                {
                    AddToSlot(slots[i], itemData, leftToStack, out int amountStacked);
                    totalStacked += amountStacked;
                    leftToStack -= amountStacked;

                    if (leftToStack == 0)
                    {
                        return;
                    }
                }
            }
        }

        private void Awake()
        {
            _playerInventorySlots = new(_playerInventorySlotsParent.childCount);
            _hotbarSlots = new(_inventoryHotbarSlotsParent.childCount);
            _storageSlots = new(_storageSlotsParent.childCount);

            _itemDatas = new(_playerInventorySlots.Capacity);

            GetSlots(_playerInventorySlots, _playerInventorySlotsParent);
            GetSlots(_hotbarSlots, _inventoryHotbarSlotsParent);
            GetSlots(_storageSlots, _storageSlotsParent);

            void GetSlots(List<DraggableItemSlot> slots, Transform slotsParent)
            {
                slots.Clear();

                foreach (Transform t in slotsParent)
                {
                    if (t.TryGetComponent(out DraggableItemSlot slot))
                    {
                        slots.Add(slot);
                    }
                }

            }
        }
    }
}
