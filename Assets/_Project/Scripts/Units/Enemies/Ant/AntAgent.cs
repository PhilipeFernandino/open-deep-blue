using Core.Colony;
using Core.ItemSystem;
using Core.Level;
using Core.Map;
using Core.Util;
using System;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Core.Units
{
    [RequireComponent(typeof(Ant))]
    public class AntAgent : Agent
    {
        [SerializeField] private AntInputHandler _inputHandler;
        [SerializeField] private Transform _startingArea;
        [SerializeField] private List<Tile> _interactableTileTypes;

        private DecisionRequester _decisionRequester;
        private AntBlackboard _blackboard;
        private Ant _ant;
        private float _existentialPenalty;

        public bool CanEat => _blackboard.IsCarrying(Item.Fungus);
        public bool CanFeedFungus => _blackboard.IsCarrying(Item.Leaf) && _ant.IsFacing(Tile.Fungus);
        public bool CanFeedQueen => _blackboard.IsCarrying(Item.Fungus) && _ant.IsFacing(Tile.QueenAnt);
        public bool CanGatherLeaf => _blackboard.IsCarrying(Item.None) && _ant.IsFacing(Tile.GreenGrass);
        public bool CanGatherFungus => _blackboard.IsCarrying(Item.None) && _ant.IsFacing(Tile.Fungus);

        public bool CanDig => _ant.IsFacing(Tile.BlueStone);

        public event Func<Vector2> SpawnPointRequested;

        public enum AntAction
        {
            None,
            Eat,
            Dig,
            FeedQueen,
            FeedFungus,
            GatherLeaf,
            GatherFungus,
        }

        public enum MoveDirection
        {
            None,
            Up,
            Left,
            Down,
            Right
        }

        protected override void Awake()
        {
            base.Awake();

            _blackboard = GetComponent<AntBlackboard>();
            _decisionRequester = GetComponent<DecisionRequester>();
            _ant = GetComponent<Ant>();

            var model = GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().Model;
            if (model != null)
            {
                Debug.Log($"{GetType()} - Model loaded: {model.name}");
            }
            else
            {
                Debug.LogWarning($"{GetType()} - No model assigned");
            }
        }


        public override void OnEpisodeBegin()
        {
            _ant.ResetState();

            _blackboard.CarryingItem = Item.None;
            _blackboard.MovingDirection = Vector2.zero;

            _ant.MovementController.Teleport(SpawnPointRequested.Invoke());
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            _existentialPenalty = ServiceLocatorUtilities.GetServiceAssert<ICurriculumService>().GetCurrentConfig().AgentExistentialPenalty;

            if (!_ant.IsStarted)
            {
                sensor.AddObservation(new float[17]);
                return;
            }

            sensor.AddObservation(_blackboard.MovingDirection.normalized);
            sensor.AddOneHotObservation((int)_blackboard.CarryingItem, (int)Item.Fungus + 1);

            (int tileObservationSize, int tileOneHotIndex) = ObserveFacingTile();

            sensor.AddOneHotObservation(tileOneHotIndex, tileObservationSize);

            sensor.AddObservation(Mathf.Clamp01(_blackboard.SacietyPercentage));
            sensor.AddObservation(Mathf.Clamp01(_blackboard.EnergyPercentage));

            sensor.AddObservation(CanEat ? 1.0f : 0.0f);
            sensor.AddObservation(CanDig ? 1.0f : 0.0f);
            sensor.AddObservation(CanFeedFungus ? 1.0f : 0.0f);
            sensor.AddObservation(CanFeedQueen ? 1.0f : 0.0f);
            sensor.AddObservation(CanGatherLeaf ? 1.0f : 0.0f);
            sensor.AddObservation(CanGatherFungus ? 1.0f : 0.0f);

        }

        private (int tileObservationSize, int tileOneHotIndex) ObserveFacingTile()
        {
            int tileObservationSize, tileOneHotIndex;

            Tile tileInFront = _ant.IsFacing();

            int interactableIndex = _interactableTileTypes.IndexOf(tileInFront);

            tileObservationSize = _interactableTileTypes.Count + 1;
            if (interactableIndex != -1)
            {
                tileOneHotIndex = interactableIndex;
            }
            else
            {
                tileOneHotIndex = _interactableTileTypes.Count;
            }

            return (tileObservationSize, tileOneHotIndex);
        }

        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMasker)
        {
            if (!_ant.IsStarted)
                return;

            actionMasker.SetActionEnabled(0, (int)AntAction.None, true);

            actionMasker.SetActionEnabled(0, (int)AntAction.Eat, CanEat);
            actionMasker.SetActionEnabled(0, (int)AntAction.Dig, CanDig);
            actionMasker.SetActionEnabled(0, (int)AntAction.FeedFungus, CanFeedFungus);
            actionMasker.SetActionEnabled(0, (int)AntAction.FeedQueen, CanFeedQueen);
            actionMasker.SetActionEnabled(0, (int)AntAction.GatherLeaf, CanGatherLeaf);
            actionMasker.SetActionEnabled(0, (int)AntAction.GatherFungus, CanGatherFungus);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            if (!_ant.IsStarted)
                return;

            var moveAction = (MoveDirection)actions.DiscreteActions[1];

            Vector2 targetDirection = Vector2.zero;
            switch (moveAction)
            {
                case MoveDirection.Up:
                    targetDirection = Vector2.up;
                    break;
                case MoveDirection.Right:
                    targetDirection = Vector2.right;
                    break;
                case MoveDirection.Down:
                    targetDirection = Vector2.down;
                    break;
                case MoveDirection.Left:
                    targetDirection = Vector2.left;
                    break;
            }

            _blackboard.MovingDirection = targetDirection;

            var chosenAction = (AntAction)actions.DiscreteActions[0];

            switch (chosenAction)
            {
                case AntAction.None:
                    break;
                case AntAction.Eat:
                    _ant.TryEat();
                    break;
                case AntAction.Dig:
                    _ant.TryDig(_decisionRequester.DecisionPeriod);
                    break;
                case AntAction.FeedQueen:
                case AntAction.FeedFungus:
                case AntAction.GatherLeaf:
                case AntAction.GatherFungus:
                    _ant.TryToInteract();
                    break;

            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            if (!_ant.IsStarted)
                return;

            var discreteActions = actionsOut.DiscreteActions;

            Vector2 moveInput = _inputHandler.MoveInput;
            MoveDirection moveAction = MoveDirection.None;

            if (moveInput.y > 0.5f)
            {
                moveAction = MoveDirection.Up;
            }
            else if (moveInput.y < -0.5f)
            {
                moveAction = MoveDirection.Down;
            }
            else if (moveInput.x > 0.5f)
            {
                moveAction = MoveDirection.Right;
            }
            else if (moveInput.x < -0.5f)
            {
                moveAction = MoveDirection.Left;
            }

            discreteActions[1] = (int)moveAction;

            AntAction chosenAction = AntAction.None;

            if (_inputHandler.EatTriggered)
            {
                chosenAction = AntAction.Eat;
                _inputHandler.ConsumeEatInput();
            }
            else if (_inputHandler.InteractTriggered)
            {
                if (CanDig)
                {
                    chosenAction = AntAction.Dig;
                }
                else if (CanFeedQueen)
                {
                    chosenAction = AntAction.FeedQueen;
                }
                else if (CanFeedFungus)
                {
                    chosenAction = AntAction.FeedFungus;
                }
                else if (CanGatherLeaf)
                {
                    chosenAction = AntAction.GatherLeaf;
                }
                else if (CanGatherFungus)
                {
                    chosenAction = AntAction.GatherFungus;
                }

                _inputHandler.ConsumeInteractInput();
            }

            discreteActions[0] = (int)chosenAction;
        }

        private void FixedUpdate()
        {
            if (!_ant.IsStarted)
                return;
            _blackboard.CumulativeReward = GetCumulativeReward();
            AddReward(_existentialPenalty);
        }
    }
}