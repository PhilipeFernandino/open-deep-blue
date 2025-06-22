using Core.ItemSystem;
using Core.Level;
using Core.Map;
using Core.Train;
using Core.Util;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.Sentis;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Core.Units
{
    public class AntAgent : Agent
    {
        [SerializeField] private AntInputHandler _inputHandler;
        [SerializeField] private Transform _startingArea;
        [SerializeField] private List<Tile> _interactableTileTypes;

        private Ant _ant;
        private IChemicalGridService _chemicalGrid;
        private IColonyService _colonyManager;
        private DecisionRequester _decisionRequester;
        private AntBlackboard _blackboard;

        public bool CanEat => _ant.IsCarrying(Item.Fungus);
        public bool CanDig => _ant.IsFacing(Tile.BlueStone);
        public bool CanFeedFungus => _ant.IsCarrying(Item.Leaf) && _ant.IsFacing(Tile.Fungus);
        public bool CanFeedQueen => _ant.IsCarrying(Item.Fungus) && _ant.IsFacing(Tile.QueenAnt);
        public bool CanGatherLeaf => _ant.IsCarrying(Item.None) && _ant.IsFacing(Tile.GreenGrass);
        public bool CanGatherFungus => _ant.IsCarrying(Item.None) && _ant.IsFacing(Tile.Fungus);


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

        public void Setup(Ant ant)
        {
            _ant = ant;
            _blackboard = _ant.Blackboard;
            _chemicalGrid = _ant.ChemicalGrid;
            _colonyManager = ServiceLocatorUtilities.GetServiceAssert<IColonyService>();
            _decisionRequester = GetComponent<DecisionRequester>();

            var model = GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().Model;
            if (model != null)
            {
                Debug.Log($"{GetType()} - Model loaded: {model.name}");
            }
            else
            {
                Debug.LogWarning($"{GetType()} - No model assigned");
            }
            _colonyManager.RegisterAnt(this);
        }

        public override void OnEpisodeBegin()
        {
            _ant.Blackboard.CarryingItem = Item.None;
            _ant.Blackboard.MovingDirection = Vector2.zero;

            Vector2 randomPos = new(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f));
            _ant.MovementController.Teleport(_startingArea.position.XY() + randomPos);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(_blackboard.MovingDirection.normalized);
            sensor.AddOneHotObservation((int)_blackboard.CarryingItem, (int)Item.Last + 1);

            (int tileObservationSize, int tileOneHotIndex) = ObserveFacingTile();
            sensor.AddOneHotObservation(tileOneHotIndex, tileObservationSize);
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
            actionMasker.SetActionEnabled(branch: 0, actionIndex: (int)AntAction.None, isEnabled: true);

            actionMasker.SetActionEnabled(branch: 0, actionIndex: (int)AntAction.Eat, isEnabled: CanEat);
            actionMasker.SetActionEnabled(branch: 0, actionIndex: (int)AntAction.Dig, isEnabled: CanDig);
            actionMasker.SetActionEnabled(branch: 0, actionIndex: (int)AntAction.FeedFungus, isEnabled: CanFeedFungus);
            actionMasker.SetActionEnabled(branch: 0, actionIndex: (int)AntAction.FeedQueen, isEnabled: CanFeedQueen);
            actionMasker.SetActionEnabled(branch: 0, actionIndex: (int)AntAction.GatherLeaf, isEnabled: CanGatherLeaf);
            actionMasker.SetActionEnabled(branch: 0, actionIndex: (int)AntAction.GatherFungus, isEnabled: CanGatherFungus);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            float moveX = actions.ContinuousActions[0];
            float moveY = actions.ContinuousActions[1];

            _ant.Blackboard.MovingDirection = new Vector2(moveX, moveY);

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
            var continuousActions = actionsOut.ContinuousActions;
            continuousActions[0] = _inputHandler.MoveInput.x;
            continuousActions[1] = _inputHandler.MoveInput.y;

            var discreteActions = actionsOut.DiscreteActions;

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
            if (_ant.Blackboard.CarryingItem == Item.Leaf)
            {
                _ant.ChemicalGrid.Drop(_ant.Position, Chemical.FoodPheromone, 22.5f * Time.fixedDeltaTime);
            }

            _ant.ChemicalGrid.Drop(_ant.Position, Chemical.PresencePheromone, 10f * Time.fixedDeltaTime);
            _blackboard.CumulativeReward = GetCumulativeReward();
        }
    }
}