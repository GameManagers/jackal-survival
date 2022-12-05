using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Support;
using WE.Bullet;
using Sirenix.OdinInspector;
namespace WE.Skill
{
    public class BladeSawSupportActiveSkill : ActiveSkill
    {
        protected override void ExcuteSkill()
        {
            float stepAngle = (float)360 / BaseBulletNumb;
            for (int i = 0; i < BaseBulletNumb; i++)
            {
                BladeSawSupportBullet bullet = Helper.SpawnBullet<BladeSawSupportBullet>(bulletPrefabs, transform.position, Quaternion.identity, this.transform);
                bullet.transform.localEulerAngles = new Vector3(0, 0, stepAngle * i);
                InitBullet(bullet);
            }
        }

        private void Update()
        {
            transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z + BaseBulletSpeed * Time.deltaTime);
        }
    }
}

