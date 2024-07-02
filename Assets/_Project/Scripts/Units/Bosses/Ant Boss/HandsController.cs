using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using PrimeTween;
using UnityEngine;

namespace Core.Units
{
    using Hand = Transform;
    using Player = Player.Player;

    public class HandsController : MonoBehaviour
    {
        [SerializeField] private Hand _hand1;
        [SerializeField] private Hand _hand2;

        [SerializeField] private TweenSettings<Vector3> _raiseHand_ScaleTws;
        [SerializeField] private TweenSettings<Vector3> _lowerHand_ScaleTws;

        private Player _player;

        private void Awake()
        {
            _player = FindObjectOfType<Player>();
        }

        private void Play()
        {

        }

        private void MoveToPosition(Transform hand, Vector3 position)
        {
            Tween.Position(hand, position, 2f);
        }

        private void RaiseHand(Transform hand)
        {
            Tween.Scale(hand, _raiseHand_ScaleTws);
        }

        private void LowerHand(Transform hand)
        {
            Tween.Scale(hand, _lowerHand_ScaleTws);
        }

        #region debug
        [Button]
        private void _RaiseHand()
        {
            RaiseHand(_hand1);
        }

        [SerializeField] private Vector3 d_movePos;
        [Button]
        private void _MoveToPosition()
        {
            MoveToPosition(_hand1, d_movePos);
        }

        [Button]
        private void _LowerHand()
        {
            LowerHand(_hand1);
        }
        #endregion
    }
}
