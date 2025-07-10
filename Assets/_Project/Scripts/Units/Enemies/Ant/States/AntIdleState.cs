using Core.Events;
using Core.FSM;
using UnityEngine;

namespace Core.Units
{
    public class AntIdleState : IFSMState<AntState>
    {
        private Ant _fsmAgent;
        private Vector2EventChannelSO _targetPositionEventBus;

        public void Enter(IEnterStateData enterStateData)
        {
            _targetPositionEventBus.OnEventRaised += TargetPositionChanged_EventHandler;
        }

        public void Exit()
        {
            _targetPositionEventBus.OnEventRaised -= TargetPositionChanged_EventHandler;
        }

        public void Initialize(IFSMAgent<AntState> fsmAgent)
        {
            _fsmAgent = (Ant)fsmAgent;
        }

        private void TargetPositionChanged_EventHandler(Vector2 vector)
        {
            if (Vector2.Distance(_fsmAgent.Position, vector) < _fsmAgent.AggroDistance)
            {
                _fsmAgent.TransferState(AntState.Moving, null, this);
            }
        }
    }
}