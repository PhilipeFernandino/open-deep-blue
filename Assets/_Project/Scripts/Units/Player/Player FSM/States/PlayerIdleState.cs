using Core.FSM;
using UnityEngine;

namespace Core.Player
{
    public class PlayerIdleState : PlayerStateBase
    {
        public override void Enter(IEnterStateData enterStateData)
        {
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
        }

        public override void MoveInput(Vector2 input)
        {
            Debug.Log($"{GetType()} - {input}");
            _fsmAgent.TransferState(PlayerState.Moving, null, this);
        }
    }
}