using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.GameAction.Tank;
using WE.Manager;

namespace WE.Tank
{
    public class TankAnimation : MonoBehaviour
    {
        //[SerializeField]
        //private AudioClip clipTankMove;
        [SerializeField]
        private SkeletonAnimation spineBody;
        [SerializeField]
        private SkeletonAnimation spineGun;

        public BaseBodyAnimation baseBodyAnimation;
        private BaseGunAnimation baseGunAnimation;
        //private AudioSource cachedAudioSource = null;
        private bool isMoving = false;
        public EAnimationState State { get; set; }

        private void Start()
        {
            Init();
        }
        public void Init()
        {
            baseBodyAnimation.Init(spineBody, this);
            isMoving = false;
            State = EAnimationState.None;
        }
        //private void OnDisable()
        //{
        //    DespawnAudioSource();
        //}
        //private void DespawnAudioSource()
        //{
        //    if (cachedAudioSource != null)
        //    {
        //        cachedAudioSource.Stop();
        //        cachedAudioSource.clip = null;
        //        Utils.Utils.Despawn(cachedAudioSource.gameObject);
        //        cachedAudioSource = null;
        //    }
        //}

        public void Shot()
        {
            baseBodyAnimation.Shot();
            baseGunAnimation.Shot();

        }



        public void Moving()
        {

            if (!isMoving)
            {
                //if (cachedAudioSource == null)
                //    cachedAudioSource = SoundManager.Instance.GetSoundFx(clipTankMove, true);
                //else
                //{
                //    if(cachedAudioSource.enabled && cachedAudioSource.gameObject.activeInHierarchy)
                //        cachedAudioSource.Play();
                //}
                baseBodyAnimation.Move();
            }
            isMoving = true;
        }
        public void Idle()
        {
            isMoving = false;
            baseBodyAnimation.Idle();
            //baseGunAnimation.Idle();
            //if (cachedAudioSource != null)
            //    cachedAudioSource.Stop();

        }

        public void Pause()
        {
            baseBodyAnimation.Pause();
            baseGunAnimation.Pause();
            //if (cachedAudioSource != null)
            //    cachedAudioSource.Stop();

        }
        public void Resume()
        {
            baseGunAnimation.Resume();
            baseBodyAnimation.Resume();
            //if (cachedAudioSource != null)
            //    cachedAudioSource.Play();
        }

        public void SetBody(SkeletonDataAsset dataAsset)
        {
            spineBody.skeletonDataAsset = dataAsset;
            spineBody.Initialize(true);
            baseBodyAnimation = gameObject.AddComponent<BaseBodyAnimation>();
            baseBodyAnimation.Init(spineBody, this);

        }
        public void SetWeapon(SkeletonDataAsset dataAsset, string baseIDWeapon)
        {
            spineGun.skeletonDataAsset = dataAsset;
            spineGun.Initialize(true);
            if (baseIDWeapon != "Rocket Launcher")
            {
                baseGunAnimation = gameObject.AddComponent<BaseGunAnimation>();
            }
            else
            {
                //baseGunAnimation = gameObject.AddComponent<RocketLaucherGunAnimation>();
            }
            baseGunAnimation.Init(spineGun, this);
        }
    }

}