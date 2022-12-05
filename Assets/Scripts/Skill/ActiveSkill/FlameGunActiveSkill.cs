using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Effect;
using WE.Manager;
using WE.Support;
using WE.Unit;
using UniRx;
using WE.Bullet;
using Sirenix.OdinInspector;

namespace WE.Skill
{
    public class FlameGunActiveSkill : ActiveSkill
    {
        public AnimationEffect flameFx;
        public float randomAngle = 15;
        public CircleCollider2D damagingCircle;
        public LayerMask layerCast;
        [FoldoutGroup("Evole SKill")]
        public float damageMul = 0.5f, intervalMul = 1f, durationMul = 0.7f;

        public AudioClip gunSfx;
        public override void Init()
        {
            base.Init();
            damagingCircle.transform.SetParent(firePos);
            damagingCircle.radius = BaseAreaScale / 2;
            damagingCircle.transform.localPosition = new Vector3(0, 1.1f, 0) * BaseAreaScale / 2;
        }
        public override void Dispose()
        {
            damagingCircle.transform.SetParent(this.transform);
            base.Dispose();
        }
        protected override void ExcuteSkill()
        {
            Player.Instance.UseWeapon();
            StopMiniObserve();
            miniDisposables = new CompositeDisposable();
            Observable.Interval(System.TimeSpan.FromSeconds(1f)).Subscribe(_ => Player.Instance.UseWeapon()).AddTo(miniDisposables);
            Observable.Timer(System.TimeSpan.FromSeconds(BaseDuration)).Subscribe(_ => StopBurn()).AddTo(miniDisposables);
            Observable.Interval(System.TimeSpan.FromSeconds(1.0f / (BaseHitPerSec + BaseBulletNumb))).Subscribe(_ => CastDamage()).AddTo(miniDisposables);
            Observable.Interval(System.TimeSpan.FromSeconds(1.0f / (BaseHitPerSec + BaseBulletNumb) / 2)).Subscribe(_ => PlayFx()).AddTo(miniDisposables);

        }
        public void PlayFx()
        {
            if (gunSfx != null)
            {
                SoundManager.Instance.PlaySoundFx(gunSfx);
            }
            AnimationEffect fx = Helper.SpawnEffect(flameFx, firePos.position, null, Random.Range(BaseAreaScale * 0.8f, BaseAreaScale * 1.2f));
            fx.transform.eulerAngles = firePos.eulerAngles + new Vector3(0, 0, Random.Range(-randomAngle, randomAngle));
            fx.PlayTime = 0.5f / BaseBulletSpeed;
        }
        public void StopBurn()
        {
            StopMiniObserve();

        }
        public void CastDamage()
        {
            //PlayFx();
            Enemy[] eArray = Helper.CastDamage(damagingCircle.transform.position, damagingCircle.radius, layerCast, BaseDamage, PushBackForce);
            if (IsEvoleSkill)
            {
                if (eArray != null)
                    for (int i = 0; i < eArray.Length; i++)
                    {
                        if (eArray[i] != null)
                        {
                            if (eArray[i].IsAlive)
                            {
                                FlameGunBurningBullet bul = eArray[i].GetComponentInChildren<FlameGunBurningBullet>();
                                if (bul == null)
                                    bul = Helper.SpawnBullet<FlameGunBurningBullet>(bulletPrefabs, eArray[i].transform.position, Quaternion.identity, eArray[i].transform);
                                bul.gameObject.SetActive(true);
                                bul.e = eArray[i];
                                bul.damageMul = this.damageMul;
                                bul.intervalMul = this.intervalMul;
                                bul.durationMul = this.durationMul;
                                InitBullet(bul);
                            }
                        }

                    }
            }
        }

    }
}


