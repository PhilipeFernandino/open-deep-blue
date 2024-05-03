using System;

namespace Core.FSM
{
    public interface IFSMState<T> where T : Enum
    {
        public void Enter(IEnterStateData enterStateData);
        public void Exit();
        public void Update();
        public void FixedUpdate() { }
        public void Initialize(IFSMAgent<T> fsmAgent);
    }
}

