using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using WE.Support;

namespace WE.Bullet
{
    public class ZeusSpearChainLightning : PlayerBullet
    {
        public SpriteRenderer lightningIcon;
        public BoxCollider2D hitCollider;
        public float distance;
        public override void ShotBullet()
        {
            hitFx.OnSpawn();
            Vector2 size = new Vector2(distance, lightningIcon.size.y);
            lightningIcon.size = size;
            hitCollider.size = new Vector2(size.x, AreaScale);
            hitCollider.offset = Vector2.zero;
            hitCollider.transform.localPosition = new Vector3(distance / 2, 0, 0);
            disposable = new CompositeDisposable();
            Observable.Interval(System.TimeSpan.FromSeconds(Interval)).Subscribe(_ => CastDamage()).AddTo(disposable);
        }
        public void CastDamage()
        {
            Helper.CastDamage(hitCollider.transform.position, hitCollider.size, hitCollider.transform.eulerAngles.z, layerCast, BulletDamage, pushBackForce, null, null, 1, true, hitSfx);
        }
    }
}

