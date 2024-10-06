using Coimbra;
using Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.ItemSystem
{
    public class UIInventory : MonoBehaviour
    {
        [SerializeField] private List<UIItemCategory> _itemCategories;

        [SerializeField] private UIItemCategoryTab _itemCategoryTabPrefab;

        [SerializeField] private RectTransform _itemCategoryTabsParent;

        [SerializeField] private int _itemsPerRow = 6;

        private List<UIItemCategoryTab> _uiItemCategoryTabs;
        private IInventoryService _inventoryService;

        private void Start()
        {
            _inventoryService = ServiceLocatorUtilities.GetServiceAssert<IInventoryService>();
            _uiItemCategoryTabs = new();

            SetCategories(_itemCategories);
        }

        public void SelectCategory(ItemCategory category)
        {
            var showItems = _inventoryService.Filter(category: category);

        }

        public void SetCategories(List<UIItemCategory> categories)
        {
            foreach (Transform child in _itemCategoryTabsParent)
            {
                child.Dispose(true);
            }

            _uiItemCategoryTabs.Clear();

            foreach (UIItemCategory category in categories)
            {
                UIItemCategoryTab itemTab = Instantiate(_itemCategoryTabPrefab, _itemCategoryTabsParent);
                itemTab.Setup(category);
                itemTab.Selected += CategorySelectedEventHandler;
                _uiItemCategoryTabs.Add(itemTab);
            }
        }

        private void CategorySelectedEventHandler(ItemCategory category)
        {
            SelectCategory(category);
        }
    }

    [Serializable]
    public class UIItemCategory
    {
        [field: SerializeField] public ItemCategory Category { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}