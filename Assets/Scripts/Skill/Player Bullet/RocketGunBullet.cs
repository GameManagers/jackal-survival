using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Effect;
using WE.Manager;
using WE.Support;
using WE.Unit;
namespace WE.Bullet
{
    public class RocketGunBullet : PlayerBullet
    {
        public override void Hit(Enemy e)
        {
            CastDamage();
        }
        public void CastDamage()
        {
            PlayFx(transform.position);
            Helper.CastDamage(transform.position, AreaScale / 2, layerCast, BulletDamage, PushBackForce);
            Despawn();
        }
        public override void OnMaxDistance()
        {
            CastDamage();
        }
    }
}

