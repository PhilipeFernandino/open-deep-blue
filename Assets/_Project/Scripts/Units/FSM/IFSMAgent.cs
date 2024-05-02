
using System;
using System.Collections.Generic;

namespace Core.FSM
{
    public interface IFSMAgent<T> where T : Enum
    {
        public Dictionary<T, IFSMState<T>> States { get; }

        public void TransferState(T nextState, IEnterStateData enterStateData, IFSMState<T> actor);
    }
}