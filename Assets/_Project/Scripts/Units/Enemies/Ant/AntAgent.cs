using Core.ItemSystem;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Core.Units
{
    public class AntAgent : Agent
    {
        [SerializeField] private AntInputHandler _inputHandler;
        [SerializeField] private Transform _startingArea;

        private Ant _ant;

        public enum Action
        {
            None,
            Interact
        }

        public void Setup(Ant ant)
        {
            _ant = ant;
            _ant.Blackboard.CarryingItemChanged += (item =>
            {
                if (item == Item.Leaf)
                {
                    AddReward(1.0f);
                    EndEpisode();
                }
            });
        }

        public override void OnEpisodeBegin()
        {
            _ant.Blackboard.CarryingItem = Item.None;
            _ant.Blackboard.MovingDirection = Vector2.zero;

            Vector2 randomPos = new(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            _ant.MovementController.Teleport(_startingArea.position.XY() + randomPos);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            var blackboard = _ant.Blackboard;
            sensor.AddObservation(blackboard.MovingDirection.normalized);
            sensor.AddOneHotObservation((int)blackboard.CarryingItem, (int)Item.Last + 1);
        }

        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMasker)
        {
            bool canInteract = CanInteract();
            Debug.Log($"{GetType()} canInteract - {canInteract}");
            actionMasker.SetActionEnabled(branch: 0, actionIndex: (int)Action.Interact, isEnabled: canInteract);
        }


        public override void OnActionReceived(ActionBuffers actions)
        {
            float moveX = actions.ContinuousActions[0];
            float moveY = actions.ContinuousActions[1];

            _ant.Blackboard.MovingDirection = new Vector2(moveX, moveY).normalized;

            int interactAction = actions.DiscreteActions[0];

            Debug.Log($"{GetType()} - interact {interactAction}");
            if (interactAction == (int)Action.Interact)
            {
                _ant.TryToInteract();
            }
        }
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
            ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

            Vector2 moveInput = _inputHandler.MoveInput;

            continuousActions[0] = moveInput.x;
            continuousActions[1] = moveInput.y;

            // 1. Read the latched input
            bool interactInput = _inputHandler.InteractTriggered;

            // 2. Set the action based on the input
            discreteActions[0] = interactInput ? 1 : 0;

            // 3. If the input was true, immediately consume it so it's not used again next time.
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
        }
    }
}