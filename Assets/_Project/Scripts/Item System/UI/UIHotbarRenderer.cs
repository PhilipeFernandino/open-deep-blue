using Core.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.ItemSystem
{
    public class UIHotbarRenderer : UIDynamicCanvas
    {
        [SerializeField] private List<UIInventoryItem> _slots = new();

        public event Action<(UIInventoryItem item, int index)> SlotClicked;

        public int SlotCount => _slots.Count;

        public UIInventoryItem GetSlot(int index)
        {
            return _slots[index];
        }

        public void SetupSlot(HotbarUpdateEventArgs e)
        {
            Debug.Log(e);
            _slots[e.Index].Setup(e.Item);
        }

        public void Activate()
        {
            ShowSelf();
        }

        public void Deactivate()
        {
            HideSelf();
        }

        private void Start()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                int index = i;
                _slots[i].Clicked += (item) => SlotClicked?.Invoke((item, index));
            }
        }
    }
}