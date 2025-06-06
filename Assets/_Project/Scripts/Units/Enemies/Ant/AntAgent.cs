using Core.ItemSystem;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Core.Units
{
    public class AntAgent : Agent
    {
        private Ant _ant;
        private Transform _startingArea;

        public enum Action
        {
            None,
            Interact
        }

        public void Setup(Ant ant)
        {
            _ant = ant;
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

            actionMasker.SetActionEnabled(branch: 0, actionIndex: (int)Action.Interact, isEnabled: canInteract);
        }


        public override void OnActionReceived(ActionBuffers actions)
        {
            float moveX = actions.ContinuousActions[0];
            float moveY = actions.ContinuousActions[1];
            _ant.Blackboard.MovingDirection = new Vector2(moveX, moveY).normalized;

            int interactAction = actions.DiscreteActions[0];

            if (interactAction == (int)Action.Interact)
            {
                _ant.TryToInteract();
            }
        }

        private bool CanInteract()
        {
            return _ant.CanInteract();
        }
    }
}