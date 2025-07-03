using Core.FSM;
using UnityEngine;

namespace Core.Player
{
    public class PlayerIdleState : PlayerStateBase
    {
        public override void UseEquipmentInput(Vector2 input) => _fsmAgent.StateResolver.MoveInput(input, this);
        public override void MoveInput(Vector2 input) => _fsmAgent.StateResolver.MoveInput(input, this);
        public override void DashInput() => _fsmAgent.StateResolver.DashInput(this);
    }
}