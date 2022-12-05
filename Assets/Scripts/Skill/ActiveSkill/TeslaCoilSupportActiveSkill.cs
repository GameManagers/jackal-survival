using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Bullet;
using WE.Support;

using Sirenix.OdinInspector;
namespace WE.Skill
{
    public class TeslaCoilSupportActiveSkill : ActiveSkill
    {
        protected override void ExcuteSkill()
        {
            for (int i = 0; i < BaseBulletNumb; i++)
            {
                TeslaCoilBullet bullet = Helper.SpawnBullet<TeslaCoilBullet>(bulletPrefabs, transform.position, Quaternion.identity, null);
                bullet.chainTime = BaseHitEffect;
                InitBullet(bullet);
            }
        }
    }
}

