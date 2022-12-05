using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Bullet;
using WE.Support;
using WE.Unit;
using UniRx;
using Sirenix.OdinInspector;
namespace WE.Skill
{
    public class MolotovActiveSkill : ActiveSkill
    {
        public float throwInterval = 0.3f;
        public float stepAngle = 20;
        public float angleRotate = 2700;
        protected int currentBulet;
        protected override void ExcuteSkill()
        {
            float angle = Random.Range(0, 360);
            float _stepAngle = 360.0f / BaseBulletNumb;
            if (_stepAngle < stepAngle)
                stepAngle = _stepAngle;
            currentBulet = 0;
            StopMiniObserve();
            miniDisposables = new CompositeDisposable();
            Observable.Interval(System.TimeSpan.FromSeconds(throwInterval *(1- Player.Instance.CooldownReduction/100))).Subscribe(_ => ShotBullet(angle)).AddTo(miniDisposables);
        }
        public void ShotBullet(float angle)
        {
            if(currentBulet < BaseBulletNumb)
            {
                currentBulet++;
                MolotovBullet bullet = Helper.SpawnBullet<MolotovBullet>(bulletPrefabs, Player.Instance.transform.position, Quaternion.identity, null);
                bullet.rotateAngle = angleRotate;
                bullet.tarGetPos = Helper.GetPositionFromAngle(transform.position, angle + stepAngle * currentBulet, Maxdistance * BaseAreaScale);
                InitBullet(bullet);
            }
            else
            {
                if (miniDisposables != null)
                    miniDisposables.Dispose();
            }
        }
    }
}

