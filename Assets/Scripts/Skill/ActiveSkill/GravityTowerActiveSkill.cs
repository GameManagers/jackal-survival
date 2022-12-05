using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Bullet;
using WE.Support;
using WE.Unit;

using Sirenix.OdinInspector;
namespace WE.Skill
{
    public class GravityTowerActiveSkill : ActiveSkill
    {
        protected override void ExcuteSkill()
        {
            float angle = Random.Range(0, 360);
            float stepAngle = 360 / BaseBulletNumb;
            for (int i = 0; i < BaseBulletNumb; i++)
            {
                SpawnTower(angle + stepAngle * i);
            }
        }
        public void SpawnTower(float angle)
        {
            Vector3 pos = Helper.GetPositionFromAngle(Player.Instance.transform.position, angle, Maxdistance * (1 + Player.Instance.AreaEffectIncrease / 100));
            GravityTower tower = Helper.SpawnBullet<GravityTower>(bulletPrefabs, pos, Quaternion.identity, null);
            InitBullet(tower);
        }
    }
}

