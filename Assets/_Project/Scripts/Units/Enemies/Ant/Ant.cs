using Coimbra;
using Core.Debugger;
using Core.EventBus;
using Core.FSM;
using Core.HealthSystem;
using Core.Interaction;
using Core.ItemSystem;
using Core.Level;
using Core.Map;
using Core.Player;
using Core.Train;
using Core.Util;
using System;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

namespace Core.Units
{
    [RequireComponent(typeof(Agent))]
    [RequireComponent(typeof(Movement2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(AntTilemapCollision))]
    public class Ant : Actor, IFSMAgent<AntState>
    {
        [SerializeField] private HealthComponent _healthComponent;

        [Header("Debugging")]
        [SerializeField] private DebugChannelSO _debugChannel;
        [SerializeField] private bool _debug;

        private AntAgent _agent;
        private FSM<AntState> _fsm;
        private Movement2D _movementController;
        private BoxCollider2D _boxCollider;

        private AntTilemapCollision _tilemapCollision;
        private ColonyEconomySettings _economySettings;

        internal AntBlackboard Blackboard { get; private set; }

        internal AntAgent Agent => _agent;
        internal AntTilemapCollision TilemapCollision => _tilemapCollision;
        internal Movement2D MovementController => _movementController;
        internal BoxCollider2D BoxCollider => _boxCollider;

        internal float Health => _healthComponent.Health;
        internal float MaxHealth => _healthComponent.MaxHealth;
        internal float AttackDistance => Blackboard.AttackDistance;
        internal float AttackDamage => Blackboard.AttackDamage;
        internal float DigDamage => Blackboard.DigDamage;
        internal float AggroDistance => Blackboard.AggroDistance;
        internal float MovementSpeed => Blackboard.MovementSpeed;

        internal IPathService PathService { get; private set; }
        internal IGridService GridService { get; private set; }
        internal IChemicalGridService ChemicalGrid { get; private set; }
        internal IInteractionService InteractionService { get; private set; }

        public Item Carrying => Blackboard.CarryingItem;

        public Vector2 Position => MovementController.Position;

        public Vector2 InteractPosition => Position + _movementController.FacingDirection * 1f;

        Dictionary<AntState, IFSMState<AntState>> IFSMAgent<AntState>.States => _fsm.States;


        public void GiveReward(float value) => _agent.AddReward(value);
        public bool IsCarrying(Item item) => Carrying == item;
        public bool IsFacing(Tile tile) => IsFacing() == tile;
        public Tile IsFacing() => GridService.Get(InteractPosition).TileType;
        public bool CanInteract() => InteractionService.CanInteract(InteractPosition);
        public void TryToInteract() => InteractionService.Interact(InteractPosition, this);
        public void GiveItem(Item item) => Blackboard.CarryingItem = item;

        public void TryEat()
        {
            if (IsCarrying(Item.Fungus))
            {
                Blackboard.Saciety += _economySettings.FungusFeedAntsAmount;
                GiveItem(Item.None);
                new AntEvent(AntEventType.Eat, this).Invoke(this);
            }
        }

        public void TransferState(AntState nextState, IEnterStateData enterStateData, IFSMState<AntState> actor)
        => _fsm.TransferState(nextState, enterStateData, actor);

        public void Log(string message) => Debug.Log(message);

        public void TryDig(float scale)
        {
            if (!Blackboard.HasEnergy(Blackboard.DigEnergyCost))
                return;

            GridService.DamageTileAt(InteractPosition, scale * DigDamage);
            Blackboard.Energy -= Blackboard.DigEnergyCost;
            new AntEvent(AntEventType.Dig, this).Invoke(this);
        }

        private void Update()
        {
            if (!IsStarted)
            {
                return;
            }

            _fsm.Update();
            RaiseDebug();
        }

        [System.Diagnostics.Conditional(conditionString: "DEBUG"), System.Diagnostics.Conditional(conditionString: "UNITY_EDITOR")]
        private void RaiseDebug()
        {
            if (_debug)
            {
                _debugChannel.RaiseEvent("ant",
                    new AntDebugData()
                    {
                        CarryingItem = Carrying,
                        FacingTile = IsFacing(),
                        Health = Health,
                        MaxHealth = MaxHealth,
                        MaxSaciety = Blackboard.MaxSaciety,
                        Energy = Blackboard.Energy,
                        MaxEnergy = Blackboard.MaxEnergy,
                        Position = Position,
                        Saciety = Blackboard.Saciety,
                        CumulativeReward = Blackboard.CumulativeReward,
                        CanEat = _agent.CanEat,
                        CanGatherFungus = _agent.CanGatherFungus,
                        CanGatherLeaf = _agent.CanGatherLeaf,
                        CanFeedQueen = _agent.CanFeedQueen,
                        CanFeedFungus = _agent.CanFeedFungus,
                        CanDig = _agent.CanDig
                    });
            }
        }

        private void FixedUpdate()
        {
            if (!IsStarted)
                return;

            Blackboard.Saciety -= Blackboard.SacietyLoss * Time.fixedDeltaTime;

            float energyDifference = -Blackboard.EnergyLoss;

            if (IsCarrying(Item.Leaf))
            {
                ChemicalGrid.Drop(Position, Chemical.FoodPheromone, Blackboard.DropFoodPheromone * Time.fixedDeltaTime);
                energyDifference -= Blackboard.CarryLeafCost;
            }

            if (Blackboard.SacietyPercentage > Blackboard.EnergyRegenerationThreshold)
            {
                energyDifference += Blackboard.EnergyRegenerationRate;
            }

            Blackboard.Energy += energyDifference * Time.fixedDeltaTime;

            ChemicalGrid.Drop(Position, Chemical.PresencePheromone, Blackboard.DropPresencePheromone * Time.fixedDeltaTime);

            _fsm.FixedUpdate();
        }

        public void ResetState()
        {
            Blackboard.ResetState();
        }

        protected override void OnInitialize()
        {
            _movementController = GetComponent<Movement2D>();
            _boxCollider = GetComponent<BoxCollider2D>();
            _healthComponent.Attacked += Attacked_EventHandler;
            _tilemapCollision = GetComponent<AntTilemapCollision>();
            _agent = GetComponent<AntAgent>();

            Blackboard = GetComponent<AntBlackboard>();
            Blackboard.SacietyZeroed += SacietyZeroedEventHandler;

            OnStarting += AntOnStarting;
        }

        private void SacietyZeroedEventHandler()
        {
            new AntEvent(AntEventType.Death, this).Invoke(this);
        }

        private void AntOnStarting(Actor sender)
        {
            _movementController.Setup(MovementSpeed);

            _fsm = new(new()
            {
                { AntState.Idle, new AntIdleState() },
                { AntState.Moving, new AntMovingState() },
            }, this);

            GridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            PathService = ServiceLocatorUtilities.GetServiceAssert<IPathService>();
            ChemicalGrid = ServiceLocatorUtilities.GetServiceAssert<IChemicalGridService>();
            InteractionService = ServiceLocatorUtilities.GetServiceAssert<IInteractionService>();
            _economySettings = ScriptableSettings.GetOrFind<ColonyEconomySettings>();

            TransferState(AntState.Moving, null, null);
        }

        private void Attacked_EventHandler(AttackedData data)
        {
            var knockbackForce = (Position - data.Attack.SourcePosition).normalized * data.Attack.Knockback;
            _movementController.AddKnockback(knockbackForce);
        }
    }

    public enum AntState
    {
        None,
        Idle,
        Moving,
    }
}