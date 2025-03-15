using Coimbra;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.ItemSystem
{
    public class UIInventoryItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _rarityImage;
        [SerializeField] private Image _highlightImage;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _quantityTMP;

        public event Action<UIInventoryItem> Clicked;

        private InventoryItem _item;
        private RectTransform _rectTransform;

        public bool HasItem => _item is not null;
        public InventoryItem Item => _item;
        public RectTransform RectTransform => _rectTransform;

        public void Setup(InventoryItem item, Action<UIInventoryItem> onClickCallback = null)
        {
            Debug.Log(_item);

            if (item == null)
            {
                _item = null;
                Deactivate();
                return;
            }

            _icon.sprite = item.Icon;
            _quantityTMP.text = item.Amount > 1 ? item.Amount.ToString() : "";

            var colorSettings = ScriptableSettings.GetOrFind<UIItemRarityConfig>();

            _rarityImage.color = colorSettings.ItemRarityColor[item.Rarity];

            Clicked += onClickCallback;

            _item = item;

            Activate();
        }

        public void Activate()
        {
            _icon.enabled = true;
            _rarityImage.enabled = true;
        }

        public void Deactivate()
        {
            _icon.enabled = false;
            _rarityImage.enabled = false;
            _quantityTMP.text = "";
            SetHighlight(false);
        }

        public void SetHighlight(bool value)
        {
            _highlightImage.enabled = value;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(this);
        }

        private void Awake()
        {
            _item = null;
            Deactivate();
            _rectTransform = GetComponent<RectTransform>();
        }
    }
}