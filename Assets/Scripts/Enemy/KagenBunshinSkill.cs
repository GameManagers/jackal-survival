using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Effect;
using WE.Support;
using WE.Unit;

namespace WE.SkillEnemy
{
    public class KagenBunshinSkill : EnemyAttackSkill
    {
        public Enemy shadowPrefabs;
        public AnimationEffect fxSkill;
        public int shadowNumb = 3;
        protected override void OnInit()
        {
            base.OnInit();
            Owner.OnEnemyDie += OnOwnerDie;
        }
        public override void ExcuteSkill()
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 pos = transform.position + (Vector3)Random.insideUnitCircle * parentScale;
                Helper.SpawnEffect(fxSkill, pos, null);
                Enemy e = Helper.SpawnEmptyEnemy(shadowPrefabs, pos);
                e.Init(false, Owner.hpMul, Owner.dmgMul, Owner.expMul, Owner.weightMul, Owner.ParentScale);
                e.enemySpriteController.spr.material = Owner.enemySpriteController.spr.material;
            }
        }
        public void OnOwnerDie(Enemy e)
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

