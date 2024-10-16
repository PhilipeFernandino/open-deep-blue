using Core.Utils;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.ItemSystem.Test
{
    public class InventoryTest : MonoBehaviour
    {
        [SerializeField] private List<Sprite> _icons = new();
        [SerializeField] private int _addRandomItems = 0;

        private IInventoryService _inventoryService;

        private void Start()
        {
            _inventoryService = ServiceLocatorUtilities.GetServiceAssert<IInventoryService>();
            AddItems();
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void AddItems()
        {
            List<InventoryItem> addItems = new(_addRandomItems);

            for (int i = 0; i < _addRandomItems; i++)
            {
                ItemSO itemSO = (ItemSO)ScriptableObject.CreateInstance(typeof(ItemSO));
                itemSO.Setup(
                    UnityEngine.Random.Range(1, 1000).ToString(),
                    (ItemRarity)UnityEngine.Random.Range(1, Enum.GetValues(typeof(ItemRarity)).Length),
                    (ItemCategory)UnityEngine.Random.Range(1, Enum.GetValues(typeof(ItemCategory)).Length),
                   _icons[UnityEngine.Random.Range(0, _icons.Count - 1)],
                   true);

                InventoryItem item = new(itemSO, 0, UnityEngine.Random.Range(1, 64), false);
                addItems.Add(item);
            }

            _inventoryService.AddItems(addItems);
        }
    }
}