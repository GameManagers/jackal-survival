using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Unit;
using WE.Utils;
using DG.Tweening;
using WE.Support;

namespace WE.AI
{
    public class EnemyMover : MonoBehaviour
    {

        EnemySpriteController sprController => Owner.enemySpriteController;
        public Rigidbody2D rb;
        public float MoveSpeed => Owner.MoveSpeed;
        public Transform Target => Owner.Target;
        protected float mass => Owner.Mass;

        public System.Action<Enemy> OnChaseToPosDone;

        protected bool chasing;
        protected Vector3 chasingPos;
        protected float chaseSpeed;
        protected Enemy Owner;
        protected bool isInited;
        public bool movingOnTween;
        Tween pushBackTween;
        public virtual void Init(Enemy e)
        {
            movingOnTween = false;
            pushBackTween = null;
            isInited = true;
            Owner = e;
        }
        public virtual void ChaseTo(Vector3 pos, float _chaseSpeed)
        {
            chaseSpeed = _chaseSpeed;
            chasing = true;
            chasingPos = pos;
        }
        public virtual void OnUpdate(float t)
        {
            sprController.EnableSprite(Helper.IsInScreen(transform.position));
            if (!this.isActiveAndEnabled || !isInited || movingOnTween)
                return;
            UpdateMove(t);
            if (Helper.CheckSpawn(this.gameObject))
            {
                chasing = false;
            }
        }
        protected virtual void UpdateMove(float t)
        {
            if (!Player.Instance.IsAlive)
            {
                return;
            }
            Vector2 dir = (Target.position - transform.position).normalized;
            if (chasing)
            {
                CheckTarget();
                dir = (chasingPos - transform.position).normalized;
            }
            sprController.FlipX(dir.x < 0);
            Vector2 moveVector = dir * MoveSpeed * t;
            transform.position += (Vector3)moveVector;

        }
        public virtual void OnTakeHit(float pushBackForce, Transform target = null)
        {
            if (!this.isActiveAndEnabled)
                return;
            if (target == null)
                target = Player.Instance.transform;
            if (!movingOnTween)
            {
                float valuePush = pushBackForce / Owner.MaxHP;
                float timePush = valuePush / mass * 0.1f;
                if(timePush > 0.01f)
                {
                    if (pushBackTween != null)
                        pushBackTween.Kill(true);
                    pushBackTween = transform.DOMove(transform.position + (transform.position - target.position).normalized * valuePush / mass * 0.5f, timePush).SetEase(Ease.OutCirc);
                }
            }

            //rb.AddForce((transform.position - target.position).normalized * pushBackForce, ForceMode2D.Impulse);
        }
        public virtual void CheckTarget()
        {
            if (Vector3.SqrMagnitude(transform.position - chasingPos) < 0.5f)
            {
                ChaseDone();
            }
        }
        public virtual void ChaseDone()
        {
            chasing = false;
            OnChaseToPosDone?.Invoke(Owner);
        }
        protected virtual void OnDisable()
        {
            isInited = false;
        }
    }
}

