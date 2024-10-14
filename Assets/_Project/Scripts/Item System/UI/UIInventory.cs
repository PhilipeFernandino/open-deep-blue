using Coimbra;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Core.ItemSystem
{
    public class UIInventory : MonoBehaviour
    {
        [SerializeField] private List<UIItemCategory> _itemCategories;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _selectedItemTMP;
        [SerializeField] private UIItemActions _itemActions;

        [Header("Tab References")]
        [SerializeField] private UIItemCategoryTab _tabPrefab;
        [SerializeField] private RectTransform _tabsParent;

        [Header("Item References")]
        [SerializeField] private UIInventoryItem _itemPrefab;
        [SerializeField] private RectTransform _itemsLayoutPrefab;

        [SerializeField] private int _itemsPerLayout = 7;
        [SerializeField] private int _minLayoutCount = 7;

        [SerializeField] private RectTransform _itemsParent;

        private List<UIItemCategoryTab> _uiItemCategoryTabs;

        private List<RectTransform> _uiItemLayouts;
        private List<UIInventoryItem> _uiItems;

        private IInventoryService _inventoryService;

        public event Action<ItemAction> ItemActionRaised;

        private void Start()
        {
            _inventoryService = ServiceLocatorUtilities.GetServiceAssert<IInventoryService>();

            _uiItems = new();
            _uiItemLayouts = new();
            _uiItemCategoryTabs = new();

            _itemActions.ItemActionRaised += ItemActionRaised;

            SpawnLayouts(_minLayoutCount);

            SetCategories(_itemCategories);
        }

        private void SpawnLayouts(int spawnLayouts)
        {
            for (int i = 0; i < spawnLayouts; i++)
            {
                RectTransform layout = Instantiate(_itemsLayoutPrefab, _itemsParent);
                _uiItemLayouts.Add(layout);

                for (int j = 0; j < _itemsPerLayout; j++)
                {
                    UIInventoryItem item = Instantiate(_itemPrefab, layout);
                    _uiItems.Add(item);
                }
            }
        }

        public void SelectCategory(ItemCategory category)
        {
            var showItems = _inventoryService.Filter(category: category).ToList();

            RenderItems(showItems);
        }

        private void RenderItems(List<InventoryItem> showItems)
        {
            int showItemsCount = showItems.Count;
            int availableCapacity = _uiItemLayouts.Count * _itemsPerLayout;

            // If there's less room on pooled containers than items to show
            if (availableCapacity < showItemsCount)
            {
                int diff = showItemsCount - availableCapacity;
                int spawnLayouts = diff / _itemsPerLayout + (diff % _itemsPerLayout == 0 ? 0 : 1);

                SpawnLayouts(spawnLayouts);
            }

            int necessaryLayouts = Mathf.Max(showItemsCount / _itemsPerLayout + (showItemsCount % _itemsPerLayout == 0 ? 0 : 1), _minLayoutCount);

            // Activate layouts till necessary
            for (int i = 0; i < necessaryLayouts; i++)
            {
                _uiItemLayouts[i].gameObject.SetActive(true);
            }

            // Deactivate the rest
            for (int i = necessaryLayouts; i < _uiItemLayouts.Count; i++)
            {
                _uiItemLayouts[i].gameObject.SetActive(false);
            }

            // Activate items till count
            for (int i = 0; i < showItemsCount; i++)
            {
                _uiItems[i].Activate();
                _uiItems[i].Setup(showItems[i], ItemClickedEventHandler);
            }

            // Deactivate the rest
            for (int i = showItemsCount; i < _uiItems.Count; i++)
            {
                _uiItems[i].Deactivate();
            }
        }

        private void ItemClickedEventHandler(UIInventoryItem item)
        {
            _selectedItemTMP.text = item.Item.Name;

            if (_itemActions.InventoryItem == item)
            {
                _itemActions.Deactivate();
            }
            else
            {
                _itemActions.Setup(item, true);
            }
        }

        public void SetCategories(List<UIItemCategory> categories)
        {
            foreach (UIItemCategory category in categories)
            {
                UIItemCategoryTab itemTab = Instantiate(_tabPrefab, _tabsParent);
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