using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Bullet;
using WE.Manager;
using WE.Support;
using UniRx;
using WE.Unit;
using Sirenix.OdinInspector;

namespace WE.Skill
{
    public class ShotBulletActiveSkill : ActiveSkill
    {
        public float randomAngle = 10;
        public float randomOffset = 0.2f;
        public float minTimeShot = 0.1f;
        public float maxTimeShot => BaseCoolDonw * 0.8f;
        protected int currentBullet = 0;

        public AudioClip gunSfx;
        public override void Init()
        {
            base.Init();

        }
        protected override void ExcuteSkill()
        {
            Player.Instance.UseWeapon();
            float bulletWait = minTimeShot;
            if (BaseBulletNumb * minTimeShot > maxTimeShot)
            {
                bulletWait = maxTimeShot / BaseBulletNumb;
            }
            float angle = Helper.Get2DAngle(gunTransform.right);
            StopMiniObserve();
            miniDisposables = new CompositeDisposable();
            currentBullet = 0;
            Observable.Interval(System.TimeSpan.FromSeconds(bulletWait)).Subscribe(_ => ShotBullet(angle)).AddTo(miniDisposables);
        }
        public virtual void ShotBullet(float angle)
        {
            if(currentBullet < BaseBulletNumb)
            {
                currentBullet++;
                float _angle = Random.Range(angle - randomAngle, angle + randomAngle);
                Vector3 pos = firePos.position + (Vector3)Random.insideUnitCircle * Random.Range(0, randomOffset);
                ShotBullet(pos, _angle);
            }
            else
            {
                miniDisposables.Dispose();
            }
        }
        public virtual void ShotBullet(Vector3 pos,float angle)
        {
            if (gunSfx != null)
            {
                SoundManager.Instance.PlaySoundFx(gunSfx);
            }
            CarController.Instance.PlayFx();
            PlayerBullet bullet = Helper.SpawnBullet(bulletPrefabs, pos, Quaternion.identity, null);
            bullet.transform.eulerAngles = new Vector3(0, 0, angle);
            InitBullet(bullet);
        }

    }
}

