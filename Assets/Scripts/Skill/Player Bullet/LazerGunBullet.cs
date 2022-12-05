using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using WE.Support;

namespace WE.Bullet
{
    public class LazerGunBullet : PlayerBullet
    {
        public SpriteRenderer lazerIcon;
        public BoxCollider2D checkCollider;
        public BoxCollider2D hitCollider;
        public override void ShotBullet()
        {
            Vector2 size = new Vector2(MaxDistance, areaScale);
            lazerIcon.size = size;
            checkCollider.size = size;
            hitCollider.size = size;
            checkCollider.offset = Vector2.zero;
            checkCollider.transform.localPosition = new Vector3(MaxDistance / 2, 0, 0);
            hitCollider.offset = checkCollider.transform.localPosition;
            disposable = new CompositeDisposable();
            Observable.Timer(System.TimeSpan.FromSeconds(Duration)).Subscribe(_ => Despawn()).AddTo(disposable);
            Observable.Interval(System.TimeSpan.FromSeconds(Interval)).Subscribe(_ => CastDamage()).AddTo(disposable);
        }
        public void CastDamage()
        {
            Helper.CastDamage(checkCollider.transform.position, checkCollider.size, checkCollider.transform.eulerAngles.z, layerCast, BulletDamage, pushBackForce);
        }
    }
}

