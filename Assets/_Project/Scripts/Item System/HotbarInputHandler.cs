using Core.Events;
using Core.ItemSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Item_System
{
    public class HotbarInputHandler : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private Hotbar _hotbar;

        [Header("Listen to Event Channels")]
        [SerializeField] private CallbackContextEventChannelSO _hotbarSelectEventChannel;
        [SerializeField] private Vector2EventChannelSO _hotbarScrollEventChannel;

        private void OnEnable()
        {
            if (_hotbarSelectEventChannel != null)
            {
                _hotbarSelectEventChannel.OnEventRaised += HandleHotbarSelection;
            }
            if (_hotbarScrollEventChannel != null)
            {
                _hotbarScrollEventChannel.OnEventRaised += HandleHotbarScroll;
            }
        }

        private void OnDisable()
        {
            if (_hotbarSelectEventChannel != null)
            {
                _hotbarSelectEventChannel.OnEventRaised -= HandleHotbarSelection;
            }
            if (_hotbarScrollEventChannel != null)
            {
                _hotbarScrollEventChannel.OnEventRaised -= HandleHotbarScroll;
            }
        }

        private void HandleHotbarSelection(InputAction.CallbackContext context)
        {
            if (int.TryParse(context.control.name, out int value))
            {
                _hotbar.SelectedHotbarIndex = value == 0 ? 9 : value - 1;
            }
        }

        private void HandleHotbarScroll(Vector2 scroll)
        {
            if (scroll.y > 0)
            {
                _hotbar.SelectedHotbarIndex -= 1;
            }
            else if (scroll.y < 0)
            {
                _hotbar.SelectedHotbarIndex += 1;
            }
        }
    }
}