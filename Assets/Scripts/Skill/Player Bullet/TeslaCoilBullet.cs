using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Support;
using WE.Effect;
using WE.Manager;
using DG.Tweening;
using WE.Unit;

namespace WE.Bullet
{
    public class TeslaCoilBullet : PlayerBullet
    {
        public int chainTime;
        public override void ShotBullet()
        {
            BulletFly();
        }
        void BulletFly()
        {
            if (chainTime <= 0)
            {
                Despawn();
                return;
            }
            if(hitSfx!=null)
            SoundManager.Instance.PlaySoundFx(hitSfx);
            chainTime--;
            float minDistance = ResolutionManager.Instance.ScreenHigh / 3;
            Vector3 tarPos = Helper.GetRandomPosInScreen();
            int t = 0;
            while (Vector3.SqrMagnitude(tarPos - transform.position) < minDistance * minDistance)
            {
                tarPos = Helper.GetRandomPosInScreen();
                t++;
                if (t > 100)
                {
                    DebugCustom.LogError("Can't get random pos, check logic");
                    break;
                }
            }
            transform.localEulerAngles = new Vector3(0, 0, Helper.Get2DAngle(tarPos - transform.position));
            AnimationEffect fx = Helper.SpawnEffect(hitFx, transform.position, this.transform);
            fx.transform.localEulerAngles = Vector3.zero;
            fx.PlayTime = Vector3.Distance(tarPos, transform.position)/ BulletSpeed/3;
            transform.DOMove(tarPos, BulletSpeed).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() => {
                fx.Despawn();
                BulletFly(); });
        }
        public override void PlayFx(Vector3 pos)
        {
            
        }
        public override void Hit(Enemy e)
        {
            e.TakeDamage(bulletDamage, pushBackForce, null, true);
        }
    }
}

