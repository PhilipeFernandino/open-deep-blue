using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Player
{
    [RequireComponent(typeof(Player))]
    public class PlayerInputHandler : MonoBehaviour
    {
        private Player _player;
        private Camera _mainCamera;

        public void MoveInput(InputAction.CallbackContext context)
        {
            _player.MoveInput(context.ReadValue<Vector2>());
        }

        public void UseCurrentItemInput(InputAction.CallbackContext context)
        {
            _player.UseEquipmentInput(To2DWorldPosition(Mouse.current.position.ReadValue()));
        }

        public void InteractInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _player.InteractInput();
            }
        }

        public void DashInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _player.DashInput();
            }
        }

        private Vector2 To2DWorldPosition(Vector2 mousePosition)
        {
            var worldPosition = (Vector3)mousePosition;
            worldPosition.z = 10f;
            worldPosition = _mainCamera.ScreenToWorldPoint(worldPosition);
            return worldPosition.XY();
        }

        private void Awake()
        {
            _player = GetComponent<Player>();
            _mainCamera = Camera.main;
        }
    }
}
