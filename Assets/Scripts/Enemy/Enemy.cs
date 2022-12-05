using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.AI;
using WE.Pooling;
using WE.Manager;
using WE.Support;
using WE.SkillEnemy;
using WE.Utils;
using UniRx;
using Sirenix.OdinInspector;
using TigerForge;
using System;
using WE.Effect;

namespace WE.Unit
{
    [Serializable]
    public class EnemyStats
    {
        public float Hp;
        public float Damage;
        public float MoveSpeed;
        public int ExpDrop;
        public int ChanceToDrop;
        public float weight;
        public SkillStats[] Skills;

        [Serializable]
        public class SkillStats
        {
            public float skillDamage;
            public float skillBulletSpeed;
        }
    }
    public class Enemy : PoolingObject
    {
        public string EnemyId => poolingKey;
        [GUIColor(1, 1, 0)]
        [FoldoutGroup("Base Setup")]
        public EnemySpriteController enemySpriteController;
        [GUIColor(1, 1, 0)]
        [FoldoutGroup("Base Setup")]
        public EnemyMover mover;
        [GUIColor(1, 1, 0)]
        [FoldoutGroup("Base Setup")]
        public EnemyImpactZone impactZone;
        [GUIColor(1, 1, 0)]
        [FoldoutGroup("Base Setup")]
        public float impactInterval;
        [FoldoutGroup("Base Setup")]
        public bool dontDropOnDie = false;



        [GUIColor(0, 1, 0)]
        [FoldoutGroup("Stats Setup")]
        public EnemyStats enemyStats;
        [GUIColor(0, 1, 0)]
        [FoldoutGroup("Stats Setup")]
        public EnemyAttackSkill[] skills;


        [GUIColor(1, 0, 0)]
        [FoldoutGroup("Visualize Setup")]
        public Sprite[] spriteSet;
        [GUIColor(1, 0, 0)]
        [FoldoutGroup("Visualize Setup")]
        public float animFrameRate;
        [GUIColor(1, 0, 0)]
        [FoldoutGroup("Visualize Setup")]
        public Sprite spriteFlash;
        [GUIColor(1, 0, 0)]
        [FoldoutGroup("Visualize Setup")]
        public float flashTime;

        [GUIColor(0, 0, 0)]
        [FoldoutGroup("Auto Set Variable")]
        public Transform Target;

        public System.Action<Vector3, float, bool> OnReviceDamage;
        public System.Action<bool, bool> OnTakeDamage;
        public System.Action<Enemy> OnEnemyDie;
        public virtual float MaxHP
        {
            get => maxHp;
        }
        public virtual float CurrentHp
        {
            get => currentHp;
        }
        public virtual float MoveSpeed
        {
            get => moveSpeed;
        }
        public virtual int ExpDrop
        {
            get => expDrop;
        }
        public virtual bool CanTakeDamage
        {
            get => canTakeDamage;
        }
        public virtual bool IsAlive
        {
            get => isAlive;
        }
        public virtual float Mass
        {
            get => mass;
        }
        public virtual float ParentScale
        {
            get => parentScale;
        }
        public bool IsBoss
        {
            get => isBoss;
        }
        public int ChanceToDropExp
        {
            get => chanceToDropExp;
        }
        protected float maxHp;
        protected float currentHp;
        protected float moveSpeed;
        protected float mass;
        protected int expDrop;
        protected float impactDamage;
        protected bool isAlive;
        protected bool canTakeDamage = false;
        protected bool cachedFrame = true;
        protected bool isBoss;
        protected float parentScale;
        protected int chanceToDropExp;
        [HideInInspector]
        public float hpMul, dmgMul, expMul, weightMul;
        //protected virtual void OnEnable()
        //{
        //    Init();
        //    //Observable.Timer(System.TimeSpan.FromSeconds(Random.Range(7, 15))).Subscribe(_ => Helper.Despawn(this)).AddTo(gameObject);
        //    //TigerForge.EventManager.EmitEvent(Constant.ON_ENEMY_SPAWNED, Random.Range(7, 15), this);
        //}
        public virtual void Init(bool _isBoss = false, float _hpMul = 1, float _dmgMul = 1, float _expMul = 1, float _weightMulti = 1, float _scaleEnemy = 1)
        {
            Target = Player.Instance.transform;
            isBoss = _isBoss;
            hpMul = _hpMul;
            dmgMul = _dmgMul;
            expMul = _expMul;
            weightMul = _weightMulti;

            InitStats(GameplayManager.Instance.GetMultiple(), _isBoss, _hpMul, _dmgMul, _expMul, _weightMulti, _scaleEnemy);

            enemySpriteController.Init(this);
            //if (_isBoss)
            //{
            //    enemySpriteController.spr.material = EnemySpawner.Instance.bossMaterial;
            //}
            //else
            //{
            //    enemySpriteController.spr.material = EnemySpawner.Instance.defautMaterial;
            //}
            cachedFrame = true;
            isAlive = true;

            OnEnemyDie += GameplayManager.Instance.OnEnemyDie;
            OnReviceDamage += GameplayManager.Instance.OnEnemyHit;
        }
        public virtual void InitStats(float multi, bool _isBoss = false, float _hpMul = 1, float _dmgMul = 1, float expMul = 1, float weightMulti = 1, float _scaleEnemy = 1)
        {
            if (enemyStats == null)
            {
                DebugCustom.LogError("Null Enemy Stats");
                return;
            }
            maxHp = enemyStats.Hp * multi * _hpMul;
            currentHp = maxHp;
            parentScale = _scaleEnemy;
            moveSpeed = enemyStats.MoveSpeed;
            impactDamage = enemyStats.Damage * multi;
            mass = enemyStats.weight * weightMulti;
            expDrop = (int)(enemyStats.ExpDrop * expMul);
            chanceToDropExp = enemyStats.ChanceToDrop;
            transform.localScale = Vector3.one * parentScale;
            if (impactZone != null)
            {
                impactZone.SetOwner(this, impactDamage, impactInterval);
            }
            if (mover != null)
            {
                mover.Init(this);
            }
            if (skills != null)
            {
                if (skills.Length != enemyStats.Skills.Length)
                {
                    DebugCustom.LogError("Enemy:" + name + "Skill Lenght Invalid!");
                    return;
                }
                for (int i = 0; i < skills.Length; i++)
                {
                    skills[i].Init(this, enemyStats.Skills[i].skillDamage * multi * _dmgMul, enemyStats.Skills[i].skillBulletSpeed, parentScale);
                }
            }
        }
        public virtual void TakeDamage(float value, float pushBackForce, Transform target = null, bool shock = false, bool showHit = true)
        {
            if (!IsAlive)
            {
                return;
            }
            float critRate = Player.Instance.CritRate;
            bool isCrit = Helper.Random_Conditional(100 - critRate, critRate);
            if (isCrit)
                value *= Constant.CRIT_MULTIPLE / 100;
            else
            {
                if (Helper.IsLuckApply())
                {
                    isCrit = Helper.Random_Conditional(100 - critRate, critRate);
                }
            }
            currentHp -= value;
            OnReviceDamage?.Invoke(transform.position, value, isCrit);
            if (currentHp > maxHp)
            {
                currentHp = maxHp;
            }
            if (currentHp < 0)
            {
                Die();
            }
            else
            {
                mover.OnTakeHit(pushBackForce * value, target);
            }
            OnTakeDamage?.Invoke(shock, showHit);
        }
        public void OnUpdate(float t)
        {
            mover.OnUpdate(t);
            impactZone.OnUpdate(t);
            if (skills!=null)
            {
                for (int i = 0; i < skills.Length; i++)
                {
                    skills[i].OnUpdate(t);
                }
            }
        }
        public virtual void Die()
        {
            isAlive = false;
            if (isBoss)
                GameplayManager.Instance.BossDie(this);
            AnimationEffect fx = Helper.SpawnEffect(ObjectPooler.Instance.fxEnemyDie, transform.position, null, parentScale);
            fx.SetFlipX(enemySpriteController.spr.flipX);
            OnEnemyDie?.Invoke(this);
            if (GameplayManager.Instance.CurrentGameplayType == GameType.Tutorial)
            {
                if (isBoss)
                    EventManager.EmitEvent(Constant.TUT_ON_LEVEL_TUT_END);
                else
                    EventManager.EmitEvent(Constant.TUT_ON_FIRST_ENEMY_DIE);
            }
            Despawn();
        }
        public override void Despawn()
        {
            EnemySpawner.Instance.listActiveEnemy.Remove(this);
            OnEnemyDie -= GameplayManager.Instance.OnEnemyDie;
            OnReviceDamage -= GameplayManager.Instance.OnEnemyHit;
            base.Despawn();
        }
    }
}
