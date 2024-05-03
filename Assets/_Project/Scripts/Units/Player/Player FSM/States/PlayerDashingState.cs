using Core.FSM;

namespace Core.Player
{
    public class PlayerDashingState : PlayerStateBase
    {
        private DashingEnterStateData _enterStateData;

        public override void Enter(IEnterStateData enterStateData)
        {
            _enterStateData = (DashingEnterStateData)enterStateData;
        }

        public override void Update()
        {

        }

        public override void Exit()
        {
        }

    }
}