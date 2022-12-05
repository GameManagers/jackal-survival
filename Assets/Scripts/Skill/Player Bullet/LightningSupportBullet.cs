using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WE.Support;
using WE.Unit;
using UniRx;
using WE.Manager;

namespace WE.Bullet
{
    public class LightningSupportBullet : PlayerBullet
    {
        public Vector3 tarPos;
        public float timeFly;
        public bool showWarning;
        public Pooling.PoolingObject ringWarning;
        Pooling.PoolingObject warningObj;
        public override void ShotBullet()
        {
            if (showWarning)
            {
                warningObj = Helper.Spawn<Pooling.PoolingObject>(ringWarning, tarPos, Quaternion.identity, null);
                warningObj.transform.localScale = Vector3.one * areaScale;
            }
            disposable = new CompositeDisposable();
            Observable.Timer(System.TimeSpan.FromSeconds(1)).Subscribe(_ =>
                transform.DOMove(tarPos, timeFly / (1 + Player.Instance.BulletSpeedIncrease / 100)).SetEase(Ease.Linear).OnComplete(() => { CastDamage(); })
            ).AddTo(disposable);
            
        }
        public void CastDamage()
        {
            if (showWarning)
            {
                Helper.Despawn(warningObj);
                warningObj = null;
            }
            Helper.SpawnEffect(hitFx, tarPos, null, areaScale * fxScale);
            if(hitSfx != null)
                SoundManager.Instance.PlaySoundFx(hitSfx);
            Helper.CastDamage(tarPos, areaScale / 2, layerCast, BulletDamage, pushBackForce);
            Despawn();
        }
    }
}

