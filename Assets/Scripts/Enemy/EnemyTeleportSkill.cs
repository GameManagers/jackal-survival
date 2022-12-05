using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using WE.Unit;
using WE.Effect;
using WE.Support;
using DG.Tweening;
namespace WE.SkillEnemy
{
    public class EnemyTeleportSkill : EnemyAttackSkill
    {
        public float cooldown = 3;
        public float DistanceTele = 2;
        public AnimationEffect smokeFx;
        bool canExcute;
        public override void ExcuteSkill()
        {
            if (canExcute)
            {
                Vector3 pos = (Vector3)Random.insideUnitCircle.normalized * parentScale * DistanceTele;
                Helper.SpawnEffect(smokeFx, transform.position, null, parentScale);
                Helper.SpawnEffect(smokeFx, transform.position + pos, null, parentScale);
                Owner.transform.DOKill();
                Owner.transform.DOMove(Owner.transform.position + pos, 0);
            }
        }

        protected override void OnInit()
        {
            base.OnInit();
            Owner.OnTakeDamage += OnOwnerTakeHit;
            canExcute = true;
        }
        public virtual void OnOwnerTakeHit(bool a , bool b)
        {
            if (canExcute && Owner.IsAlive)
            {
                ExcuteSkill();
                canExcute = false;
                disposables = new CompositeDisposable();
                Observable.Timer(System.TimeSpan.FromSeconds(cooldown)).Subscribe(_ => canExcute = true).AddTo(disposables);
            }
        }
        protected override void OnDisable()
        {
            Owner.OnTakeDamage -= OnOwnerTakeHit;
            base.OnDisable();
        }
    }
}

