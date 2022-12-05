using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WE.Bullet
{
    public class ShortGunBullet : PlayerBullet
    {
        public override void ShotBullet()
        {
            float dis = duration * bulletSpeed;
            transform.DOMove(transform.position + transform.up * dis, Duration).SetEase(Ease.Linear).OnComplete(() => { Despawn(); });
        }
    }
}

