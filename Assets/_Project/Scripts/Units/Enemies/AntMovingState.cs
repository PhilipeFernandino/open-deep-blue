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
        private List<Vector2Int> _path = new(25);
        private PositionEventBus _targetPositionEventBus;

        private int _pathIndex;

        private Vector2 _currentTrackPos;

        public Vector2 Position => _fsmAgent.Position;
        public Vector2 TargetPosition => _fsmAgent.PositionEventBus.Position;

        public void Enter(IEnterStateData enterStateData)
        {
            _targetPositionEventBus.PositionChanged += TargetPositionChanged_EventHandler;
        }

        public void Exit()
        {
            _targetPositionEventBus.PositionChanged -= TargetPositionChanged_EventHandler;
        }

        public void Initialize(IFSMAgent<AntState> fsmAgent)
        {
            _fsmAgent = (AntEnemy)fsmAgent;
            _targetPositionEventBus = _fsmAgent.PositionEventBus;
        }

        public void Update()
        {
            if (_path.Count == 0)
            {

                if (_fsmAgent.PathService.TryFindPath(Position, TargetPosition, in _path, 500))
                {
                    //StringBuilder sb = new("Found path: \n");
                    //foreach (var path in _path)
                    //{
                    //    sb.AppendLine(path.ToString());
                    //}
                    //_fsmAgent.Log(sb.ToString());

                    _currentTrackPos = TargetPosition;
                    WalkPath();
                }
            }
            else
            {
                WalkPath();
            }
        }

        private void WalkPath()
        {
            var distance = Vector2.Distance(Position, TargetPosition);

            if (distance < _fsmAgent.AttackDistance)
            {
                _fsmAgent.MovementController.ResetMovement();
                return;
            }

            if (_pathIndex < _path.Count)
            {
                var moveDir = (_path[_pathIndex] - Position).normalized;
                _fsmAgent.MovementController.TryToMove(moveDir);

                var distanceToNode = Vector2.Distance(Position, _path[_pathIndex]);
                if (distanceToNode < 1)
                {
                    _pathIndex++;
                }
            }
            else
            {
                ResetPath();
            }
        }

        private void ResetPath()
        {
            _path?.Clear();
            _pathIndex = 0;
        }

        private void TargetPositionChanged_EventHandler(Vector2 vector)
        {
            if (_currentTrackPos.ManhattanDistance(vector) > 2)
            {
                ResetPath();
            }
        }
    }
}