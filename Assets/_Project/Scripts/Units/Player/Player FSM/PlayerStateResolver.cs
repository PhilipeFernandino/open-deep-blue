using Core.FSM;
using UnityEngine;

namespace Core.Player
{
    /// <summary>
    /// This class serves as a common resolver for input and other general handling of events.
    /// Every state that wants to deal with the movement input for example can use this.
    /// Not every state will handle every input, so we can't put this on an abstract class for 
    /// the states. A dashing player can't move, but an idle or dancing player can.
    /// On the other hand, the input can be handled by the state itself. Take the movement 
    /// input, for example. Having this class as a separate component allows reusability and
    /// flexibility at the same time.
    /// </summary>
    public class PlayerStateResolver
    {
        private Player _player;

        public void Setup(Player player)
        {
            _player = player;
        }

        public void MoveInput(Vector2 input, IFSMState<PlayerState> actor)
        {
            Debug.Log($"{GetType()} - {input}");
            _player.TransferState(PlayerState.Moving, null, actor);
        }

        public void UseEquipmentInput(Vector2 input, IFSMState<PlayerState> actor)
        {
            var result = _player.PlayerHold.TryUseEquipment(input);
            if (result.success)
            {
                _player.TransferState(
                    PlayerState.UsingEquipment,
                    new UsingEquipmentEnterStateData(input, result.effect), actor);
            }
        }

        public void DashInput(IFSMState<PlayerState> actor)
        {
            Vector2 direction = _player.PlayerMovementDirection;

            _player.TransferState(
                PlayerState.Dashing,
                new DashingEnterStateData(
                    direction,
                    _player.DashSpeed,
                    _player.DashDuration),
                actor);
        }
    }
}