using UnityEditor;
using UnityEngine;

namespace Core.ItemSystem
{
    public class UISelectItem : MonoBehaviour
    {
        private UIInventoryItem _selectedItem;

        public UIInventoryItem SelectedItem => _selectedItem;

        public void SelectItem(UIInventoryItem item)
        {

            if (_selectedItem == item)
            {
                return;
            }

            if (_selectedItem != null)
            {
                _selectedItem.SetHighlight(false);
            }

            if (item != null)
            {
                Debug.Log($"{GetType()} - Select {item.name}");
                item.SetHighlight(true);
            }

            _selectedItem = item;
        }

        public void Deselect()
        {
            Debug.Log($"{GetType()} - Deselect");
            SelectItem(null);
        }
    }
}