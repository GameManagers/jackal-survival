using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Data;
using WE.Manager;
using CodeStage.AntiCheat.ObscuredTypes;
using WE.Utils;
using WE.Support;
using WE.Pooling;
using System;

namespace WE.Unit
{
    public class PlayerStats
    {

        public UnitBaseAttribute AttackAttribute;
        public UnitBaseAttribute HpAttribute;
        public UnitBaseAttribute MovespeedAttribute;
        public UnitBaseAttribute Reward_Increase;

        public UnitSubAttribute Hp_Recovery_Increase;
        public UnitSubAttribute Cooldown_Increase;
        public UnitSubAttribute Bullet_Speed_Increase;
        public UnitSubAttribute Effect_Area_Increase;
        public UnitSubAttribute Effect_Duration_Increase;
        public UnitSubAttribute Push_Back_Force_Increase;
        public UnitSubAttribute Crit_Rate_Increase;
        public UnitSubAttribute Projectile_Number_Add;
        public UnitSubAttribute Item_Absorb_Range_Increase;
        public UnitSubAttribute Luck_Increase;
        public UnitSubAttribute Exp_Increase;
        public UnitSubAttribute Damage_Reviced_Reduction_Increase;
        public UnitSubAttribute Revial;

        Player player => Player.Instance;
        DataPlayer playerData => Player.Instance.PLayerData;

        public Dictionary<EPassiveSkill, int> supportSkillUpgradeDict;


        public void SetCar(EVehicleType vehicleType)
        {
            AttackAttribute = new UnitBaseAttribute();
            HpAttribute = new UnitBaseAttribute();
            MovespeedAttribute = new UnitBaseAttribute();
            Reward_Increase = new UnitBaseAttribute();

            Hp_Recovery_Increase = new UnitSubAttribute();
            Cooldown_Increase = new UnitSubAttribute();
            Bullet_Speed_Increase = new UnitSubAttribute();
            Effect_Area_Increase = new UnitSubAttribute();
            Effect_Duration_Increase = new UnitSubAttribute();
            Push_Back_Force_Increase = new UnitSubAttribute();
            Crit_Rate_Increase = new UnitSubAttribute();
            Projectile_Number_Add = new UnitSubAttribute();
            Item_Absorb_Range_Increase = new UnitSubAttribute();
            Luck_Increase = new UnitSubAttribute();
            Exp_Increase = new UnitSubAttribute();
            Damage_Reviced_Reduction_Increase = new UnitSubAttribute();
            Revial = new UnitSubAttribute();
            VehicleConfig config = CarController.Instance.dataVehicle.VehicleDict[playerData.currentVehicle];
            HpAttribute.SetValueCount(config.GetCarHp());
            AttackAttribute.SetValueCount(config.GetCarDamage());
            MovespeedAttribute.SetValueCount(config.BaseMoveSpeed);
            HpAttribute.SetValuePercentGlobal(DataManager.Instance.dataGlobalUpgrade.GUValueIncreasePerLevel * playerData.GlobalHpIncreaseLevel);
            AttackAttribute.SetValuePercentGlobal(DataManager.Instance.dataGlobalUpgrade.GUValueIncreasePerLevel * playerData.GlobalAttackIncreaseLevel);
            Reward_Increase.SetValueCount(1);
            Reward_Increase.AddValuePercentGlobal(DataManager.Instance.dataGlobalUpgrade.GUValueIncreasePerLevel * playerData.GlobalRewardIncreaseLevel);
            Player.Instance.OnCarChange?.Invoke(playerData.currentVehicle);

        }
        public void OnAddGlobalUpgrade()
        {
            HpAttribute.SetValuePercentGlobal(DataManager.Instance.dataGlobalUpgrade.GUValueIncreasePerLevel * playerData.GlobalHpIncreaseLevel);
            AttackAttribute.SetValuePercentGlobal(DataManager.Instance.dataGlobalUpgrade.GUValueIncreasePerLevel * playerData.GlobalAttackIncreaseLevel);
            Reward_Increase.SetValueCount(1);
            Reward_Increase.AddValuePercentGlobal(DataManager.Instance.dataGlobalUpgrade.GUValueIncreasePerLevel * playerData.GlobalRewardIncreaseLevel);
            Helper.SpawnEffect(ObjectPooler.Instance.fxUiUpgare, Player.Instance.transform.position, Player.Instance.transform);
        }
        //public void AddUpgrade(VehicleUpgrade upgarde)
        //{
        //    float val = upgarde.UpgradeValue;
        //    switch (upgarde.UpgaradeType)
        //    {
        //        case EVehicleUpgrade.Hp_Increase:
        //            HpAttribute.AddValueCount(val);
        //            break;
        //        case EVehicleUpgrade.Attack_Increase:
        //            AttackAttribute.AddValueCount(val);
        //            break;
        //        default:
        //            break;
        //    }
        //}

        public void InitSkill()
        {
            supportSkillUpgradeDict = new Dictionary<EPassiveSkill, int>();

        }
        public void AddPassive(EPassiveSkill passiveSkill)
        {
            if (supportSkillUpgradeDict.ContainsKey(passiveSkill))
            {
                supportSkillUpgradeDict[passiveSkill]++;
            }
            else
            {
                supportSkillUpgradeDict.Add(passiveSkill, 1);
            }
            float value = SkillController.Instance.dataSkill.PassiveSkillData[passiveSkill].ValuePerLevel;
            switch (passiveSkill)
            {
                case EPassiveSkill.Cooldown_Increase:
                    Cooldown_Increase.AddValue(value);
                    break;
                case EPassiveSkill.Bullet_Speed_Increase:
                    Bullet_Speed_Increase.AddValue(value);
                    break;
                case EPassiveSkill.Bullet_Damage_Increase:
                    AttackAttribute.AddValuePercentLocal(value);
                    break;
                case EPassiveSkill.Max_Hp_Increase:
                    player.SavePercentCurrentHp();
                    HpAttribute.AddValuePercentLocal(value);
                    player.UpdateMaxHp();
                    break;
                case EPassiveSkill.Exp_Increase:
                    Exp_Increase.AddValue(value);
                    break;
                case EPassiveSkill.Item_Absorb_Range_Increase:
                    Item_Absorb_Range_Increase.AddValue(value);
                    break;
                case EPassiveSkill.Effect_Area_Increase:
                    Effect_Area_Increase.AddValue(value);
                    break;
                case EPassiveSkill.Effect_Duration_Increase:
                    Effect_Duration_Increase.AddValue(value);
                    break;
                case EPassiveSkill.Coin_Value_Increase:
                    Reward_Increase.AddValuePercentLocal(value);
                    break;
                case EPassiveSkill.Hp_Recovery_Increase:
                    Hp_Recovery_Increase.AddValue(value);
                    break;
                case EPassiveSkill.Damage_Reviced_Reduction_Increase:
                    Damage_Reviced_Reduction_Increase.AddValue(value);
                    break;
                case EPassiveSkill.Luck_Increase:
                    Luck_Increase.AddValue(value);
                    break;
                case EPassiveSkill.Projectile_Number_Increase:
                    Projectile_Number_Add.AddValue(value);
                    break;
                case EPassiveSkill.Move_Speed_Increase:
                    MovespeedAttribute.AddValuePercentLocal(value);
                    break;
                case EPassiveSkill.Crit_Incease:
                    Crit_Rate_Increase.AddValue(value);
                    break;
                case EPassiveSkill.Push_Back_Force_Increase:
                    Push_Back_Force_Increase.AddValue(value);
                    break;
                case EPassiveSkill.Revival:
                    player.AddRevival();
                    break;
                default:
                    break;
            }
        }
        public void ClearRivival()
        {
            supportSkillUpgradeDict.Remove(EPassiveSkill.Revival);
            SkillController.Instance.passiveSkillList.Remove(EPassiveSkill.Revival.ToString());
            SkillController.Instance.dicStringCurrentSkill.Remove(EPassiveSkill.Revival.ToString());
        }
    }
    public class UnitBaseAttribute
    {
        public UnitBaseAttribute()
        {
            ValueCount = 0;
            ValuePercentGlobal = 0;
            ValuePercentLocal = 0;
        }
        public float Value { get => ValueCount * ValuePercent; }
        public float ValuePercent { get => (1 + ValuePercentGlobal/100) * (1 + ValuePercentLocal/100); }
        public ObscuredFloat ValueCount { get; private set; }
        public ObscuredFloat ValuePercentGlobal { get; private set; }
        public ObscuredFloat ValuePercentLocal { get; private set; }

        public void SetValueCount(float v)
        {
            ValueCount = v;
            DebugCustom.Log("get car hp", v, Player.Instance.MaxHp);
            if (ValueCount < 0)
            {
                ValueCount = 0;
            }
        }
        public void AddValueCount(float v)
        {
            ValueCount += v;
            if (ValueCount < 0)
            {
                ValueCount = 0;
            }
        }
        public void SetValuePercentGlobal(float v)
        {
            ValuePercentGlobal = v;
        }
        public void AddValuePercentGlobal(float v)
        {
            ValuePercentGlobal += v;
        }
        public void SetValuePercentLocal(float v)
        {
            ValuePercentLocal = v;
        }
        public void AddValuePercentLocal(float v)
        {
            ValuePercentLocal += v;
        }
    }
    public class UnitSubAttribute
    {
        public ObscuredInt Value { get; private set; }
        public void AddValue(float val)
        {
            Value += (int)val;
        }
    }
    public class DataPlayer
    {
        public int maxMapUnlock = 0;
        public int currentMap = 0;
        public int GlobalAttackIncreaseLevel = 0;
        public int GlobalHpIncreaseLevel = 0;
        public int GlobalRewardIncreaseLevel = 0;
        public int currentCoin = 0;
        public int totalEnemyKill = 0;
        public int totalCoinSpend = 0;
        public Dictionary<EVehicleType, int> vehicleUpgrades;
        public EVehicleType currentVehicle = EVehicleType.Machine_Gun_Vehicle;

        public bool vibrateSetting = true;
        public bool soundSetting = true;
        public bool musicSetting = true;
        public Dictionary<int, int> dataZoneRecord = new Dictionary<int, int>();

        public int endlesskey = 0;
        public int endlessRecordTime = 0;
        public int endlessRecordKill = 0;
        public Dictionary<EVehicleType, int> vehicleAds;
        public int noAdsAdsCount = 0;

        public DateTime timeCloseNoAds;

        public bool IsPlayTut = false;
        public void SetTutDone()
        {
            IsPlayTut = true;
            SaveData();
        }

        /**
         * Set no ads banner
         */

        public bool IsOnNoAds()
        {
            return true;
           // return timeCloseNoAds > DateTime.UtcNow;
        }
        public void SetNoAds()
        {
            timeCloseNoAds = DateTime.UtcNow + System.TimeSpan.FromMinutes(30);
            noAdsAdsCount = 0;
            SaveData();
        }
        public void AdsNoAdsCount()
        {
            noAdsAdsCount++;
            SaveData();
        }
        public int GetVehicleAds(EVehicleType type)
        {
            if (vehicleAds == null)
            {
                vehicleAds = new Dictionary<EVehicleType, int>();
            }
            if (vehicleAds.ContainsKey(type))
            {
                return vehicleAds[type];
            }
            else
            {
                vehicleAds.Add(type, 0);
                SaveData();
                return 0;
            }
        }
        public void UpdateAds(EVehicleType type)
        {
            if (vehicleAds == null)
            {
                vehicleAds = new Dictionary<EVehicleType, int>();
            }
            if (vehicleAds.ContainsKey(type))
            {
                vehicleAds[type]++;
                SaveData();
            }
            else
            {
                vehicleAds.Add(type, 1);
                SaveData();
            }
        }
        public void AddKey(int val)
        {
            endlesskey += val;
            SaveData();
        }
        public void UseKey()
        {
            endlesskey--;
            SaveData();
        }
        public void ToggleVibrateSetting()
        {
            vibrateSetting = !vibrateSetting;
            SaveData();
        }
        public void ToggleSoundSetting()
        {
            soundSetting = !soundSetting;
            SaveData();
        }
        public void ToggleMusicSetting()
        {
            musicSetting = !musicSetting;
            SaveData();
        }
        public void AddAttackUpgrade()
        {
            GlobalAttackIncreaseLevel++;
            Player.Instance.playerStats.OnAddGlobalUpgrade();
            SaveData();
        }
        public void AddHpUpgrade()
        {
            GlobalHpIncreaseLevel++;
            Player.Instance.playerStats.OnAddGlobalUpgrade();
            SaveData();
        }
        public void AddRewardUpgrade()
        {
            GlobalRewardIncreaseLevel++;
            Player.Instance.playerStats.OnAddGlobalUpgrade();
            SaveData();
        }
        public void UnlockVehicle(EVehicleType vehicleType)
        {
            if (vehicleUpgrades == null)
                vehicleUpgrades = new Dictionary<EVehicleType, int>();
            vehicleUpgrades.Add(vehicleType, 1);
            SaveData();
        }
        public void UpgradeVehicle(EVehicleType vehicleType)
        {
            vehicleUpgrades[vehicleType]++;
            SaveData();
        }
        public void SelectVehicle(EVehicleType vehicleType)
        {
            currentVehicle = vehicleType;
            SaveData();
        }
        public void SelectMap(int id)
        {
            currentMap = id;
            SaveData();
        }
        public void Winlevel()
        {
            if (currentMap == maxMapUnlock)
            {
                UnlockLevel();
            }
        }
        public void UnlockLevel()
        {
            maxMapUnlock++;
            currentMap = maxMapUnlock;
            //if (currentMap > Constant.MAX_ZONE)
            //{
            //    currentMap = Constant.MAX_ZONE;
            //}

            if (dataZoneRecord == null)
            {
                dataZoneRecord = new Dictionary<int, int>();
            }
            dataZoneRecord.Add(maxMapUnlock, 0);
            SaveData();
        }
        public void AddCoin(int val)
        {
            currentCoin += val;
            if (currentCoin < 0)
                currentCoin = 0;
            if (val < 0)
            {
                totalCoinSpend -= val;
            }
            SaveData();
        }
        public void AddEnemyKill(int val)
        {
            totalEnemyKill += val;
            SaveData();
        }
        public void AddRecord(int sec)
        {
            dataZoneRecord[Player.Instance.CurrentMap] = sec;
        }
        public int GetRecord(int level)
        {
            return dataZoneRecord[level];
        }
        //public static void LoadData()
        //{
        //    if (CPlayerPrefs.HasKey(Constant.PLAYER_DATA))
        //    {
        //        Player.Instance.PLayerData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataPlayer>(CPlayerPrefs.GetString(Constant.PLAYER_DATA));
        //    }
        //}
        public void SetEndlessRecord(int timer, int killCount)
        {
            if (endlessRecordKill < killCount)
                endlessRecordKill = killCount;
            if (endlessRecordTime < timer)
                endlessRecordTime = timer;
        }
        public static void SaveData()
        {
            CPlayerPrefs.SetString(Constant.PLAYER_DATA, Newtonsoft.Json.JsonConvert.SerializeObject(Player.Instance.PLayerData));
        }
    }
}

