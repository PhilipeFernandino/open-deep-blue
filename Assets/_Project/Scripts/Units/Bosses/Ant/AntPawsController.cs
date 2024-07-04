using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using PrimeTween;
using System;
using UnityEngine;

namespace Core.Units.Bosses.Ant
{
    using Player = Player.Player;

    public class AntPawsController : MonoBehaviour
    {
        [SerializeField] private AntPaw _paw1;
        [SerializeField] private AntPaw _paw2;

        [SerializeField] private TweenSettings<Vector3> _raisePawScaleTws;
        [SerializeField] private TweenSettings<Vector3> _lowerPawScaleTws;

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
            CurrentHandToPlayer().Forget();
        }

        private async UniTask CurrentHandToPlayer()
        {
            await RaiseHand(_currentPaw);
            await MoveToPlayerPosition(_currentPaw, _player.transform.position.XY(), _distanceToDurationRatio);
            await LowerHand(_currentPaw);

            _currentPaw = _currentPaw == _paw1 ? _paw2 : _paw1;
            CurrentHandToPlayer().Forget();
        }

        private async UniTask MoveToPlayerPosition(AntPaw paw, Vector3 position, float distanceToDurationRatio)
        {
            float distance = Vector3.Distance(_player.transform.position, position);
            float duration = distance * distanceToDurationRatio;
            Debug.Log($"{GetType()} - {duration}");

            await Tween.Position(paw.transform, position, new TweenSettings
            {
                duration = duration
            });
        }

        private async UniTask RaiseHand(AntPaw paw)
        {
            await Tween.Scale(paw.transform, _raisePawScaleTws);
            paw.ColliderEnabled = false;
        }

        private async UniTask LowerHand(AntPaw paw)
        {
            await Tween.Scale(paw.transform, _lowerPawScaleTws);
            paw.ColliderEnabled = true;
        }

        #region debug
        [Button]
        private void _RaiseHand()
        {
            RaiseHand(_paw1);
        }

        [SerializeField] private Vector3 d_movePos;
        [Button]
        private void _MoveToPosition()
        {
            MoveToPlayerPosition(_paw1, d_movePos, 1f);
        }

        [Button]
        private void _LowerHand()
        {
            LowerHand(_paw1);
        }
        #endregion
    }
}
