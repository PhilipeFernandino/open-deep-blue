using Core.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Player
{
    [CreateAssetMenu(menuName = "Core/Input/Input Proxy")]
    public class InputProxySO : ScriptableObject
    {
        [Header("Player Event Channels")]
        [SerializeField] private Vector2EventChannelSO _moveEventChannel;
        [SerializeField] private VoidEventChannelSO _dashEventChannel;
        [SerializeField] private VoidEventChannelSO _interactEventChannel;
        [SerializeField] private VoidEventChannelSO _useCurrentItemEventChannel;

        [Header("Inventory Event Channels")]
        [SerializeField] private CallbackContextEventChannelSO _hotbarSelectEventChannel;
        [SerializeField] private Vector2EventChannelSO _hotbarScrollEventChannel;
        [SerializeField] private VoidEventChannelSO _toggleInventoryEventChannel;


        public void OnMove(InputAction.CallbackContext context)
        {
            if (_moveEventChannel != null)
            {
                _moveEventChannel.RaiseEvent(context.ReadValue<Vector2>());
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed && _dashEventChannel != null)
            {
                _dashEventChannel.RaiseEvent();
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed && _interactEventChannel != null)
            {
                _interactEventChannel.RaiseEvent();
            }
        }

        public void OnUseCurrentItem(InputAction.CallbackContext context)
        {
            if (context.performed && _useCurrentItemEventChannel != null)
            {
                _useCurrentItemEventChannel.RaiseEvent();
            }
        }

        public void OnToggleInventory(InputAction.CallbackContext context)
        {
            if (context.performed && _toggleInventoryEventChannel != null)
            {
                _toggleInventoryEventChannel.RaiseEvent();
            }
        }

        public void OnHotbarSelect(InputAction.CallbackContext context)
        {
            if (context.performed && _hotbarSelectEventChannel != null)
                _hotbarSelectEventChannel.RaiseEvent(context);
        }

        public void OnHotbarScroll(InputAction.CallbackContext context)
        {
            if (_hotbarScrollEventChannel != null)
                _hotbarScrollEventChannel.RaiseEvent(context.ReadValue<Vector2>());
        }
    }
}
