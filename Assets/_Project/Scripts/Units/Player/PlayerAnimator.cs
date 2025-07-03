using Spine.Unity;
using UnityEngine;

namespace Core.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation _skeletonAnimation;

        private bool _isSide = true;

        public void WalkingDirectionInput(Vector2 input)
        {
            if (input.x != 0)
            {
                SetSkin(input.x > 0 ? SkinDirection.Right : SkinDirection.Left);
            }
            else if (input.y != 0)
            {
                SetSkin(input.y > 0 ? SkinDirection.Back : SkinDirection.Front);
            }
        }

        public void StartWalking()
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, "walk", true);
            _skeletonAnimation.timeScale = 2f;
        }

        public void StopWalking()
        {
            _skeletonAnimation.timeScale = 0f;
        }

        private void SetSkin(SkinDirection skinDirection)
        {
            _skeletonAnimation.Skeleton.SetSkin(GetSkinName(skinDirection));
            _skeletonAnimation.Skeleton.SetSlotsToSetupPose();

            if (skinDirection == SkinDirection.Left)
            {
                _skeletonAnimation.Skeleton.ScaleX = -1;
            }
            else if (skinDirection == SkinDirection.Right)
            {
                _skeletonAnimation.Skeleton.ScaleX = 1;
            }
        }

        private string GetSkinName(SkinDirection skinDirection) =>
            skinDirection switch
            {
                SkinDirection.Back => "back",
                SkinDirection.Front => "front",
                SkinDirection.Left or SkinDirection.Right => "side",
            };


        public enum SkinDirection
        {
            Back,
            Front,
            Left,
            Right,
        }
    }
}