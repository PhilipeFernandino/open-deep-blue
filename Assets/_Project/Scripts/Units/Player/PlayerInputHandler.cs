using Core.Events;
using Core.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Player
{
    [RequireComponent(typeof(Player))]
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private Vector2EventChannelSO _moveEventChannel;
        [SerializeField] private VoidEventChannelSO _useCurrentItemEventChannel;
        [SerializeField] private VoidEventChannelSO _dashEventChannel;
        [SerializeField] private VoidEventChannelSO _interactEventChannel;

        private Player _player;
        private PlayerInteraction _playerInteraction;
        private Camera _mainCamera;

        private void Awake()
        {
            _player = GetComponent<Player>();
            _playerInteraction = GetComponent<PlayerInteraction>();
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            _moveEventChannel.OnEventRaised += OnMove;
            _dashEventChannel.OnEventRaised += OnDash;
            _interactEventChannel.OnEventRaised += OnInteract;
            _useCurrentItemEventChannel.OnEventRaised += OnUseCurrentItem;
        }

        private void OnDisable()
        {
            _moveEventChannel.OnEventRaised -= OnMove;
            _dashEventChannel.OnEventRaised -= OnDash;
            _interactEventChannel.OnEventRaised -= OnInteract;
            _useCurrentItemEventChannel.OnEventRaised -= OnUseCurrentItem;
        }

        private void OnUseCurrentItem()
        {
            _player.UseEquipmentInput(To2DWorldPosition(Mouse.current.position.ReadValue()));
        }

        private void OnMove(Vector2 direction)
        {
            _player.MoveInput(direction);
        }

        private void OnDash()
        {
            _player.DashInput();
        }

        private void OnInteract()
        {
            _playerInteraction.TryInteract();
        }


        private Vector2 To2DWorldPosition(Vector2 mousePosition)
        {
            var worldPosition = (Vector3)mousePosition;
            worldPosition.z = 10f;
            worldPosition = _mainCamera.ScreenToWorldPoint(worldPosition);
            return worldPosition.XY();
        }
    }
}