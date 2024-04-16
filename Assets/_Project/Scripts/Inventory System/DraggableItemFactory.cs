using Coimbra;
using UnityEngine;

namespace Core.InventorySystem
{
    [ProjectSettings("Game Settings")]
    public class DraggableItemFactory : ScriptableSettings
    {
        [SerializeField] private DraggableItem _draggableItemPrefab;

        public (DraggableItem item, int added) Create(
            ItemData itemData,
            int stack,
            Transform parentWhileDragging,
            Transform imediateParent = null,
            bool startDragging = false
            )
        {
            var draggableItem = Instantiate(_draggableItemPrefab, imediateParent);
            draggableItem.Setup(itemData, parentWhileDragging, startDragging);
            int added = draggableItem.TryStack(itemData, stack);
            return (draggableItem, added);
        }
    }
}