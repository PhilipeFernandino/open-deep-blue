using Cinemachine;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using PrimeTween;
using Spine;
using System;
using UnityEngine;

namespace Core.Units.Bosses.Ant
{
    using Player = Player.Player;

    public class AntPawsController : MonoBehaviour
    {
        [SerializeField] private BoundsInt _arenaBounds;

        [SerializeField] private AntPaw _paw1;
        [SerializeField] private AntPaw _paw2;

        [SerializeField] private TweenSettings<Vector3> _raisePawScaleTws;
        [SerializeField] private TweenSettings<Vector3> _lowerPawScaleTws;

        [SerializeField] private ShakeSettings _shakeCameraSettings;

        [SerializeField] private float _distanceToDurationRatio = 0.5f;

        private Player _player;

        private bool _isIdle;
        private AntPaw _currentPaw;

        private void Awake()
        {
            _player = FindObjectOfType<Player>();
            _currentPaw = _paw1;
        }

        private void Start()
        {
            JumpingPawsAttackFlow().Forget();
        }

        private async UniTask JumpingPawsAttackFlow()
        {
            await JumpingPawAttack(_currentPaw);
            SwitchCurrentPaw();
            await JumpingPawsAttackFlow();
        }

        private void SwitchCurrentPaw()
        {
            _currentPaw = _currentPaw == _paw1 ? _paw2 : _paw1;
        }

        private async UniTask JumpingPawAttack(AntPaw paw)
        {
            await RaisePaw(paw);
            await MoveToPosition(paw, _player.transform.position.XY(), _distanceToDurationRatio);
            await LowerPaw(paw);

        }

        private Vector2 RandomInsideSquare(BoundsInt boundsInt)
        {
            int x = UnityEngine.Random.Range(boundsInt.xMin, boundsInt.xMax);
            int y = UnityEngine.Random.Range(boundsInt.yMin, boundsInt.yMax);
            return new Vector2(x, y);
        }

        private async UniTask DraggedPawsAttackFlow()
        {
            await RaisePaw(_currentPaw);
            await MoveToPosition(_currentPaw, _player.transform.position.XY(), _distanceToDurationRatio);
            await LowerPaw(_currentPaw);
            await MoveToPosition(_currentPaw, RandomInsideSquare(_arenaBounds), _distanceToDurationRatio / 2);
            SwitchCurrentPaw();
            DraggedPawsAttackFlow();
        }

        // TODO
        // -- Enter State
        // -- Exit State (FSM)
        // Maybe have the raise/low concurrently with the movement
        private async UniTask MoveToPosition(AntPaw paw, Vector3 position, float distanceToDurationRatio)
        {
            float distance = Vector3.Distance(_player.transform.position, position);
            float duration = distance * distanceToDurationRatio;
            Debug.Log($"{GetType()} - {duration}");


            await Tween.Position(paw.transform, position, new TweenSettings
            {
                duration = duration
            });
        }

        private async UniTask RaisePaw(AntPaw paw)
        {
            _raisePawScaleTws.startValue = paw.transform.lossyScale;
            await Tween.Scale(paw.transform, _raisePawScaleTws);
            paw.SetIsRaised(true);
        }

        private async UniTask LowerPaw(AntPaw paw)
        {
            _lowerPawScaleTws.startValue = paw.transform.lossyScale;
            await Tween.Scale(paw.transform, _lowerPawScaleTws);
            paw.SetIsRaised(false);

            var target = FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();

            _ = Tween.ShakeCustom(
                target,
                Vector3.zero,
                _shakeCameraSettings, (target, val) => target.m_TrackedObjectOffset = val)
                .OnComplete(() => target.m_TrackedObjectOffset = Vector3.zero);
        }

        #region debug
        [Button]
        private void _RaiseHand()
        {
            RaisePaw(_paw1);
        }

        [SerializeField] private Vector3 d_movePos;
        [Button]
        private void _MoveToPosition()
        {
            MoveToPosition(_paw1, d_movePos, 1f);
        }

        [Button]
        private void _LowerHand()
        {
            LowerPaw(_paw1);
        }
        #endregion
    }
}
