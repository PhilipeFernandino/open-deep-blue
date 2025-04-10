using Core.EventBus;
using Core.FSM;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Core.Units
{
    [System.Serializable]
    public class AntMovingState : IFSMState<AntState>
    {
        private AntEnemy _fsmAgent;
        private LinkedList<Vector2Int> _path;

        public Vector2 Position => _fsmAgent.Position;
        public Vector2 TargetPosition => _fsmAgent.PositionEventBus.Position;

        public void Enter(IEnterStateData enterStateData)
        {
        }

        public void Exit()
        {
        }

        public void Initialize(IFSMAgent<AntState> fsmAgent)
        {
            _fsmAgent = (AntEnemy)fsmAgent;
        }

        public void Update()
        {
            if (_path == null)
            {
                if (_fsmAgent.PathService.TryFindPath(Position, TargetPosition, out var foundPath))
                {
                    _path = new LinkedList<Vector2Int>(foundPath);

                    StringBuilder sb = new("Found path: \n");
                    foreach (var path in _path)
                    {
                        sb.AppendLine(path.ToString());
                    }
                    _fsmAgent.Log(sb.ToString());
                }
            }
            else
            {
                var distance = Vector2.Distance(Position, TargetPosition);

                if (distance < _fsmAgent.AttackDistance)
                {
                    _fsmAgent.MovementController.ResetMovement();
                    return;
                }

                if (_path.Count > 0)
                {
                    var moveDir = (_path.First.Value - Position).normalized;
                    _fsmAgent.Log($"Ant move to {moveDir}");
                    _fsmAgent.MovementController.TryToMove(moveDir);

                    var distanceToNode = Vector2.Distance(Position, _path.First.Value);
                    if (distanceToNode < 1)
                    {
                        _path.RemoveFirst();
                    }
                }
                else
                {
                    _path = null;
                }
            }
        }
    }
}