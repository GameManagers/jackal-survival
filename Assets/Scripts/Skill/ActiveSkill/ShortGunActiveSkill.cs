using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Bullet;
using WE.Support;

using Sirenix.OdinInspector;
using WE.Unit;
using WE.Manager;

namespace WE.Skill
{
    public class ShortGunActiveSkill : ActiveSkill
    {
        public float bulletAngleDistance = 10;
        public float maxSpreadAngle = 60;
        public AudioClip gunSfx;
        protected override void ExcuteSkill()
        {
            if (gunSfx != null)
                SoundManager.Instance.PlaySoundFx(gunSfx);
            Player.Instance.UseWeapon();
            float _angle = maxSpreadAngle * BaseAreaScale / BaseBulletNumb;
            if (_angle > bulletAngleDistance)
                _angle = bulletAngleDistance;
            float angle = Helper.Get2DAngle(gunTransform.right);
            float startAngle = angle - _angle * (BaseBulletNumb - 1) / 2;
            for (int i = 0; i < BaseBulletNumb; i++)
            {
                ShotBullet(firePos.position, startAngle + _angle * i);
            }
        }
        public void ShotBullet(Vector3 pos, float angle)
        {
            CarController.Instance.PlayFx();
            PlayerBullet bullet = Helper.SpawnBullet(bulletPrefabs, pos, Quaternion.identity, null);
            bullet.transform.eulerAngles = new Vector3(0, 0, angle);
            InitBullet(bullet);
        }
    }
}

