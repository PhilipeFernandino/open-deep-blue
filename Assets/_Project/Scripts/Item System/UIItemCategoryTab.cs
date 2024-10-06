using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Core.ItemSystem
{
    public class UIItemCategoryTab : MonoBehaviour
    {
        [SerializeField] private Button _button;

        [SerializeField] private Image _iconImage;

        private ItemCategory _category;

        public event Action<ItemCategory> Selected;

        public void Setup(UIItemCategory itemCategory)
        {
            _iconImage.sprite = itemCategory.Icon;
            _category = itemCategory.Category;
        }

        private void Start()
        {
            _button.onClick.AddListener(ClickedEventHandler);
        }

        private void ClickedEventHandler()
        {
            OnSelect();
        }

        private void OnSelect()
        {
            Selected?.Invoke(_category);
        }
    }
}