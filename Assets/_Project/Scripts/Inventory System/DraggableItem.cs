
using Coimbra;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils.UI;

namespace Core.InventorySystem
{
    public class DraggableItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _stackTMP;

        private bool _isDragging;
        private Item _item;
        private Transform _parentWhenDragging;

        public int Stack => _item.Stack;
        public int MaxStack => _item.MaxStack;
        public int StackCurrentCapacity => _item.StackCurrentCapacity;
        public ItemData ItemData => _item.ItemData;
        public Item Item => _item;

        public event Action DragStarting;

        public int TryStack(ItemData item, int amount) => _item.TryStack(item, amount);
        public int TryTakeFromStack(int amount) => _item.TryTakeFromStack(amount);

        public void Setup(ItemData itemData, Transform parentWhenDragging, bool startDragging = false)
        {
            _item = new Item(itemData);
            _parentWhenDragging = parentWhenDragging;
            _iconImage.sprite = itemData.Sprite;
            _item.StackChanged += StackChangedEventHandler;

            if (startDragging)
            {
                StartDrag();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isDragging)
            {
                PointerClickWhileDraggingEventHandler(eventData);
            }
            else
            {
                PointerClickNotDraggingEventHandler(eventData);
            }
        }

        private void PointerClickNotDraggingEventHandler(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                // Try to grab the whole stack
                case PointerEventData.InputButton.Left:
                {
                    StartDrag();
                    break;
                }

                // Try to grab a single item
                case PointerEventData.InputButton.Right:
                {
                    if (Stack > 1)
                    {
                        int newItemStack = Stack;
                        TryTakeFromStack(Stack - (Stack / 2));
                        newItemStack -= Stack;
                        ScriptableSettings.Get<DraggableItemFactory>()
                            .Create(
                            ItemData,
                            newItemStack,
                            _parentWhenDragging,
                            startDragging: true);
                    }
                    else
                    {
                        StartDrag();
                    }
                    break;
                }
            }
        }

        private void PointerClickWhileDraggingEventHandler(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                // Drop the whole stack
                case PointerEventData.InputButton.Left:
                {
                    DropItem();
                    break;
                }

                // Try to drop a single unit
                case PointerEventData.InputButton.Right:
                {
                    if (Stack > 1)
                    {
                        if (TryGetInventorySlotRaycast(out DraggableItemSlot slot))
                        {
                            int amountAdded = slot.TrySetOrAddToItemStack(
                                _item.ItemData,
                                1,
                                _parentWhenDragging);

                            if (amountAdded > 0)
                            {
                                TryTakeFromStack(1);
                            }
                        }
                    }
                    else
                    {
                        DropItem();
                    }
                    break;
                }
            }
        }


        private void Update()
        {
            TryDrag();
        }

        private void StartDrag()
        {
            DragStarting?.Invoke();

            Debug.Log($"{GetType()} - Start Drag");

            _isDragging = true;
            transform.SetParent(_parentWhenDragging, false);


            TryDrag();
        }

        private void TryDrag()
        {
            if (_isDragging)
            {
                transform.position = Input.mousePosition;
            }
        }

        private void DropItem()
        {
            if (TryGetInventorySlotRaycast(out DraggableItemSlot inventorySlot))
            {
                int amountAdded = inventorySlot.TrySetOrAddToItemStack(_item.ItemData, Stack, _parentWhenDragging);

                if (amountAdded == Stack)
                {
                    gameObject.Dispose(false);
                }
                else
                {
                    TryTakeFromStack(amountAdded);
                }
            }
        }

        private void StackChangedEventHandler()
        {
            _stackTMP.text = Stack.ToString();
        }

        private bool TryGetInventorySlotRaycast(out DraggableItemSlot inventorySlot)
        {
            var obj = UIRaycastUtilities.UIRaycastAllTryGetComponent(Input.mousePosition, out inventorySlot);
            Debug.Log($"{GetType()} - {obj}");
            return obj;
        }
    }
}