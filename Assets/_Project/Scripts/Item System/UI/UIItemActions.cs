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
    public class UIItemActions : MonoBehaviour, IDeselectHandler
    {
        [SerializeField] private Vector2 _positionOffset;

        [SerializeField] private List<ButtonItemAction> _actionTextButtons;

        public event Action<ItemActionRaisedEvent> ItemActionRaised;

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

        public async void OnDeselect(BaseEventData eventData)
        {
            // TODO? 
            await UniTask.Delay(100);
            Deactivate();
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            for (int i = 0; i < _actionTextButtons.Count; i++)
            {
                var button = _actionTextButtons[i];

                button.TextButton.Button.onClick.AddListener(
                    () =>
                    {
                        Debug.Log("ok");
                        ItemActionRaised?.Invoke(new ItemActionRaisedEvent(InventoryItem, button.Action));

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