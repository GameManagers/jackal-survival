using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Manager;
using Sirenix.OdinInspector;
using WE.Support;
using System.Linq;
using WE.Car;
using WE.Unit;

namespace WE.Data
{
    [CreateAssetMenu(fileName = "Data Vehicle", menuName = "WE/Data/Data Vehicle")]
    public class DataVehicle : SerializedScriptableObject
    {
        public Dictionary<EVehicleType, VehicleConfig> VehicleDict;
        public Dictionary<EVehicleType, CarVisualize> CarVisualizeDict;
#if UNITY_EDITOR
        #region Load Data
        [Button("Load Data Vehicle")]
        public void LoadDataInteractive()
        {
            VehicleDict = new Dictionary<EVehicleType, VehicleConfig>();
            CarVisualizeDict = new Dictionary<EVehicleType, CarVisualize>();
            string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTBO_e7NjDFNq1K56OO5JB19OFqI3VyA8vIW345_YL1QnanxaV0MNvCKJBcpkkWF9DziZOWzuQmXqTG/pub?gid=720108220&single=true&output=csv";
            System.Action<string> actionComplete = new System.Action<string>((string str) =>
            {
                var data = CSVReader.ReadCSV(str);
                string path1 = "Car/UI/";
                string path2 = "Car/Visualize/";
                for (int i = 2; i < data.Count; i++)
                {
                    var _data = data[i];
                    if (!string.IsNullOrEmpty(_data[0]))
                    {
                        VehicleConfig config = new VehicleConfig();
                        Helper.TryToEnum<EVehicleType>(_data[0], out config.VehicleType);
                        Helper.TryToEnum<EUnlockType>(_data[1], out config.UnlockType);
                        Helper.TryToEnum<EActiveSkill>(_data[3], out config.LinkedSkill);
                        int.TryParse(_data[2], out config.ValueUnlock);
                        config.BaseHp = Helper.ParseFloat(_data[4]);
                        config.BaseAttack = Helper.ParseFloat(_data[5]);
                        config.BaseMoveSpeed = Helper.ParseFloat(_data[6]);
                        config.nameCar = _data[7];
                        config.iconVisual = Resources.Load<GameObject>(path1 + _data[0] + "_UI");
                        CarVisualizeDict.Add(config.VehicleType, Resources.Load<CarVisualize>(path2 + _data[0] + "_Visual"));
                        VehicleDict.Add(config.VehicleType, config);
                        //CarVisualizeDict.Add(config.VehicleType, null);
                    }
                }
                UnityEditor.EditorUtility.SetDirty(this);
            });
            EditorCoroutine.start(Helper.IELoadData(url, actionComplete));
        }
        #endregion
#endif
    }
    [Serializable]
    public class VehicleConfig
    {
        public EVehicleType VehicleType = EVehicleType.Machine_Gun_Vehicle;
        public EActiveSkill LinkedSkill = EActiveSkill.Machine_Gun;
        public EUnlockType UnlockType = EUnlockType.Coin;
        public int ValueUnlock;
        public float BaseHp;
        public float BaseAttack;
        public float BaseMoveSpeed;

        public GameObject iconVisual;
        public string nameCar;
        //public float BaseHpRecovery;
        //public float BaseCritRate;
        //public float BaseRewardIncrease;
        //public float BaseCooldownIncrese;
        //public float BaseBulletSpeedIncrease;
        //public float BaseExpIncrease;
        //public float BaseItemAbsorbIncrease;
        //public float BaseAreaEffectIncrease;
        //public float BaseEffectDurationIncrease;

        public void CoppyTo(VehicleConfig config)
        {
            config.VehicleType = VehicleType;
            config.LinkedSkill = LinkedSkill;
            config.UnlockType = UnlockType;
            config.ValueUnlock = ValueUnlock;
            
            config.BaseHp = BaseHp;
            config.BaseAttack = BaseAttack;
            config.BaseMoveSpeed = BaseMoveSpeed;

            config.iconVisual = iconVisual;
            config.nameCar = nameCar;
            //config.BaseHpRecovery = BaseHpRecovery;
            //config.BaseCritRate = BaseCritRate;
            //config.BaseRewardIncrease = BaseRewardIncrease;
            //config.BaseCooldownIncrese = BaseCooldownIncrese;
            //config.BaseBulletSpeedIncrease = BaseBulletSpeedIncrease;
            //config.BaseExpIncrease = BaseExpIncrease;
            //config.BaseItemAbsorbIncrease = BaseItemAbsorbIncrease;
            //config.BaseAreaEffectIncrease = BaseAreaEffectIncrease;
            //config.BaseEffectDurationIncrease = BaseEffectDurationIncrease;
        }
        public float GetCarHp()
        {
            float hp = BaseHp;
            if (Player.Instance.PLayerData.vehicleUpgrades.ContainsKey(VehicleType))
            {
                int lvlUpgrade = Player.Instance.PLayerData.vehicleUpgrades[VehicleType];
                if (lvlUpgrade >= 2)
                {
                    for (int i = 2; i <= lvlUpgrade; i++)
                    {
                        hp *= (1 + (float)DataManager.Instance.dataGlobalUpgrade.CarValueIncreasePerLevel / 100);
                    }
                }
            }
            return hp;
        }
        public float GetCarDamage()
        {
            float dmg = BaseAttack;
            if (Player.Instance.PLayerData.vehicleUpgrades.ContainsKey(VehicleType))
            {
                int lvlUpgrade = Player.Instance.PLayerData.vehicleUpgrades[VehicleType];
                if (lvlUpgrade >= 2)
                {
                    for (int i = 2; i <= lvlUpgrade; i++)
                    {
                        dmg *= (1 + (float)DataManager.Instance.dataGlobalUpgrade.CarValueIncreasePerLevel / 100);
                    }
                }
            }
            return dmg;
        }
    }
}

