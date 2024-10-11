using Core.UI;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.ItemSystem
{
    [RequireComponent(typeof(RectTransform))]
    public class UIItemActions : MonoBehaviour, IDeselectHandler
    {
        [SerializeField] private Vector2 _positionOffset;

        [SerializeField] private TextBtn _equipTextButton;
        [SerializeField] private TextBtn _discardTextButton;
        [SerializeField] private TextBtn _favoriteTextButton;

        public event Action<UIInventoryItem> EquipTextButton;
        public event Action<UIInventoryItem> FavoriteTextButton;
        public event Action<UIInventoryItem> DiscardTextButton;

        private RectTransform _rectTransform;
        private UIInventoryItem _inventoryItem;

        public UIInventoryItem InventoryItem => _inventoryItem;

        public void Setup(UIInventoryItem item, bool activate = false)
        {
            _inventoryItem = item;
            _rectTransform.position = item.RectTransform.position.XY() + _positionOffset;

            if (activate)
            {
                Activate();
            }
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            _inventoryItem = null;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Deactivate();
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            _equipTextButton.Button.onClick.AddListener(() => EquipTextButton?.Invoke(_inventoryItem));
            _discardTextButton.Button.onClick.AddListener(() => EquipTextButton?.Invoke(_inventoryItem));
            _favoriteTextButton.Button.onClick.AddListener(() => EquipTextButton?.Invoke(_inventoryItem));
        }

        private void Start()
        {
            Deactivate();
        }
    }
}