using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.ItemSystem
{
    public class UIHotbarSlot : MonoBehaviour, IPointerClickHandler, ISelectHandler, IDeselectHandler
    {
        public UIInventoryItem Item { get; private set; }


        public void OnSelect(BaseEventData eventData)
        {

        }

        public void OnDeselect(BaseEventData eventData)
        {

        }

        public void OnPointerClick(PointerEventData eventData)
        {

        }

        public void Setup(UIInventoryItem item)
        {
            Item = item;
        }
    }
}