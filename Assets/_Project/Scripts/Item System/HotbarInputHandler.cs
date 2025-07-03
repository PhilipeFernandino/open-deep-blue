using Core.ItemSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Item_System
{
    public class HotbarInputHandler : MonoBehaviour
    {
        [SerializeField] private Hotbar _hotbar;

        public void HotbarNavigation(InputAction.CallbackContext context)
        {
            if (context.performed && int.TryParse(context.control.name, out int value))
            {
                if (value == 0)
                {
                    _hotbar.SelectedHotbarIndex = 9;
                }
                else
                {
                    _hotbar.SelectedHotbarIndex = value - 1;
                }
            }
        }

        public void HotbarNavigationWithScroll(InputAction.CallbackContext context)
        {
            float yScroll = context.ReadValue<Vector2>().y;

            if (yScroll > 0)
            {
                _hotbar.SelectedHotbarIndex -= 1;
            }
            else if (yScroll < 0)
            {
                _hotbar.SelectedHotbarIndex += 1;
            }
        }
    }
}