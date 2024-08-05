using Coimbra;
using Core.FSM;
using Core.HealthSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Player
{
    using PlayerFSMState = IFSMState<PlayerState>;

    [RequireComponent(typeof(PlayerHold))]
    [RequireComponent(typeof(PlayerAnimator))]
    [RequireComponent(typeof(PlayerMovement2D))]
    [RequireComponent(typeof(BoxCollider2D))]

    public class Player : Actor, IFSMAgent<PlayerState>
    {
        [SerializeField] private HealthComponent _healthComponent;

        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _dashSpeed;
        [SerializeField] private float _dashDuration;

        private PlayerHold _playerHold;
        private PlayerAnimator _playerAnimator;
        private PlayerMovement2D _playerMovement;
        private BoxCollider2D _boxCollider;

        private PlayerStateResolver _playerStateResolver;

        private FSM<PlayerState> _fsm;

        internal PlayerAnimator PlayerAnimator => _playerAnimator;
        internal PlayerMovement2D PlayerMovement => _playerMovement;
        internal PlayerHold PlayerHold => _playerHold;
        internal PlayerStateResolver StateResolver => _playerStateResolver;
        internal BoxCollider2D BoxCollider => _boxCollider;

        internal float DashSpeed => _dashSpeed;
        internal float DashDuration => _dashDuration;

        #region FSM Delegation
        Dictionary<PlayerState, PlayerFSMState> IFSMAgent<PlayerState>.States => _fsm.States;

        public Vector2 PlayerMovementDirection => PlayerMovement.LastMovementInput;

        public IPlayerFSMState State => ((IPlayerFSMState)_fsm.State);
        #endregion

        #region Health Delegation
        public float Health => _healthComponent.Health;

        public void Hurt(Attack attackData)
        {
            _healthComponent.TryHurt(attackData);
        }
        public void AddIFrames(float duration)
        {
            _healthComponent.AddIFrames(duration);
        }
        #endregion

        #region Input Handling Delegation
        public void MoveInput(Vector2 direction) => State.MoveInput(direction);

        public void InteractInput() => State.InteractInput();

        public void DashInput() => State.DashInput();

        public void UseEquipmentInput(Vector2 worldPosition) => State.UseEquipmentInput(worldPosition);

        #endregion

        private void Update()
        {
            _fsm.Update();
        }

        private void FixedUpdate()
        {
            _fsm.FixedUpdate();
        }

        protected override void OnInitialize()
        {
            base.OnSpawn();
            _playerAnimator = GetComponent<PlayerAnimator>();
            _playerHold = GetComponent<PlayerHold>();
            _playerMovement = GetComponent<PlayerMovement2D>();
            _boxCollider = GetComponent<BoxCollider2D>();

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
                { PlayerState.UsingEquipment, new PlayerUsingEquipmentState() },
                { PlayerState.Dashing, new PlayerDashingState() },
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