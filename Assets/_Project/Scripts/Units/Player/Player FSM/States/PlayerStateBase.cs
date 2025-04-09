using Core.FSM;
using UnityEngine;

namespace Core.Player
{
    public abstract class PlayerStateBase : IPlayerFSMState
    {
        protected Player _fsmAgent;

        public virtual void Enter(IEnterStateData enterStateData) { }
        public virtual void Update() { }
        public virtual void Exit() { }
        public virtual void FixedUpdate() { }

        public virtual void MoveInput(Vector2 input) { }
        public virtual void UseEquipmentInput(Vector2 input) { }
        public virtual void InteractInput() { }
        public virtual void DashInput() { }

        public virtual void Initialize(IFSMAgent<PlayerState> fsmAgent)
        {
            _fsmAgent = (Player)fsmAgent;
        }
    }
}