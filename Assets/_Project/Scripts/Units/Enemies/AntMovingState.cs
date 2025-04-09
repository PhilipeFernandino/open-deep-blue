using Core.EventBus;
using Core.FSM;

namespace Core.Units
{
    public class AntMovingState : IFSMState<AntState>
    {
        private AntEnemy _fsmAgent;

        public void Enter(IEnterStateData enterStateData)
        {
        }

        public void Exit()
        {
        }

        public void Initialize(IFSMAgent<AntState> fsmAgent)
        {
            _fsmAgent = (AntEnemy)fsmAgent;
        }

        public void Update()
        {
            var moveDir = (_fsmAgent.PositionEventBus.Position - _fsmAgent.Position).normalized;
            _fsmAgent.Log($"Ant move to {moveDir}");
            _fsmAgent.MovementController.TryToMove(moveDir);
        }
    }
}