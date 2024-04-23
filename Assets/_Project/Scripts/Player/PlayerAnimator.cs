using Spine.Unity;
using UnityEngine;

namespace Core.Units
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation _sideSkeletonAnimation;
        [SerializeField] private SkeletonAnimation _frontSkeletonAnimation;

        private Vector2 _input;

        private bool _isSide = true;

        private void Update()
        {
            GetInput();
            if (_input.x != 0)
            {
                if (!_isSide)
                {
                    _isSide = true;
                    _sideSkeletonAnimation.gameObject.SetActive(true);
                    _frontSkeletonAnimation.gameObject.SetActive(false);
                    _sideSkeletonAnimation.AnimationState.SetAnimation(0, "walk", true);
                }

                if (_input.x > 0)
                {
                    _sideSkeletonAnimation.skeleton.ScaleX = 1;
                }
                else
                {
                    _sideSkeletonAnimation.skeleton.ScaleX = -1;
                }
            }
            else if (_input.y != 0)
            {
                if (_isSide)
                {
                    _isSide = false;
                    _sideSkeletonAnimation.gameObject.SetActive(false);
                    _frontSkeletonAnimation.gameObject.SetActive(true);
                    _frontSkeletonAnimation.AnimationState.SetAnimation(0, "walk", true);
                }
            }

        }

        private void GetInput()
        {
            _input.x = Input.GetAxisRaw("Horizontal");
            _input.y = Input.GetAxisRaw("Vertical");
        }

    }
}