using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Effect;
using WE.Support;
using WE.Unit;

namespace WE.SkillEnemy
{
    public class KingSpawnSkill : EnemyAttackSkill
    {
        public Enemy enemyToSpawn;
        public int numberSpawn = 6;
        public AnimationEffect spawnFx;
        public float distanceToSpawn = 2;
        [Range(0, 1)]
        public float percentHpToSpawn = 0.3f;

        bool excuted;
        protected override void OnInit()
        {
            excuted = false;
            Owner.OnTakeDamage += OnOnwerTakeDamage;
            base.OnInit();
        }
        public override void ExcuteSkill()
        {
            excuted = true;
            float angle = Random.Range(0, 360);
            float angleStep = 360 / numberSpawn;
            for (int i = 0; i < numberSpawn; i++)
            {
                float _angle = angle + angleStep * i;
                Vector3 pos = Helper.GetPositionFromAngle(transform.position, _angle, distanceToSpawn * parentScale);
                Enemy e = Helper.SpawnEmptyEnemy(enemyToSpawn, pos );
                e.Init(false, Owner.hpMul, Owner.dmgMul, Owner.expMul, Owner.weightMul, Owner.ParentScale);
                e.enemySpriteController.spr.material = Owner.enemySpriteController.spr.material;
                Helper.SpawnEffect(spawnFx, pos, null, parentScale);
            }
        }
        protected override void OnDisable()
        {
            Owner.OnTakeDamage -= OnOnwerTakeDamage;
            base.OnDisable();
        }
        public virtual void OnOnwerTakeDamage(bool a, bool b)
        {

            if (!excuted && Owner.CurrentHp / Owner.MaxHP < percentHpToSpawn)
            {
                ExcuteSkill();
            }
        }

    }
}

