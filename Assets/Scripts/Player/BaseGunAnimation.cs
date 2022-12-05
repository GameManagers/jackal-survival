using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Tank;
using static WE.Tank.TankAnimation;

namespace WE.GameAction.Tank
{
    public class BaseGunAnimation : MonoBehaviour
    {
        protected string animationGunAttack = "Attack";
        protected string animationGunIdle = "idle";
        protected SkeletonAnimation skeletonAnimation;
        protected TankAnimation tankAnimation;
        public virtual void Init(SkeletonAnimation _skeletonAnimation, TankAnimation _tankAnimation)
        {
            skeletonAnimation = _skeletonAnimation;
            tankAnimation = _tankAnimation;
            InitAnimationName();
            skeletonAnimation.AnimationState.SetAnimation(1, animationGunIdle, true);
        }
        public virtual void Idle()
        {
            if (tankAnimation.State != EAnimationState.Idle)
            {
                skeletonAnimation.AnimationState.SetAnimation(1, animationGunIdle, true);
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


        public virtual void Shot()
        {
            skeletonAnimation.AnimationState.SetAnimation(0, animationGunAttack, false);
            skeletonAnimation.AnimationState.Complete += GunShot_Complete;            
        }

        private void GunShot_Complete(Spine.TrackEntry trackEntry)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, animationGunIdle, true);
            skeletonAnimation.AnimationState.Complete -= GunShot_Complete;
            
        }

        protected virtual void InitAnimationName()
        {
            animationGunAttack = "Attack";
            animationGunIdle = "idle";
        }
    }
}