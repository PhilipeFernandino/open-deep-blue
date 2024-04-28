using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Player))]
    public class PlayerInputHandler : MonoBehaviour
    {
        private Player _player;

        public void MoveInput(InputAction.CallbackContext context)
        {
            _player.TryToMove(context.ReadValue<Vector2>());
        }

        public void UseCurrentItemInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _player.TryToUseCurrentItem(Input.mousePosition);
            } 
        }

        public void InteractInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _player.TryToInteract();
            }
        }

        private void Awake()
        {
            _player = GetComponent<Player>();
        }
    }
}
