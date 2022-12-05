using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Effect;
using WE.Pooling;
using WE.Support;
using WE.Unit;
using WE.Utils;
using DG.Tweening;
using WE.Manager;

namespace WE.Bullet
{
    public class PlayerBullet : PoolingObject
    {
        public virtual float BulletDamage => bulletDamage;
        public virtual float BulletSpeed => bulletSpeed;
        public virtual float MaxDistance => maxDistance;
        public float PushBackForce => pushBackForce;
        public float AreaScale => areaScale;
        public float Duration => duration;
        public float Interval => interval;
        protected float areaScale;
        public bool CanImpactObstacle;
        public AnimationEffect hitFx;
        public AudioClip hitSfx;
        public float fxScale = 1;
        public LayerMask layerCast;
        public bool piercing;
        protected float pushBackForce;
        protected float bulletDamage;
        protected float bulletSpeed;
        protected float maxDistance;
        protected float duration;
        protected float interval;
        protected bool IsInitilize = false;
        public override void Despawn()
        {
            IsInitilize = false;
            base.Despawn();
        }
        public virtual void InitBullet(float _bulletDamage, float _pushBackForce, float _bulletSpeed = 0, float _bulletMaxdistance = 0, float _areaScale = 1, float _duration = 0, float _interval = 0)
        {
            bulletDamage = _bulletDamage;
            bulletSpeed = _bulletSpeed;
            maxDistance = _bulletMaxdistance;
            pushBackForce = _pushBackForce;
            areaScale = _areaScale;
            duration = _duration;
            interval = _interval;
            IsInitilize = true;
            ShotBullet();
        }
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (IsInitilize)
            {
                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Hit(enemy);
                }
                else if (CanImpactObstacle)
                {
                    ImpactObstacle();
                }
            }
        }
        public virtual void PlayFx(Vector3 pos)
        {
            if (hitFx != null)
            {
                Helper.SpawnEffect(hitFx, pos, null, fxScale);
            }
            if(hitSfx != null)
            {
                SoundManager.Instance.PlaySoundFx(hitSfx);
            }
        }
        public virtual void Hit(Enemy e)
        {
            PlayFx(e.transform.position);
            e.TakeDamage(bulletDamage, pushBackForce); 
            if(!piercing)
                Despawn();
        }
        public virtual void ImpactObstacle()
        {
            if (!piercing)
                Despawn();
        }
        public virtual void ShotBullet()
        {
            transform.DOKill();
            transform.DOMove(transform.position + transform.up * MaxDistance, BulletSpeed).SetEase(Ease.Linear).SetSpeedBased().OnComplete(() => { OnMaxDistance(); });
        }
        public virtual void OnMaxDistance()
        {
            Despawn();
        }
    }
}

