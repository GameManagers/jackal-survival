using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Manager;
using WE.Unit;
using UniRx;
namespace WE.SkillEnemy
{
    public abstract class EnemyAttackSkill : MonoBehaviour
    {
        public EnemyBullet bulletPrefabs;
        public CircleCollider2D enemyRange;
        public float detectRange;
        protected float damage;
        protected float bulletSpeed;
        protected float parentScale;
        protected Enemy Owner;
        protected CompositeDisposable disposables;

        public virtual void Init(Enemy _owner ,float _damage, float _bulletSpeed, float _parentScale)
        {
            Owner = _owner;
            this.damage = _damage;
            this.bulletSpeed = _bulletSpeed;
            this.parentScale = _parentScale;
            if(enemyRange != null)
                enemyRange.radius = detectRange/2;
            OnInit();
        }
        protected virtual void OnInit()
        {

        }
        public virtual void OnUpdate(float t)
        {

        }
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            OnTargetInRange();
        }
        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            OnTargetOutRange();
        }
        public virtual void OnTargetInRange()
        {

        }
        public virtual void OnTargetOutRange()
        {

        }
        public abstract void ExcuteSkill();
        protected virtual void OnDisable()
        {
            if (disposables != null)
            {
                disposables.Dispose();
            }
            StopAllCoroutines();
        }
    }
}

