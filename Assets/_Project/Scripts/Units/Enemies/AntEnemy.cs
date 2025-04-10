using Coimbra;
using Core.EventBus;
using Core.FSM;
using Core.HealthSystem;
using Core.Level;
using Core.Player;
using Core.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Units
{
    [RequireComponent(typeof(Movement2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class AntEnemy : Actor, IFSMAgent<AntState>
    {
        [SerializeField] private HealthComponent _healthComponent;
        [SerializeField] private PositionEventBus _targetPositionEventBus;
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _attackDistance;
        [SerializeField] private float _attackDamage;

        private FSM<AntState> _fsm;
        private Movement2D _movementController;
        private BoxCollider2D _boxCollider;

        internal Movement2D MovementController => _movementController;
        internal BoxCollider2D BoxCollider => _boxCollider;
        internal PositionEventBus PositionEventBus => _targetPositionEventBus;
        internal float AttackDistance => _attackDistance;
        internal float AttackDamage => _attackDamage;

        internal IPathService PathService { get; private set; }
        internal IGridService GridService { get; private set; }

        public Vector2 Position => transform.position.XY();

        Dictionary<AntState, IFSMState<AntState>> IFSMAgent<AntState>.States => _fsm.States;

        public void TransferState(AntState nextState, IEnterStateData enterStateData, IFSMState<AntState> actor)
        {
            _fsm.TransferState(nextState, enterStateData, actor);
        }

        public void Log(string message)
        {
            Debug.Log(message);
        }

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
            _movementController = GetComponent<Movement2D>();
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        protected override void OnSpawn()
        {
            base.OnInitialize();
            _movementController.Setup(_movementSpeed);

            _fsm = new(new()
            {
                { AntState.Idle, new AntIdleState() },
                { AntState.Moving, new AntMovingState() },
            }, this);

            GridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            PathService = ServiceLocatorUtilities.GetServiceAssert<IPathService>();

            TransferState(AntState.Idle, null, null);
        }
    }

    public enum AntState
    {
        None,
        Idle,
        Moving,
    }
}