using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Support;
using WE.Unit;
using WE.Bullet;
using UniRx;
using WE.Manager;

namespace WE.Skill
{
    public class LazerGunActiveSkill : ActiveSkill
    {
        public float offSet = 0.3f;
        bool rotate;
        public AudioClip gunSfx;
        protected override void ExcuteSkill()
        {
            Player.Instance.UseWeapon(BaseDuration);
            ShotLazer(firePos.transform.eulerAngles.z + 90);
            if (BaseBulletNumb > 1)
            {
                for (int i = 0; i < BaseBulletNumb - 1; i++)
                {
                    float angle = Random.Range(0, 360);
                    ShotLazer(angle);
                }
            }
            if (IsEvoleSkill)
            {
                rotate = true;
                StopMiniObserve();
                miniDisposables = new CompositeDisposable();
                Observable.Timer(System.TimeSpan.FromSeconds(BaseDuration)).Subscribe(_ => rotate = false).AddTo(miniDisposables);
            }
            else
            {
                rotate = false;
            }
        }
        void ShotLazer(float angle)
        {
            if (gunSfx != null)
                SoundManager.Instance.PlaySoundFx(gunSfx);
            LazerGunBullet lazer = Helper.Spawn<LazerGunBullet>(bulletPrefabs, transform.position, Quaternion.identity, this.transform);
            lazer.transform.localEulerAngles = new Vector3(0, 0, angle);
            lazer.transform.position = firePos.position + lazer.transform.right * offSet;
            InitBullet(lazer);
        }
        private void LateUpdate()
        {
            if (GameplayManager.Instance.State == GameState.Gameplay && Initilized)
            {
                transform.position = firePos.position;
                if (rotate)
                    transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z + Time.deltaTime * BaseBulletSpeed);
            }
        }
        public override void Stop()
        {
            rotate = false;
            base.Stop();
        }
    }

}
