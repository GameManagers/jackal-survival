using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Support;
using DG.Tweening;
using WE.Effect;

namespace WE.Bullet
{
    public class DroneBullet : PlayerBullet
    {
        public AnimationEffect fxPrefabs;
        public float warningScale = 0.3f;
        public Vector3 tarPos;
        AnimationEffect warningIcon;
        public override void ShotBullet()
        {
            warningIcon = Helper.SpawnEffect(fxPrefabs, tarPos, null, AreaScale * warningScale);
            Vector3 dir = tarPos - transform.position;
            float angle = Helper.Get2DAngle(dir);
            int rand = Random.Range(0, 2);
            if (rand == 0) rand = -1;
            float randAngle = Random.Range(45, 75);
            angle += rand * randAngle;
            Vector3 pos1 = Helper.GetPositionFromAngle(transform.position, angle, dir.magnitude * Helper.Cos(randAngle));
            transform.DOPath(new Vector3[] { pos1, tarPos }, BulletSpeed, PathType.CatmullRom, PathMode.TopDown2D)
                .SetLookAt(0)
                .SetSpeedBased().SetEase(Ease.Linear).OnComplete(() => { CastDamage(); });
        }

        private void CastDamage()
        {
            PlayFx(transform.position);
            Helper.CastDamage(transform.position, AreaScale/2, layerCast, BulletDamage, PushBackForce);
            if (warningIcon != null)
                Helper.Despawn(warningIcon);
            Despawn();
        }
    }
}

