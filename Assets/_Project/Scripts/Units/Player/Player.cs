using Coimbra;
using Core.FSM;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Player
{
    using PlayerFSMState = IFSMState<PlayerState>;

    [RequireComponent(typeof(PlayerHold))]
    [RequireComponent(typeof(PlayerAnimator))]
    [RequireComponent(typeof(PlayerMovement2D))]

    public class Player : Actor, IFSMAgent<PlayerState>
    {
        [SerializeField] private float _movementSpeed;

        private PlayerHold _playerHold;
        private PlayerAnimator _playerAnimator;
        private PlayerMovement2D _playerMovement;
        private PlayerStateResolver _playerStateResolver;


        private FSM<PlayerState> _fsm;

        internal PlayerAnimator PlayerAnimator => _playerAnimator;
        internal PlayerMovement2D PlayerMovement => _playerMovement;
        internal PlayerHold PlayerHold => _playerHold;
        internal PlayerStateResolver StateResolver => _playerStateResolver;

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
            base.OnSpawn();
            _playerAnimator = GetComponent<PlayerAnimator>();
            _playerHold = GetComponent<PlayerHold>();
            _playerMovement = GetComponent<PlayerMovement2D>();

            _playerStateResolver = new();
        }

        protected override void OnSpawn()
        {
            base.OnInitialize();
            _playerMovement.Setup(_movementSpeed);
            _playerStateResolver.Setup(this);

            _fsm = new FSM<PlayerState>(new()
            {
                { PlayerState.Idle, new PlayerIdleState() },
                { PlayerState.Moving, new PlayerMovingState() },
                { PlayerState.UsingEquipment, new PlayerUsingEquipmentState() }
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
        UsingEquipment
    }
}