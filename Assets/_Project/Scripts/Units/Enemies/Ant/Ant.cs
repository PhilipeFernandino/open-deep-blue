using Coimbra;
using Core.EventBus;
using Core.FSM;
using Core.HealthSystem;
using Core.Level;
using Core.Player;
using Core.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Units
{
    [RequireComponent(typeof(Movement2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Ant : Actor, IFSMAgent<AntState>
    {
        [SerializeField] private HealthComponent _healthComponent;
        [SerializeField] private PositionEventBus _targetPositionEventBus;
        [SerializeField] private FloatRange _movementSpeedRange;
        [SerializeField] private FloatRange _aggroDistanceRange;
        [SerializeField] private float _attackDistance;
        [SerializeField] private float _attackDamage;

        private float _movementSpeed;
        private float _aggroDistance;

        private FSM<AntState> _fsm;
        private Movement2D _movementController;
        private BoxCollider2D _boxCollider;

        private AntVisionSensor _visionSensor;
        private AntBodySensor _bodySensor;

        internal AntBlackboard Blackboard { get; private set; }

        internal Movement2D MovementController => _movementController;
        internal BoxCollider2D BoxCollider => _boxCollider;
        internal PositionEventBus PositionEventBus => _targetPositionEventBus;
        internal float AttackDistance => _attackDistance;
        internal float AttackDamage => _attackDamage;
        internal float AggroDistance => _aggroDistance;

        internal IPathService PathService { get; private set; }
        internal IGridService GridService { get; private set; }
        internal IPheromoneService PheromoneGrid { get; private set; }

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

            _bodySensor.Sense();
            _visionSensor.Sense();
        }

        private void FixedUpdate()
        {
            _fsm.FixedUpdate();
        }

        protected override void OnInitialize()
        {
            base.OnSpawn();
            Blackboard = new();

            _movementController = GetComponent<Movement2D>();
            _boxCollider = GetComponent<BoxCollider2D>();
            _healthComponent.Attacked += Attacked_EventHandler;

            _bodySensor = new(this);
            _visionSensor = new(this);
        }

        private void Attacked_EventHandler(AttackedData data)
        {
            var knockbackForce = (Position - data.Attack.SourcePosition).normalized * data.Attack.Knockback;
            Debug.Log($"Attacked data: {data}, {knockbackForce}, {Position}");
            _movementController.AddKnockback(knockbackForce);
        }

        protected override void OnSpawn()
        {
            base.OnInitialize();

            _movementSpeed = _movementSpeedRange.RandomValue;
            _aggroDistance = _aggroDistanceRange.RandomValue;

            _movementController.Setup(_movementSpeed);

            _fsm = new(new()
            {
                { AntState.Idle, new AntIdleState() },
                { AntState.Moving, new AntMovingState() },
            }, this);

            GridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            PathService = ServiceLocatorUtilities.GetServiceAssert<IPathService>();
            PheromoneGrid = ServiceLocatorUtilities.GetServiceAssert<IPheromoneService>();

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