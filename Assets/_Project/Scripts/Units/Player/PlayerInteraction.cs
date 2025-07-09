using Core.Util;
using UnityEngine;

namespace Core.Interaction
{
    [RequireComponent(typeof(Player.Player))]
    public class PlayerInteraction : MonoBehaviour
    {
        private Player.Player _player;
        private IInteractionService _interactionService;

        public bool TryInteract()
        {
            return _interactionService.TryInteract(_player.Position + _player.FacingDirection * 1f, _player);
        }

        private void Start()
        {
            _player = GetComponent<Player.Player>();
            _interactionService = ServiceLocatorUtilities.GetServiceAssert<IInteractionService>();
        }
    }
}