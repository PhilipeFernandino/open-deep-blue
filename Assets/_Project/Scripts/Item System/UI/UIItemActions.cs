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
        [Header("References")]
        [SerializeField] private UISelectItem _uiSelectItem;
        [SerializeField] private UIItemAction _buttonPrefab;
        [SerializeField] private RectTransform _buttonsParent;

        [Header("Settings")]
        [SerializeField] private Vector2 _positionOffset;

        private List<UIItemAction> _actionTextButtons;

        private RectTransform _rectTransform;

        private bool _isMouseOverMenu = false;


        public event Action<ItemActionRaisedEvent> ItemActionRaised;

        public UIInventoryItem SelectedItem => _uiSelectItem.SelectedItem;


        public void Setup(UIInventoryItem item, ItemAction[] actions, bool activate = false)
        {
            SetupButtons(actions);

            _rectTransform.position = item.RectTransform.position.XY() + _positionOffset;
            _uiSelectItem.SelectItem(item);

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
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!_isMouseOverMenu)
            {
                Deactivate();
            }
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

            InstantiateButtons();
        }

        private void InstantiateButtons()
        {
            _actionTextButtons = new(10);

            for (int i = 0; i < _actionTextButtons.Capacity; i++)
            {
                var itemAction = Instantiate(_buttonPrefab, _buttonsParent.transform);
                _actionTextButtons.Add(itemAction);

                _actionTextButtons[i].OnClick += (action) =>
                {
                    var inventoryItem = SelectedItem;
                    _uiSelectItem.Deselect();
                    Deactivate();
                    ItemActionRaised?.Invoke(new ItemActionRaisedEvent(inventoryItem, action));
                };
            }
        }

        private void SetupButtons(ItemAction[] actions)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                _actionTextButtons[i].Setup(actions[i]);
            }

            for (int i = actions.Length; i < _actionTextButtons.Count; i++)
            {
                _actionTextButtons[i].Deactivate();
            }
        }

        private void Start()
        {
            Deactivate();
        }
    }

    public enum ItemAction
    {
        Equip,
        Unequip,
        Sell,
        Discard,
        Favorite,
    }

    public record ItemActionRaisedEvent(UIInventoryItem Item, ItemAction Action)
    {
        public UIInventoryItem Item { get; private set; } = Item;
        public ItemAction Action { get; private set; } = Action;
    }
}