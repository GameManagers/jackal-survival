using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Support;
using WE.Unit;

namespace WE.Bullet
{
    public class ShockZoneBullet : PlayerBullet
    {
        public GameObject childFx;
        public override void ShotBullet()
        {
            transform.localScale = Vector3.one * areaScale ;
            CastDamage();
        }
        public void CastDamage()
        {
            Helper.CastDamage(transform.position, AreaScale /2, layerCast,BulletDamage, PushBackForce);
        }
    }
}

