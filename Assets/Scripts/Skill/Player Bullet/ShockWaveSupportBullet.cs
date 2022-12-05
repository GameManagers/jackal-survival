using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WE.Unit;

namespace WE.Bullet
{
    public class ShockWaveSupportBullet : PlayerBullet
    {
        public float scaleTime;
        public AnimationCurve scaleEase;
        public override void ShotBullet()
        {
            transform.localScale = Vector3.one * 0.1f;
            transform.DOScale(Vector3.one * AreaScale, scaleTime).SetEase(scaleEase).OnComplete(() =>
                {
                    Despawn();
                });
        }
        public override void Hit(Enemy e)
        {
            e.TakeDamage(bulletDamage, pushBackForce, null, true);
        }
    }
}

