using Core.FSM;

namespace Core.Player
{
    public class PlayerDashingState : PlayerStateBase
    {
        private DashingEnterStateData _enterStateData;

        public override void Enter(IEnterStateData enterStateData)
        {
            _enterStateData = (DashingEnterStateData)enterStateData;

            // Enter on ground anim
            _fsmAgent.PlayerMovement.Dash(
                _enterStateData.DashSpeed * _enterStateData.Direction,
                _enterStateData.DashDuration,
                () => _fsmAgent.TransferState(PlayerState.Idle, null, this));

            _fsmAgent.AddIFrames(_enterStateData.DashDuration);
        }

        public override void Update()
        {

        }

        public override void Exit()
        {
        }

    }
}