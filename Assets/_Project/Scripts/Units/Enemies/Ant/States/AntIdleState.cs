using Core.FSM;
using Core.EventBus;
using UnityEngine;

namespace Core.Units
{
    public class AntIdleState : IFSMState<AntState>
    {
        private Ant _fsmAgent;
        private PositionEventBus _targetPositionEventBus;

        public void Enter(IEnterStateData enterStateData)
        {
            _targetPositionEventBus.PositionChanged += TargetPositionChanged_EventHandler;
            TargetPositionChanged_EventHandler(_fsmAgent.PositionEventBus.Position);
        }

        public void Exit()
        {
            _targetPositionEventBus.PositionChanged -= TargetPositionChanged_EventHandler;
        }

        public void Initialize(IFSMAgent<AntState> fsmAgent)
        {
            _fsmAgent = (Ant)fsmAgent;
            _targetPositionEventBus = _fsmAgent.PositionEventBus;
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