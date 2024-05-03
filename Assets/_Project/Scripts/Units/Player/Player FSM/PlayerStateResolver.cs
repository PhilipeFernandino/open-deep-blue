using Core.FSM;
using UnityEngine;

namespace Core.Player
{
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
                _player.TransferState(PlayerState.UsingEquipment, new UsingEquipmentEnterStateData(input, result.effect), actor);
            }
        }
    }
}