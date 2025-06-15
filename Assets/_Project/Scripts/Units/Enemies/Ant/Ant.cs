using Coimbra;
using Core.EventBus;
using Core.FSM;
using Core.HealthSystem;
using Core.Interaction;
using Core.ItemSystem;
using Core.Level;
using Core.Map;
using Core.Player;
using Core.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Units
{
    [RequireComponent(typeof(Movement2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(AntTilemapCollision))]
    public class Ant : Actor, IFSMAgent<AntState>
    {
        [SerializeField] private AntAgent _agent;
        [SerializeField] private HealthComponent _healthComponent;
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _aggroDistance;
        [SerializeField] private float _attackDistance;
        [SerializeField] private float _attackDamage;

        private FSM<AntState> _fsm;
        private Movement2D _movementController;
        private BoxCollider2D _boxCollider;

        private AntTilemapCollision _tilemapCollision;

        internal AntBlackboard Blackboard { get; private set; }

        internal AntAgent Agent => _agent;
        internal AntTilemapCollision TilemapCollision => _tilemapCollision;
        internal Movement2D MovementController => _movementController;
        internal BoxCollider2D BoxCollider => _boxCollider;
        internal float AttackDistance => _attackDistance;
        internal float AttackDamage => _attackDamage;
        internal float AggroDistance => _aggroDistance;

        internal IPathService PathService { get; private set; }
        internal IGridService TileGrid { get; private set; }
        internal IChemicalGridService ChemicalGrid { get; private set; }
        internal IInteractionService InteractionService { get; private set; }

        public Item IsCarrying => Blackboard.CarryingItem;

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

        public bool CanInteract()
        {
            Vector2 interactPosition = Position + _movementController.LastMovementInput * 0.5f;
            return InteractionService.CanInteract(interactPosition);
        }

        public void TryToInteract()
        {
            Vector2 interactPosition = Position + _movementController.LastMovementInput.normalized;
            InteractionService.Interact(interactPosition, this);
        }

        public void Give(Item item)
        {
            Blackboard.CarryingItem = item;
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
            Blackboard = new()
            {
                CarryingItem = Item.None,
                AggroDistance = _aggroDistance,
                MovementSpeed = _movementSpeed,
                MovingDirection = Vector3.zero
            };

            _movementController = GetComponent<Movement2D>();
            _boxCollider = GetComponent<BoxCollider2D>();
            _healthComponent.Attacked += Attacked_EventHandler;
            _tilemapCollision = GetComponent<AntTilemapCollision>();
        }

        private void Attacked_EventHandler(AttackedData data)
        {
            var knockbackForce = (Position - data.Attack.SourcePosition).normalized * data.Attack.Knockback;
            Debug.Log($"Attacked data: {data}, {knockbackForce}, {Position}");
            _movementController.AddKnockback(knockbackForce);
        }

        protected override void OnSpawn()
        {
            _movementController.Setup(_movementSpeed);

            _fsm = new(new()
            {
                { AntState.Idle, new AntIdleState() },
                { AntState.Moving, new AntMovingState() },
            }, this);

            TileGrid = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            PathService = ServiceLocatorUtilities.GetServiceAssert<IPathService>();
            ChemicalGrid = ServiceLocatorUtilities.GetServiceAssert<IChemicalGridService>();
            InteractionService = ServiceLocatorUtilities.GetServiceAssert<IInteractionService>();

            _agent.Setup(this);

            TransferState(AntState.Moving, null, null);
        }
    }

    public enum AntState
    {
        None,
        Idle,
        Moving,
    }
}