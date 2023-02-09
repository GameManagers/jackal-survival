using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Data;
using WE.Game;
using WE.GameAction.Tank;
using WE.Manager;
using WE.Tank;
using WE.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using WE.Support;
using WE.Pooling;

namespace WE.Unit
{
    public class Player : MonoBehaviour
    {
        public static Player Instance;
        private void Awake()
        {
            Instance = this;
        }
        
        public TankMovement tankMovement;
        public TankRange tankRange;
        public HpBarPlayer hpBar;
        public float baseAbsorbRange;
        public float CurrentHp => currentHp;
        #region DataPlayer and Attribute

        public PlayerStats playerStats;
        public float AttackDamage => playerStats.AttackAttribute.Value;
        public float MaxHp => playerStats.HpAttribute.Value;
        public float MoveSpeed => playerStats.MovespeedAttribute.Value;
        public float HpRecover => playerStats.Hp_Recovery_Increase.Value;
        public float CooldownReduction => playerStats.Cooldown_Increase.Value;
        public float BulletSpeedIncrease => playerStats.Bullet_Speed_Increase.Value;
        public float AreaEffectIncrease => playerStats.Effect_Area_Increase.Value;
        public float EffectDurationIncrease => playerStats.Effect_Duration_Increase.Value;
        public float PushBackForceIncrease => playerStats.Push_Back_Force_Increase.Value;
        public float CritRate => playerStats.Crit_Rate_Increase.Value;
        public int ProjectileAdd => (int)playerStats.Projectile_Number_Add.Value;
        public float ItemAbsorbRankIncrease => playerStats.Item_Absorb_Range_Increase.Value;
        public float Luck => playerStats.Luck_Increase.Value;
        public float ExpBonus => playerStats.Exp_Increase.Value;
        public float CoinBonus => playerStats.Reward_Increase.Value;
        public float DamageReviceReduction => playerStats.Damage_Reviced_Reduction_Increase.Value;
        public int TotalRevival => (int)playerStats.Revial.Value;

        public float AbsorbRange
        {
            get
            {
                if (GameplayManager.Instance.CurrentGameplayType != GameType.Tutorial)
                    return baseAbsorbRange * (1 + ItemAbsorbRankIncrease / 100);
                else return 4.5f;
            }
        }
        private DataPlayer playerData;
        public DataPlayer PLayerData => playerData;
        public int CurrentMap 
        { 
            get
            {
                if (GameplayManager.Instance.CurrentGameplayType == GameType.Endless && GameplayManager.Instance.State != GameState.MainUi)
                {
                    return 0;
                }
                else if (GameplayManager.Instance.CurrentGameplayType == GameType.Tutorial && GameplayManager.Instance.State != GameState.MainUi)
                {
                    return 0;
                }
                else if (GameplayManager.Instance.CurrentGameplayType == GameType.PVP && GameplayManager.Instance.State != GameState.MainUi)
                {
                    return Context.PVPCurrentMap;
                }
                else
                    return playerData.currentMap;
            }
        }
        public EVehicleType CurrentTank => currentTank;
        EVehicleType currentTank;
        public int currentCoin => playerData.currentCoin;

        public System.Action<EVehicleType> OnCarChange;
        public System.Action OnHpChange;

        float currentHp;
        bool IsInitialize;
        public bool IsImortal => isImortal;
        bool isImortal;
        float cachedPercentHp;
        //[SerializeField,ReadOnly]
        //Enemy target;
        //public Enemy Target => target;
        //public void NextTarget()
        //{
        //    if (target != null)
        //        target.OnEnemyDie -= NextTarget;
        //    target = tankRange.GetTarget();
        //    if (target != null)
        //        target.OnEnemyDie += NextTarget;
        //}
        public Vector3 Target;

        public int RevivalCount => revivalCount;
        int revivalCount = 0;
        int adsRevieCount = 0;
        public int AdsRevieCount => adsRevieCount;
        public bool IsGetRevieSkill => isGetRevieSkill;

        public bool IsAlive => !dieing;
        bool isGetRevieSkill;
        //public GameObject absorbRange;
        public GameObject graveStone;
        public GameObject shieldObj;
        public GameObject killZone;
        bool dieing;

        public GameObject cachedShieldObj;
        public int EndlessKeyCount => playerData.endlesskey;
        public void Init()
        {
            LoadData();
            CarController.Instance.Init();
            playerStats.SetCar(playerData.currentVehicle);
            playerStats.InitSkill();
            ObjectPooler.Instance.Init();
            MapController.Instance.Init();
            graveStone.SetActive(false);
            shieldObj = cachedShieldObj;
            shieldObj.SetActive(false);
            hpBar.Despawn();
            //TigerForge.EventManager.StartListening(Constant.GAME_TICK_EVENT, OnTick);
            currentHp = MaxHp;
            killZone.SetActive(false);
            IsInitialize = true;
        }
        public void LoadData()
        {
            playerData = new DataPlayer();
            playerStats = new PlayerStats();
            if (CPlayerPrefs.HasKey(Constant.PLAYER_DATA))
            {
                playerData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataPlayer>(CPlayerPrefs.GetString(Constant.PLAYER_DATA));
            }
            else
            {
                playerData.vehicleUpgrades = new Dictionary<EVehicleType, int>();
                playerData.UnlockLevel();
                playerData.UnlockVehicle(EVehicleType.Machine_Gun_Vehicle);
                playerData.SelectVehicle(EVehicleType.Machine_Gun_Vehicle);
            }
            currentTank = playerData.currentVehicle;
        }
        public void SaveData()
        {
            DataPlayer.SaveData();
        }
        #endregion

        public void UnlockCar(EVehicleType vehicleType)
        {
            playerData.UnlockVehicle(vehicleType);
            playerData.SelectVehicle(vehicleType);
            currentTank = vehicleType;
            playerStats.SetCar(vehicleType);
        }
        public void SelectCar(EVehicleType vehicleType)
        {
            playerData.SelectVehicle(vehicleType);
            currentTank = vehicleType;
            playerStats.SetCar(vehicleType);
        }
        public void UpgradeCar(EVehicleType vehicleType)
        {
            playerData.UpgradeVehicle(vehicleType);
            playerStats.SetCar(vehicleType);
        }

        public bool CanBuyItem(int cost)
        {
            return currentCoin >= cost;
        }
        public void AddCoin(int val)
        {
            playerData.AddCoin(val);
            TigerForge.EventManager.EmitEvent(Constant.ON_COINS_CHANGE);
        }

        public void ChangeName()
        {
            TigerForge.EventManager.EmitEvent(Constant.ON_CHANGE_NAME);
        }

        public void AddEndlessKey(int val)
        {
            playerData.AddKey(val);
        }
        public void ConsumeKey()
        {
            playerData.UseKey();
        }
        public void AddEnemyKill(int val)
        {
            playerData.AddEnemyKill(val);
        }
        public bool CanPreviousLevel()
        {
            return CurrentMap > 1;
        }
        public void PreviousLevel()
        {
            playerData.currentMap--;
            TigerForge.EventManager.EmitEvent(Constant.ON_CHANGE_ZONE);
            SaveData();
        }
        public bool CanNextLevel()
        {
            //return CurrentMap < playerData.maxMapUnlock && CurrentMap < Constant.MAX_ZONE;
            return CurrentMap < playerData.maxMapUnlock;
        }
        public void NextLevel()
        {
            playerData.currentMap++;
            TigerForge.EventManager.EmitEvent(Constant.ON_CHANGE_ZONE);
            SaveData();
        }
        public void AddGlobalUpgrade(GolobalUpgrade upgradeType)
        {

            SoundManager.Instance.PlaySoundFx(SoundManager.Instance.upgradeSfx);
            switch (upgradeType)
            {
                case GolobalUpgrade.Hp_Increase:
                    PLayerData.AddHpUpgrade();
                    break;
                case GolobalUpgrade.Attack_Increase:
                    PLayerData.AddAttackUpgrade();
                    break;
                case GolobalUpgrade.Reward_Increase:
                    PLayerData.AddRewardUpgrade();
                    break;
                default:
                    break;
            }
            TigerForge.EventManager.EmitEvent(Constant.ON_ADD_GLOBAL_UPGRADE);

        }
        public int GetGlobalUpgradeCost(GolobalUpgrade upgradeType)
        {
            switch (upgradeType)
            {
                case GolobalUpgrade.Hp_Increase:
                    return DataManager.Instance.dataGlobalUpgrade.GetGlobalUpgradeCost(playerData.GlobalHpIncreaseLevel);
                case GolobalUpgrade.Attack_Increase:
                    return DataManager.Instance.dataGlobalUpgrade.GetGlobalUpgradeCost(playerData.GlobalAttackIncreaseLevel);
                case GolobalUpgrade.Reward_Increase:
                    return DataManager.Instance.dataGlobalUpgrade.GetGlobalUpgradeCost(playerData.GlobalRewardIncreaseLevel);
                default:
                    return 0;
            }
        }
        public int GetGlobalUpgradeValue(GolobalUpgrade upgradeType)
        {
            switch (upgradeType)
            {
                case GolobalUpgrade.Hp_Increase:
                    return DataManager.Instance.dataGlobalUpgrade.GUValueIncreasePerLevel * playerData.GlobalHpIncreaseLevel;
                case GolobalUpgrade.Attack_Increase:
                    return DataManager.Instance.dataGlobalUpgrade.GUValueIncreasePerLevel * playerData.GlobalAttackIncreaseLevel;
                case GolobalUpgrade.Reward_Increase:
                    return DataManager.Instance.dataGlobalUpgrade.GUValueIncreasePerLevel * playerData.GlobalRewardIncreaseLevel;
                default:
                    return 0;
            }
        }
        public bool GetMusicSetting()
        {
            return playerData.musicSetting;
        }
        public bool GetSoundSetting()
        {
            return playerData.soundSetting;
        }
        public bool GetVibrateSetting()
        {
            return playerData.vibrateSetting;
        }
        public void ToggleSoundSetting()
        {
            playerData.ToggleSoundSetting();
            TigerForge.EventManager.EmitEvent(Constant.ON_SOUND_SETTING_CHANGE);
        }
        public void ToggVibrateSetting()
        {
            playerData.ToggleVibrateSetting();
            TigerForge.EventManager.EmitEvent(Constant.ON_SOUND_SETTING_CHANGE);
        }
        public void ToggleMusicSetting()
        {
            playerData.ToggleMusicSetting();
            TigerForge.EventManager.EmitEvent(Constant.ON_SOUND_SETTING_CHANGE);
            //SoundManager.Instance.OnToggleMusic();        
        }
        public void StartGame()
        {
            isGetRevieSkill = false;
            revivalCount = 0;
            adsRevieCount = 1;
            isImortal = false;
            dieing = false;
            currentHp = MaxHp;
            tankMovement.Init();
            hpBar.Init();
        }
        public void LevelEnd(bool win)
        {
            CarController.Instance.CurrentVisual.ResetVisual();
            if (win)
            {
                playerData.Winlevel();
            }
            tankMovement.Stop();
            SaveData();
            transform.position = Vector3.zero;
            Init();
        }
        public void AddRevival()
        {
            isGetRevieSkill = true;
            revivalCount ++;
        }
        public void SavePercentCurrentHp()
        {
            cachedPercentHp = currentHp / MaxHp;
        }
        public void UpdateMaxHp()
        {
            currentHp = MaxHp * cachedPercentHp;
            OnHpChange?.Invoke();
        }
        public void AutoRecovery()
        {
            RecoverHp(HpRecover);
        }
        public void RecoverHp(float value)
        {
            currentHp += MaxHp * value / 100;
            if (currentHp > MaxHp)
                currentHp = MaxHp;
            OnHpChange?.Invoke();
        }
        public void TakeDamage(float value, Enemy source)
        {
            if (isImortal || currentHp <0 || dieing)
                return;
            value *= (1 - playerStats.Damage_Reviced_Reduction_Increase.Value / 100);
            currentHp -= value;
            if (currentHp > MaxHp)
            {
                currentHp = MaxHp;
            }
            if (currentHp <= 0)
            {
                currentHp = 0;
                PlayerDie();
            }
            GameplayManager.Instance.Vibrate();
            OnHpChange?.Invoke();
        }
        public void SetShield(GameObject shield)
        {
            if(shieldObj != null)
                shieldObj.SetActive(false);
            shieldObj = shield;
        }
        public void SetImortal()
        {
            isImortal = true;
            if (shieldObj == null)
                shieldObj = cachedShieldObj;
            shieldObj.SetActive(true);
        }
        public void SetImortal(float t)
        {
            StartCoroutine(IEImortal(t));
        }
        IEnumerator IEImortal(float t)
        {
            SetImortal();
            yield return new WaitForSeconds(t);
            StopImortal();
        }
        public void StopImortal()
        {
            isImortal = false;
            if (shieldObj == null)
                shieldObj = cachedShieldObj;
            shieldObj.SetActive(false);
        }
        public void PlayerDie()
        {
            SoundManager.Instance.PlaySoundFx(SoundManager.Instance.dieSfx);
            dieing = true;
            CarController.Instance.CurrentVisual.HideVisual();
            tankMovement.Stop();
            graveStone.SetActive(true);
            SkillController.Instance.StopAction();
            if (GameplayManager.Instance.CurrentGameplayType != GameType.Tutorial)
            {
                TimerSystem.Instance.StopTimeScale(1, () => { CheckShowPopupDie(); });
            }
            else
            {
                GameplayManager.Instance.ShowPopupEndGame(false);
            }
        }
        void CheckShowPopupDie()
        {
            if (adsRevieCount + revivalCount > 0)
            {
                UIManager.Instance.ShowPopupDie();
            }
            else
            {
                GameplayManager.Instance.ShowPopupEndGame(false);
            }
        }
        public void Revival()
        {
            SoundManager.Instance.PlaySoundFx(SoundManager.Instance.reviveSfx);
            GameplayManager.Instance.ReviceItem(EItemInGame.Bomb);
            if (revivalCount > 0)
            {
                revivalCount--;
                if (revivalCount == 0)
                {
                    playerStats.ClearRivival();
                }
            }
            else
            {
                adsRevieCount--;
            }
            dieing = false;
            graveStone.SetActive(false);
            CarController.Instance.CurrentVisual.ShowVisual() ;
            tankMovement.Init();
            SetImortal(2);
            RecoverHp(50);
            SkillController.Instance.ReturnAction();
        }
        //public void OnTick()
        //{
        //    //NextTarget();
        //}
        public void GetTarget()
        {
            Enemy e = tankRange.GetTarget();
            if (e != null)
            {
                Target = e.transform.position;
            }
            else Target = Vector3.up * 10;
        }
        public void UseWeapon(float attackTime = 0)
        {
            GetTarget();
            CarController.Instance.UseWeapon(attackTime);
        }
        //public void OnAddPassive()
        //{
        //    absorbRange.transform.localScale = Vector3.one * (1 + ItemAbsorbRankIncrease / 100);
        //}
        public void HackLevel()
        {
            playerData.UnlockLevel();
            MapController.Instance.LoadMap();
            TigerForge.EventManager.EmitEvent(Constant.ON_CHANGE_ZONE);
        }
        //private void OnDisable()
        //{
        //    TigerForge.EventManager.StopListening(Constant.GAME_TICK_EVENT, OnTick);
        //}
        public void ToggleKillZone()
        {
            killZone.SetActive(!killZone.activeSelf);
        }
        public void ToggleImortal()
        {
            if (IsImortal)
            {
                StopImortal();
            }
            else
            {
                SetImortal();
            }
        }
        public void AddRecord(int sec)
        {
            playerData.AddRecord(sec);
        }
        public void AddEndlessRecord(int timer, int killCount)
        {
            playerData.SetEndlessRecord(timer, killCount);
        }
        public int GetRecordTimeEndless()
        {
            return playerData.endlessRecordTime;
        }
        public int GetRecordKillEndless()
        {
            return playerData.endlessRecordKill;
        }
        public int GetAdsCount(EVehicleType carType)
        {
            return playerData.GetVehicleAds(carType);
        }
        public void AddVehicleAds(EVehicleType type)
        {
            playerData.UpdateAds(type);
        }
        public int GetNoAdsCount()
        {
            return playerData.noAdsAdsCount;
        }
        public void AddNoAdsCount()
        {
            playerData.noAdsAdsCount++;
        }
        public void StartNoAds()
        {
            playerData.SetNoAds();
        }
        public System.DateTime GetNoadsDateTime()
        {
            return playerData.timeCloseNoAds;
        }
        public bool IsOnNoAds()
        {
            return playerData.IsOnNoAds();
        }
        public string ConvetNoAdsTime()
        {
            if (!IsOnNoAds())
                return string.Empty;
            System.TimeSpan sp = GetNoadsDateTime() - System.DateTime.UtcNow;
            return Helper.ConvertTimer((int)sp.TotalSeconds);
        }
        public bool IsPlayTut()
        {
            return playerData.IsPlayTut;
        }
        public void SetTutDone()
        {
            playerData.SetTutDone();
        }
#if UNITY_EDITOR
        [Button("Clear Data")]
        public void ClearData()
        {
            CPlayerPrefs.DeleteKey(Constant.PLAYER_DATA);
            EditorApplication.isPlaying = false;
        }
#endif
    }
}

