using Coimbra;
using Core.Units;
using UnityEngine;

namespace Player
{
    public class Player : Actor
    {
        [SerializeField] private PlayerMovement2D _playerMovement;
        [SerializeField] private PlayerHold _playerHold;

        public void TryToMove(Vector2 direction)
        {
            _playerMovement.TryToMove(direction);
        }

        public void TryToInteract()
        {
        }

        public void TryToUseCurrentItem(Vector2 worldPosition)
        {
            _playerHold.TryUseEquipment(worldPosition);
        }
    }
}