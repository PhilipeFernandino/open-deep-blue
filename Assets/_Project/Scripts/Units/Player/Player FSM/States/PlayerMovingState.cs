using Core.FSM;
using UnityEngine;

namespace Core.Player
{
    public class PlayerMovingState : PlayerStateBase
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
            _fsmAgent.PlayerMovement.TryToMove(input);
        }
    }
}