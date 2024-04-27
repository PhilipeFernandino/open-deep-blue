using Spine.Unity;
using UnityEngine;

namespace Core.Units
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation _skeletonAnimation;

        private Vector2 _input;

        private bool _isSide = true;

        private void Update()
        {
            GetInput();
            if (_input.y != 0)
            {
                if (_isSide)
                {
                    _isSide = false;
                    _skeletonAnimation.AnimationState.SetAnimation(0, "walk", true);
                }

                if (_input.y > 0)
                {
                    _skeletonAnimation.Skeleton.SetSkin("back");
                    _skeletonAnimation.Skeleton.SetSlotsToSetupPose();
                }
                else
                {
                    _skeletonAnimation.Skeleton.SetSkin("front");
                    _skeletonAnimation.Skeleton.SetSlotsToSetupPose();
                }
            }
            else if (_input.x != 0)
            {
                // Set anim if not already set
                if (!_isSide)
                {
                    _isSide = true;
                    _skeletonAnimation.AnimationState.SetAnimation(0, "walk", true);
                    _skeletonAnimation.Skeleton.SetSkin("side");
                    _skeletonAnimation.Skeleton.SetSlotsToSetupPose();
                }

                // Flip accordingly
                if (_input.x > 0)
                {
                    _skeletonAnimation.skeleton.ScaleX = 1;
                }
                else
                {
                    _skeletonAnimation.skeleton.ScaleX = -1;
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