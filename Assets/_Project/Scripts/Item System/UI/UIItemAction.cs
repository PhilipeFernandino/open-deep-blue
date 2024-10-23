using Core.UI;
using System;
using System.Collections;
using UnityEngine;

namespace Core.ItemSystem
{
    public class UIItemAction : MonoBehaviour
    {
        [SerializeField] TextBtn _textButton;

        public ItemAction Action { get; set; }

        public event Action<ItemAction> OnClick;

        public void Setup(ItemAction action)
        {
            Action = action;
            _textButton.Text = action.ToString();
            Activate();
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            _textButton.Button.onClick.AddListener(() => OnClick?.Invoke(Action));
        }
    }
}