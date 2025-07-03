using Core.FSM;
using UnityEngine;

namespace Core.Player
{
    public interface IPlayerFSMState : IFSMState<PlayerState>
    {
        public void MoveInput(Vector2 input) { }
        public void UseEquipmentInput(Vector2 input) { }
        public void DashInput() { }
        public void InteractInput() { }
    }
}