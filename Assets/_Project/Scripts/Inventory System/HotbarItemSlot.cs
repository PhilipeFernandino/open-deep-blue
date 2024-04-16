using Coimbra;
using UnityEngine;

namespace Core.InventorySystem
{
    public class HotbarItemSlot : MonoBehaviour
    {
        private HotbarItem _hotbarItem;

        public HotbarItem HotbarItem
        {
            get => _hotbarItem;
            set
            {
                if (_hotbarItem == value)
                {
                    return;
                }


                if (value == null)
                {
                    _hotbarItem.gameObject.Dispose(true);
                    _hotbarItem = null;
                }
                else
                {
                    _hotbarItem = value;
                    _hotbarItem.transform.SetParent(transform, false);
                }
            }
        }

        public void Setup(HotbarItem hotbarItem)
        {
            HotbarItem = hotbarItem;
        }

        public void Setup(Item item)
        {
            HotbarItem = ScriptableSettings.Get<HotbarItemFactory>().Create(item);
        }

        public void Empty()
        {
            HotbarItem = null;
        }

        public void HoldItem() => _hotbarItem.HoldItem();
    }
}