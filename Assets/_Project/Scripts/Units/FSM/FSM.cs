using System;
using System.Collections.Generic;

namespace Core.FSM
{
    public class FSM<T> where T : Enum
    {
        public Dictionary<T, IFSMState<T>> States { get; private set; }

        public IFSMState<T> State { get; private set; }
        public bool BlockTransition { get; set; } = false;

        public FSM(Dictionary<T, IFSMState<T>> states, IFSMAgent<T> agent)
        {
            States = states;

            foreach (var state in States.Values)
            {
                state.Initialize(agent);
            }
        }

        public K GetState<K>(T state)
        {
            return (K)States[state];
        }

        public void TransferState(T nextState, IEnterStateData enterStateData, IFSMState<T> actor)
        {
            if (!BlockTransition)
            {
                State?.Exit();
                State = States[nextState];
                State.Enter(enterStateData);
            }
        }

        public void Update()
        {
            State.Update();
        }
    }
}