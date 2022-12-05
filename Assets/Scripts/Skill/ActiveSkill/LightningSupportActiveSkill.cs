using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Manager;
using WE.Support;
using WE.Bullet;
using Sirenix.OdinInspector;
namespace WE.Skill
{
    public class LightningSupportActiveSkill : ActiveSkill
    {
        protected override void ExcuteSkill()
        {
            for (int i = 0; i < BaseBulletNumb; i++)
            {
                CallLightning();
            }
        }
        public void CallLightning()
        {
            Vector3 pos = new Vector3(Random.Range(ResolutionManager.Instance.ScreenLeftEdge + 0.5f, ResolutionManager.Instance.ScreenRightEdge + 0.5f),
                Random.Range(ResolutionManager.Instance.ScreenBottomEdge + 1.5f, ResolutionManager.Instance.ScreenTopEdge - 1.5f), 0);
            LightningSupportBullet bullet = Helper.SpawnBullet<LightningSupportBullet>(bulletPrefabs, 
                pos + Vector3.up * (ResolutionManager.Instance.ScreenHigh + 2), Quaternion.identity, null);
            bullet.tarPos = pos;
            InitBullet(bullet);
        }
    }
}

