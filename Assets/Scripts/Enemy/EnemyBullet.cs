using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WE.Effect;
using WE.Unit;
using WE.Pooling;
using Sirenix.OdinInspector;
using WE.Support;

namespace WE.SkillEnemy
{
    public class EnemyBullet : PoolingObject
    {
        public AnimationEffect hitFx;
        public float fxScale = 1;
        public bool piecring;
        public bool isRotate;
        [ShowIf("isRotate", true)]
        public float rotateSpeed;
        protected float bulletSpeed;
        protected float bulletDamage;
        protected float maxDistance;
        protected float parentScale;
        protected Enemy Owner;
        public System.Action<EnemyBullet> OnHitPlayer;
        public System.Action<EnemyBullet> OnMaxDistanceReach;
        public virtual void InitBullet(float _damage, float _bulletSpeed, Enemy _owner, float _maxDistance = 0, float _parentScale = 1)
        {
            bulletSpeed = _bulletSpeed;
            bulletDamage = _damage;
            maxDistance = _maxDistance;
            Owner = _owner;
            ShotBullet();
        }
        public virtual void ShotBullet()
        {
            Vector3 pos = Player.Instance.transform.position;
            if (maxDistance > 0)
            {
                pos = transform.position + (Player.Instance.transform.position - transform.position).normalized * maxDistance;
            }
            transform.localEulerAngles = new Vector3(0, 0, Helper.Get2DAngle(pos - transform.position));
            transform.DOMove(pos, bulletSpeed).SetEase(Ease.Linear).SetSpeedBased().OnComplete(() => { Despawn(); });
            if (isRotate)
                transform.DORotate(Vector3.forward * 100000, rotateSpeed, RotateMode.WorldAxisAdd).SetSpeedBased().SetEase(Ease.Linear);
        }
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Player>())
            {
                Hit();
            }
            else
            {
                ImpactObstacle();
            }
        }
        protected virtual void Hit()
        {
            PlayFx();
            Player.Instance.TakeDamage(bulletDamage, Owner);
            OnHitPlayer?.Invoke(this);
            if(!piecring)
                Despawn();
        }
        protected virtual void PlayFx()
        {
            if (hitFx != null)
            {
                Helper.SpawnEffect(hitFx, transform.position, null, fxScale * parentScale);
            }
        }
        public virtual void OnMaxDistance()
        {
            OnMaxDistanceReach?.Invoke(this);
            Despawn();
        }
        public virtual void ImpactObstacle()
        {
            if (!piecring)
                Despawn();
        }
    }
}

