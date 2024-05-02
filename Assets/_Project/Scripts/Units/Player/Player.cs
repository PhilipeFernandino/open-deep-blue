using Coimbra;
using Core.FSM;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Player
{
    using PlayerFSMState = IFSMState<PlayerState>;

    public class Player : Actor, IFSMAgent<PlayerState>
    {
        [SerializeField] private PlayerMovement2D _playerMovement;
        [SerializeField] private PlayerHold _playerHold;
        [SerializeField] private PlayerAnimator _playerAnimator;

        [SerializeField] private float _movementSpeed;

        private FSM<PlayerState> _fsm;

        internal PlayerAnimator PlayerAnimator => _playerAnimator;
        internal PlayerMovement2D PlayerMovement => _playerMovement;

        Dictionary<PlayerState, PlayerFSMState> IFSMAgent<PlayerState>.States => throw new System.NotImplementedException();

        public IPlayerFSMState State => ((IPlayerFSMState)_fsm.State);

        #region Input Handling Delegation
        public void MoveInput(Vector2 direction) => State.MoveInput(direction);

        public void InteractInput() => State.InteractInput();

        public void UseEquipmentInput(Vector2 worldPosition) => State.UseEquipmentInput(worldPosition);
        #endregion

        private void Update()
        {
            _fsm.Update();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _playerMovement.Setup(_movementSpeed);

            _fsm = new FSM<PlayerState>(new()
            {
                { PlayerState.Idle, new PlayerIdleState() },
                { PlayerState.Moving, new PlayerMovingState() },
            }, this);

            TransferState(PlayerState.Idle, null, null);
        }

        public void TransferState(PlayerState nextState, IEnterStateData enterStateData, PlayerFSMState actor) => _fsm.TransferState(nextState, enterStateData, actor);
    }

    public enum PlayerState
    {
        None,
        Idle,
        Moving,
        Dashing,
    }
}