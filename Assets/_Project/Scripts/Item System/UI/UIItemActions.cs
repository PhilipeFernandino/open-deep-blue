using Core.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.ItemSystem
{
    [RequireComponent(typeof(RectTransform))]
    public class UIItemActions : UIDynamicCanvas, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Vector2 _positionOffset;

        [SerializeField] private List<ButtonItemAction> _actionTextButtons;

        public event Action<ItemActionRaisedEvent> ItemActionRaised;

        private RectTransform _rectTransform;
        private UIInventoryItem _inventoryItem;

        private bool _isMouseOverMenu = false;

        public UIInventoryItem InventoryItem
        {
            get => _inventoryItem;
            private set
            {
                if (_inventoryItem == value)
                {
                    return;
                }

                if (_inventoryItem != null)
                {
                    _inventoryItem.SetHighlight(false);

                }

                if (value != null)
                {
                    value.SetHighlight(true);
                }

                _inventoryItem = value;
            }
        }

        public void Setup(UIInventoryItem item, bool activate = false)
        {
            _rectTransform.position = item.RectTransform.position.XY() + _positionOffset;
            InventoryItem = item;

            if (activate)
            {
                Activate();
            }
        }

        public void Activate()
        {
            ShowSelf();
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void Deactivate()
        {
            HideSelf();
            InventoryItem = null;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!_isMouseOverMenu)
                Deactivate();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isMouseOverMenu = true;
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isMouseOverMenu = false;
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        protected override void Awake()
        {
            base.Awake();

            _rectTransform = GetComponent<RectTransform>();

            SetupActionButtons();
        }

        private void SetupActionButtons()
        {
            for (int i = 0; i < _actionTextButtons.Count; i++)
            {
                var button = _actionTextButtons[i];

                button.TextButton.Button.onClick.AddListener(
                    () =>
                    {
                        var inventoryItem = InventoryItem;
                        Deactivate();
                        ItemActionRaised?.Invoke(new ItemActionRaisedEvent(inventoryItem, button.Action));
                    });
            }
        }

        private void Start()
        {
            Deactivate();
        }

        [Serializable]
        private class ButtonItemAction { public ItemAction Action; public TextBtn TextButton; }
    }

    public enum ItemAction
    {
        Equip,
        Discard,
        Favorite
    }

    public record ItemActionRaisedEvent(UIInventoryItem Item, ItemAction Action)
    {
        public UIInventoryItem Item { get; private set; } = Item;
        public ItemAction Action { get; private set; } = Action;
    }
}