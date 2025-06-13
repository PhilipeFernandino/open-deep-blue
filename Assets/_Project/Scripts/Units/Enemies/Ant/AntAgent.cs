using Core.ItemSystem;
using Core.Level;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.Sentis;
using UnityEngine;

namespace Core.Units
{
    public class AntAgent : Agent
    {
        [SerializeField] private AntInputHandler _inputHandler;
        [SerializeField] private Transform _startingArea;

        private Ant _ant;
        private IChemicalGridService _chemicalGrid;

        private Vector2 _previousPosition;

        public enum Action
        {
            None,
            Interact
        }

        public void Setup(Ant ant)
        {
            _ant = ant;
            _chemicalGrid = _ant.ChemicalGrid;

            _ant.TilemapCollision.Collided += () =>
            {
                AddReward(-0.001f);
            };

            _ant.TilemapCollision.CollisionStaid += () =>
            {
                AddReward(-0.001f);
            };

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
            _ant.Blackboard.CarryingItem = Item.None;
            _ant.Blackboard.MovingDirection = Vector2.zero;

            Vector2 randomPos = new(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f));
            _ant.MovementController.Teleport(_startingArea.position.XY() + randomPos);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            var blackboard = _ant.Blackboard;
            sensor.AddObservation(blackboard.MovingDirection.normalized);
            sensor.AddOneHotObservation((int)blackboard.CarryingItem, (int)Item.Last + 1);
            sensor.AddObservation(CanInteract());
        }

        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMasker)
        {
            bool canInteract = CanInteract();
            actionMasker.SetActionEnabled(branch: 0, actionIndex: (int)Action.Interact, isEnabled: canInteract);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            float moveX = actions.ContinuousActions[0];
            float moveY = actions.ContinuousActions[1];

            _ant.Blackboard.MovingDirection = new Vector2(moveX, moveY);

            int interactAction = actions.DiscreteActions[0];

            bool choseInteraction = interactAction == (int)Action.Interact;

            if (choseInteraction)
            {
                _ant.TryToInteract();
            }

            bool isCarryingLeaf = _ant.Blackboard.CarryingItem == Item.Leaf;
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
            ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

            Vector2 moveInput = _inputHandler.MoveInput;

            continuousActions[0] = moveInput.x;
            continuousActions[1] = moveInput.y;

            bool interactInput = _inputHandler.InteractTriggered;

            discreteActions[0] = interactInput ? 1 : 0;

            if (interactInput)
            {
                _inputHandler.ConsumeInteractInput();
            }
        }

        private bool CanInteract()
        {
            return _ant.CanInteract();
        }

        private void FixedUpdate()
        {
            AddReward(-1f / MaxStep);

            if (_ant.Blackboard.CarryingItem == Item.None)
            {
                _ant.ChemicalGrid.Drop(_ant.Position, Chemical.ExplorePheromone, 22.5f * Time.deltaTime);
            }
        }
    }
}