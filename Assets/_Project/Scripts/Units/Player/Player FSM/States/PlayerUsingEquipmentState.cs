using Core.FSM;
using Cysharp.Threading.Tasks;
using System;

namespace Core.Player
{
    public class PlayerUsingEquipmentState : PlayerStateBase
    {
        private UsingEquipmentEnterStateData _enterStateData;

        public override void Enter(IEnterStateData enterStateData)
        {
            _enterStateData = (UsingEquipmentEnterStateData)enterStateData;
            ExitStateTask().Forget();
        }
        private async UniTask ExitStateTask()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_enterStateData.UseEffect.LockDuration));
            _fsmAgent.TransferState(PlayerState.Idle, null, this);
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
        }
    }
}