using Coimbra;
using UnityEngine;

namespace Core.InventorySystem
{
    [ProjectSettings("Game Settings")]
    public class HotbarItemFactory : ScriptableSettings
    {
        [SerializeField] private HotbarItem _hotbarItemPrefab;

        public HotbarItem Create(Item item)
        {
            var hotbarItem = Instantiate(_hotbarItemPrefab);
            hotbarItem.Setup(item);
            return hotbarItem;
        }
    }
}