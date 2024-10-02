using Coimbra.Services;
using Core.CameraSystem;
using Core.Utils;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using PrimeTween;
using System.Collections.Generic;
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
        [SerializeField] private float _trackerSpeed;

        private Player _player;

        private bool _isIdle;
        private AntPaw _currentPaw;

        private HashSet<Track> _track;

        private ICameraService _cameraService;

        private void Awake()
        {
            _track = new();

            _player = FindObjectOfType<Player>();
            _currentPaw = _paw1;
        }

        private void Start()
        {
            JumpingPawsAttackFlow().Forget();

            _cameraService = ServiceLocatorUtilities.GetServiceAssert<ICameraService>();
        }

        private void Update()
        {
            foreach (var track in _track)
            {
                track.Tracker.position = Vector2.Lerp(track.Tracker.position.XY(), track.Target.position.XY(), track.Speed * Time.deltaTime);
            }
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

            var tracker = new Track(_currentPaw.transform, _player.transform, _trackerSpeed);
            _track.Add(tracker);

            await LowerPaw(paw);

            _track.Remove(tracker);
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
            await MoveToPosition(_currentPaw, _player.Position, _distanceToDurationRatio);

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

            _cameraService.ShakeCamera(_shakeCameraSettings);
        }

        private async UniTask RotatePaw(AntPaw paw)
        {

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

public class Track : IEqualityComparer<Track>
{
    private Transform _tracker;
    private Transform _target;

    private float _speed;

    public Transform Tracker => _tracker;
    public Transform Target => _target;
    public float Speed => _speed;

    public Track(Transform tracker, Transform target, float speed)
    {
        _tracker = tracker;
        _target = target;
        _speed = speed;
    }


    /// <summary>
    /// Only compares the tracker as it can only track one object at a time. The target can be tracked by many objects
    /// </summary>
    public bool Equals(Track x, Track y)
    {
        return x.Tracker.Equals(y.Tracker);
    }

    public int GetHashCode(Track obj)
    {
        return obj.Tracker.GetHashCode();
    }
}