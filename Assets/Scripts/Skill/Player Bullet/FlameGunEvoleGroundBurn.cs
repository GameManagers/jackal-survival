using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Effect;
using WE.Support;
using UniRx;
namespace WE.Bullet
{
    public class FlameGunEvoleGroundBurn : PlayerBullet
    {
        AnimationEffect fx;
        public override void ShotBullet()
        {
            transform.localScale = Vector3.one * AreaScale;
            fx = Helper.SpawnEffect(hitFx, transform.position, null, AreaScale);
            disposable = new CompositeDisposable();
            Observable.Timer(System.TimeSpan.FromSeconds(duration)).Subscribe(_ => Despawn()).AddTo(disposable);
            Observable.Interval(System.TimeSpan.FromSeconds(interval)).Subscribe(_ => CastDamage()).AddTo(disposable);
        }
        public void CastDamage()
        {
            Helper.CastDamage(transform.position, AreaScale /2 ,  layerCast, BulletDamage, 0);
            Helper.Despawn(fx);
        }
        public override void PlayFx(Vector3 pos)
        {
            
        }
    }
}

