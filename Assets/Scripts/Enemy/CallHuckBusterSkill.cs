using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Effect;
using WE.Manager;
using WE.Support;
using WE.Unit;
using DG.Tweening;

namespace WE.SkillEnemy
{
    public class CallHuckBusterSkill : EnemyAttackSkill
    {
        public AnimationEffect fxExplose1;
        public Enemy huckBuster;
        float timeDrop = 0.5f;
        public AnimationEffect fxExplose2;
        protected override void OnInit()
        {
            Owner.OnEnemyDie += OnOwnerDie;
            base.OnInit();
        }
        public override void ExcuteSkill()
        {
            Enemy e = Helper.SpawnEmptyEnemy(huckBuster, transform.position + Vector3.up* ResolutionManager.Instance.ScreenHigh);
            e.Init(Owner.IsBoss, Owner.hpMul, Owner.dmgMul, Owner.expMul, Owner.weightMul, Owner.ParentScale);
            Helper.SpawnEffect(fxExplose1, transform.position, null, parentScale);
            e.mover.movingOnTween = true;
            e.transform.DOMove(transform.position, timeDrop).SetEase(Ease.Linear).OnComplete(() => {
                e.mover.movingOnTween = false;
                Helper.SpawnEffect(fxExplose2, transform.position, null, parentScale);
            });
        }
        public void OnOwnerDie(Enemy a)
        {
            Owner.OnEnemyDie -= OnOwnerDie;
            ExcuteSkill();
        }
        protected override void OnDisable()
        {
            Owner.OnEnemyDie -= OnOwnerDie;
            base.OnDisable();
        }
    }
}

