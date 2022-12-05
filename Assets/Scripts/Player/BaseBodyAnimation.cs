using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Tank;
using static WE.Tank.TankAnimation;

namespace WE.GameAction.Tank
{
    public class BaseBodyAnimation : MonoBehaviour
    {
        protected string animationBodyIdle = "Idle";
        protected string animationBodyAttack = "Attack";
        protected string animationBodyMove = "Move";
        public SkeletonAnimation skeletonAnimation;
        public TankAnimation tankAnimation;
        public virtual void Init(SkeletonAnimation _skeletonAnimation, TankAnimation _tankAnimation)
        {
            skeletonAnimation = _skeletonAnimation;
            tankAnimation = _tankAnimation;
            skeletonAnimation.AnimationState.SetAnimation(1, animationBodyIdle, true);
        }
        public virtual void Idle()
        {
            if (tankAnimation.State != EAnimationState.Idle)
            {
                skeletonAnimation.AnimationState.SetAnimation(1, animationBodyIdle, true);
                tankAnimation.State = EAnimationState.Idle;
            }
        }


        public virtual void Pause()
        {
            skeletonAnimation.timeScale = 0;
        }

        public virtual void Resume()
        {
            skeletonAnimation.timeScale = 1f;
        }

        public virtual void Move()
        {
            if (tankAnimation.State != EAnimationState.Move)
            {
                skeletonAnimation.AnimationState.SetAnimation(1, animationBodyMove, true);
                tankAnimation.State = EAnimationState.Move;
            }
        }
        public virtual void Shot()
        {
            skeletonAnimation.AnimationState.Complete += BodyShot_Complete;
            skeletonAnimation.AnimationState.SetAnimation(1, animationBodyAttack, false);
        }

        private void BodyShot_Complete(Spine.TrackEntry trackEntry)
        {
            skeletonAnimation.AnimationState.Complete -= BodyShot_Complete;
            string aniState = tankAnimation.State == EAnimationState.Move ? animationBodyMove : animationBodyIdle;
            skeletonAnimation.AnimationState.SetAnimation(1, aniState, true);
        }
    }
}