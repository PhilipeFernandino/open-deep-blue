using Core.FSM;
using UnityEngine;

namespace Core.Player
{
    public abstract class PlayerStateBase : IPlayerFSMState
    {
        protected Player _fsmAgent;

        public abstract void Enter(IEnterStateData enterStateData);
        public abstract void Exit();
        public abstract void Update();

        public virtual void MoveInput(Vector2 input) { }
        public virtual void UseEquipmentInput(Vector2 input) { }
        public virtual void InteractInput() { }

        public virtual void Initialize(IFSMAgent<PlayerState> fsmAgent)
        {
            _fsmAgent = (Player)fsmAgent;
        }
    }
}