using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using WE.Support;

namespace WE.SkillEnemy
{
    public class EnemyShotSkill : EnemyTimerAttackSkill
    {
        [InfoBox("Max Distance = 0 is shot to player position")]
        public float maxDistance;
        protected void ShotBullet()
        {
            EnemyBullet b = Helper.SpawnEnemyBullet(bulletPrefabs, transform.position, Quaternion.identity, null);
            b.InitBullet(damage, bulletSpeed, Owner, maxDistance, parentScale);
        }
        public override void ExcuteSkill()
        {
            base.ExcuteSkill();
            ShotBullet();
        }
    }
}

