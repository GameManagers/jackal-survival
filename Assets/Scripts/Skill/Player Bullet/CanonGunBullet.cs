using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Pooling;
using WE.Support;
using WE.Unit;
namespace WE.Bullet
{
    public class CanonGunBullet : PlayerBullet
    {
        public float ExploseDamageMulti = 0.5f;
        public override void Hit(Enemy e)
        {
            Helper.SpawnEffect(hitFx, transform.position, null, AreaScale);
            //fx.OnSpawn();
            e.TakeDamage(bulletDamage, pushBackForce);
            CastDamage();
            if (!piercing)
                Despawn();
        }
        void CastDamage()
        {
            Helper.CastDamage(transform.position, AreaScale / 2, layerCast, BulletDamage * ExploseDamageMulti, pushBackForce, null, null, 1, false, hitSfx);
        }
    }
}

