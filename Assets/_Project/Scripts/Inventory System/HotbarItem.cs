using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.InventorySystem
{
    public class HotbarItem : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _stackTMP;

        private Item _item;

        public void Setup(Item item)
        {
            _item = item;
            _iconImage.sprite = item.ItemData.Sprite;
            _stackTMP.text = item.Stack.ToString();
        }

        public void HoldItem() => _item.HoldItem();
    }
}