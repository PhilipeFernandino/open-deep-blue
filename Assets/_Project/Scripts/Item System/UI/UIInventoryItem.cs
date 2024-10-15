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
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _quantityTMP;

        public event Action<UIInventoryItem> Clicked;

        private InventoryItem _item = null;
        private RectTransform _rectTransform;

        public InventoryItem Item => _item;
        public RectTransform RectTransform => _rectTransform;

        public void Setup(InventoryItem item, Action<UIInventoryItem> onClickCallback = null)
        {
            _icon.sprite = item.Icon;
            _quantityTMP.text = item.Amount > 1 ? item.Amount.ToString() : "";

            var colorSettings = ScriptableSettings.Get<UIItemRarityConfig>();

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
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(this);
        }

        private void Awake()
        {
            Deactivate();
            _rectTransform = GetComponent<RectTransform>();
        }
    }
}