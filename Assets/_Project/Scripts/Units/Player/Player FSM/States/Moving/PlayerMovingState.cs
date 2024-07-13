using Core.FSM;
using UnityEngine;

namespace Core.Player
{
    public class PlayerMovingState : PlayerStateBase
    {
        public override void Enter(IEnterStateData enterStateData)
        {
            _fsmAgent.PlayerAnimator.StartWalking();
        }

        public override void Exit()
        {
            _fsmAgent.PlayerAnimator.StopWalking();
            _fsmAgent.PlayerMovement.ResetMovement();
        }

        public override void Update()
        {
        }

        public override void UseEquipmentInput(Vector2 input) => _fsmAgent.StateResolver.UseEquipmentInput(input, this);

        public override void MoveInput(Vector2 input)
        {
            _fsmAgent.PlayerMovement.TryToMove(input);
            _fsmAgent.PlayerAnimator.WalkingDirectionInput(input);
        }

        public override void DashInput() => _fsmAgent.StateResolver.DashInput(this);
    }
}