using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using WE.Unit;
using WE.Data;
using WE.Manager;
using WE.Support;
using UniRx;
using WE.Bullet;

namespace WE.Skill
{
    public abstract class ActiveSkill : MonoBehaviour
    {
        public PlayerBullet bulletPrefabs;
        public EActiveSkill SkillType;
        public bool IsEvoleSkill = false;
        [ShowIf("IsEvoleSkill", true)]
        public EEvoleSkill EvoleSkillType;
        public float Maxdistance = 0;
        ActiveSkillConfig baseConfig => SkillController.Instance.dataSkill.ActiveSkillConfigData[SkillType];
        ActiveSkillConfig evoleConfig => SkillController.Instance.dataSkill.EvoleSkillConfigData[EvoleSkillType];

        ActiveSkillConfig useConfig;
        Dictionary<int, string> LevelUpConfig => SkillController.Instance.dataSkill.ActiveSkillLevelUpConfig[SkillType];

        public int CurrentLevel => currentLevel;
        [SerializeField, ReadOnly]
        protected int currentLevel = 1;

        public int BaseBulletNumb => baseBulletNumb;
        protected int baseBulletNumb;
        public float BaseCoolDonw => baseCooldown;
        protected float baseCooldown;
        public float BaseDamage => baseDamage;
        protected float baseDamage;
        public float BaseBulletSpeed => baseBulletSpeed;
        protected float baseBulletSpeed;
        public float BaseAreaScale => baseAreaScale;
        protected float baseAreaScale;
        public float BaseDuration => baseDuration;
        protected float baseDuration;
        public float BaseHitPerSec => baseHitPerSec;
        protected float baseHitPerSec;
        public int BaseHitEffect => baseHitEffect;
        protected int baseHitEffect;
        public float PushBackForce => pushBackForce;
        protected float pushBackForce;
        public int MaxLevel => useConfig.MaxLevel;
        public int Priority => useConfig.Priority;


        protected Transform gunTransform;
        protected Transform firePos;
        protected bool Initilized;
        protected CompositeDisposable disposables;
        protected CompositeDisposable miniDisposables;

        public virtual void Init()
        {
            Initilized = true;
            disposables = new CompositeDisposable();
            currentLevel = 1;
            gameObject.SetActive(true);
            SkillController.Instance.OnAddUpgrade += OnAddUpgrade;
            gunTransform = CarController.Instance.CurrentVisual.gunAnim.transform;
            firePos = CarController.Instance.CurrentVisual.firePosTransform;
            CalculateUpgrade();
            StartAction();
        }
        public virtual void AddLevel()
        {
            currentLevel++;
            CalculateUpgrade();
        }
        protected virtual void CalculateUpgrade()
        {
            useConfig = new ActiveSkillConfig();
            if (IsEvoleSkill)
                evoleConfig.CoppyTo(useConfig);
            else
                baseConfig.CoppyTo(useConfig);
            if (currentLevel > 1)
            {
                for (int i = 2; i <= currentLevel; i++)
                {
                    string _data = LevelUpConfig[i];
                    Dictionary<string, string> data = Helper.GetSubStats(_data);
                    foreach (var item in data)
                    {
                        AddUpgrade(item.Key, item.Value);
                    }
                }
            }
            OnAddUpgrade();
        }
        protected virtual void AddUpgrade(string key, string _value)
        {
            ELevelUpEffect levelUp = ELevelUpEffect.Add_Projectile;
            Helper.TryToEnum(key, out levelUp);
            float value = Helper.ParseFloat(_value);
            switch (levelUp)
            {
                case ELevelUpEffect.Add_Projectile:
                    useConfig.BaseBulletNumb += (int)value;
                    break;
                case ELevelUpEffect.Reduce_Cooldown:
                    useConfig.BaseCooldown -= value;
                    break;
                case ELevelUpEffect.Increase_Damage:
                    useConfig.BaseDamage += value;
                    break;
                case ELevelUpEffect.Increase_Projectile_Speed:
                    useConfig.BaseBulletSpeed += value;
                    break;
                case ELevelUpEffect.Increase_Effect_Radius:
                    useConfig.BaseAreaScale += value;
                    break;
                case ELevelUpEffect.Increase_Effect_Duration:
                    useConfig.BaseDuration += value;
                    break;
                case ELevelUpEffect.Increase_Hit_Per_Sec:
                    useConfig.BaseHitPerSec += value;
                    break;
                case ELevelUpEffect.Increase_Number_Of_Effect:
                    useConfig.BaseHitEffect += (int)value;
                    break;
                case ELevelUpEffect.Increase_Push_Back_Force:
                    useConfig.PushBackForce += value;
                    break;
                default:
                    break;
            }
        }
        public virtual void OnAddUpgrade()
        {
            baseBulletNumb = useConfig.BaseBulletNumb + Player.Instance.ProjectileAdd;
            baseCooldown = useConfig.BaseCooldown * (100 - Player.Instance.CooldownReduction) / 100;
            baseDamage = Player.Instance.AttackDamage * (useConfig.BaseDamage) / 100;
            baseBulletSpeed = useConfig.BaseBulletSpeed * (100 + Player.Instance.BulletSpeedIncrease) / 100;
            baseAreaScale = useConfig.BaseAreaScale * (100 + Player.Instance.AreaEffectIncrease) / 100;
            baseDuration = useConfig.BaseDuration * (100 + Player.Instance.EffectDurationIncrease) / 100;
            baseHitPerSec = useConfig.BaseHitPerSec * (100 + Player.Instance.CooldownReduction) / 100
                ;
            baseHitEffect = useConfig.BaseHitEffect + Player.Instance.ProjectileAdd
                ;
            pushBackForce = useConfig.PushBackForce * (100 + Player.Instance.PushBackForceIncrease) / 100;
        }
        public virtual void Dispose()
        {
            Initilized = false;
            Stop();
            SkillController.Instance.OnAddUpgrade -= OnAddUpgrade;
            //gameObject.SetActive(false);
        }
        public virtual void StartAction()
        {
            ExcuteSkill();
            if (baseCooldown > 0)
            {
                disposables = new CompositeDisposable();
                Observable.Timer(System.TimeSpan.FromSeconds(BaseCoolDonw)).Subscribe(_ => OnTimer()).AddTo(disposables);
            }
        }
        protected virtual void OnTimer()
        {
            ExcuteSkill();
            disposables = new CompositeDisposable();
            Observable.Timer(System.TimeSpan.FromSeconds(BaseCoolDonw)).Subscribe(_ => OnTimer()).AddTo(disposables);
        }
        protected abstract void ExcuteSkill();
        protected virtual void StopMiniObserve()
        {

            if (miniDisposables != null)
                miniDisposables.Dispose();
        }
        public virtual void Stop()
        {
            StopMiniObserve();
            StopAllCoroutines();
            if (disposables != null)
                disposables.Dispose();
        }
        protected virtual Vector3 GetTarget()
        {
            return Player.Instance.Target;
        }
        public virtual void InitBullet(PlayerBullet bullet)
        {
            float interval = 0;
            if (BaseHitPerSec > 0)
                interval = 1 / BaseHitPerSec;
            bullet.InitBullet(BaseDamage, PushBackForce, BaseBulletSpeed, Maxdistance * (1 + Player.Instance.AreaEffectIncrease / 100), BaseAreaScale, BaseDuration, interval);
        }
    }

}

