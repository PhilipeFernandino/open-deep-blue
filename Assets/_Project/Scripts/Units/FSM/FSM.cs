using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.FSM
{
    [System.Serializable]
    public class FSM<T> where T : Enum
    {
        public Dictionary<T, IFSMState<T>> States { get; private set; }

        public IFSMState<T> State { get; private set; }
        public bool BlockTransition { get; set; } = false;

        [SerializeField] private string _state;

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
            _state = nextState.ToString();

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

        public void FixedUpdate()
        {
            State.FixedUpdate();
        }
    }
}