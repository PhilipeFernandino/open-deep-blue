using Core.EventBus;
using Core.FSM;
using UnityEngine;

namespace Core.Units
{
    [System.Serializable]
    public class AntMovingState : IFSMState<AntState>
    {
        private Ant _fsm;
        private PositionEventBus _targetPositionEventBus;


        private Vector2 Position => _fsm.Position;


        public void Enter(IEnterStateData enterStateData)
        {
        }

        public void Exit()
        {
        }

        public void Initialize(IFSMAgent<AntState> fsmAgent)
        {
            _fsm = (Ant)fsmAgent;
        }

        public void FixedUpdate()
        {
            _fsm.MovementController.TryToMove(_fsm.Blackboard.MovingDirection);
        }
    }
}