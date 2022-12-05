using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Bullet;
using WE.Support;
using WE.Unit;

using Sirenix.OdinInspector;
using WE.Effect;
using WE.Manager;

namespace WE.Skill
{
    public class ShockWaveSupportActiveSkill : ActiveSkill
    {
        public float baseInterval = 1f;
        public AnimationCurve scaleCurve;
        public AnimationEffect fxShockWave;
        public float fxTime = 0.4f;
        public LayerMask layerCast;
        public AudioClip Sfx;
        protected override void ExcuteSkill()
        {
            StartCoroutine(IEExcute());
        }
        IEnumerator IEExcute()
        {
            for (int i = 0; i < BaseHitEffect; i++)
            {
                ShotBullet();
                yield return new WaitForSeconds(baseInterval / (1 + Player.Instance.CooldownReduction / 100));
            }
        }
        public void ShotBullet()
        {
            if(Sfx != null)
                SoundManager.Instance.PlaySoundFx(Sfx);
            Helper.SpawnEffect(fxShockWave, transform.position, this.transform, BaseAreaScale);
            ShockWaveSupportBullet b = Helper.SpawnBullet<ShockWaveSupportBullet>(bulletPrefabs, transform.position, Quaternion.identity, this.transform);
            b.scaleEase = scaleCurve;
            b.scaleTime = fxTime;
            b.InitBullet(BaseDamage, PushBackForce, BaseBulletSpeed, Maxdistance, BaseAreaScale);
        }
        //IEnumerator IECastDamage()
        //{
        //    List<Enemy> listEffectedE = new List<Enemy>();
        //    Helper.SpawnEffect(fxShockWave, transform.position, null, BaseAreaScale);
        //    Vector3 pos = transform.position;
        //    float stepDis = BaseAreaScale / 2 / hitCast;
        //    float stepTime = fxTime / hitCast;
        //    float dis = stepDis;
        //    Helper.CastEnemyBullet(transform.position, BaseAreaScale / 2, layerCast);
        //    for (int i = 0; i < hitCast; i++)
        //    {
        //        Helper.CastDamage(pos, dis, layerCast, baseDamage, pushBackForce, null, null, 1, true);
        //    }
        //    yield return new WaitForSeconds(0.35f);
            
        //}
    }
}

