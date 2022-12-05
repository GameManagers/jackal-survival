using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Car;
using WE.Unit;
using WE.Data;
using WE.Support;
using WE.Utils;

namespace WE.Manager
{
    public class CarController : MonoBehaviour
    {
        public Transform playerBody;
        public DataVehicle dataVehicle;
        public static CarController Instance;

        [HideInInspector]
        public bool currentUpgrade;
        private void Awake()
        {
            Instance = this;
        }

        public CarVisualize CurrentVisual => currentVisual;

        DataPlayer playerData => Player.Instance.PLayerData;
        EVehicleType currentTank => Player.Instance.CurrentTank;
        CarVisualize currentVisual;
        public List<EVehicleType> listType;
        int selectedId;
        public VehicleConfig SelectedConfig => selectedConfig;
        private VehicleConfig selectedConfig;
        public void Init()
        {
            listType = new List<EVehicleType>();
            foreach (KeyValuePair<EVehicleType, VehicleConfig> item in dataVehicle.VehicleDict)
            {
                listType.Add(item.Key);
            }
            selectedId = listType.IndexOf(currentTank);
            Player.Instance.OnCarChange += SetCar;
            GetSelectedConfig();
        }
        public void SetCar(EVehicleType vehicleType)
        {
            playerBody.transform.localEulerAngles = Vector3.zero;
            if (currentVisual != null)
            {
                Destroy(currentVisual.gameObject);
            }
            currentVisual = Instantiate(dataVehicle.CarVisualizeDict[vehicleType], playerBody);
        }
        public void UseWeapon(float timeAttack = 0)
        {
            CurrentVisual.UseWeapon(timeAttack);
        }
        public void PlayFx()
        {
            CurrentVisual.PlayFx();
        }
        public void UpdateGunPos()
        {
            CurrentVisual.gunAnim.transform.position = CurrentVisual.gunBasePos.position;
            CurrentVisual.CheckSorting();
        }
        public void UINextCar()
        {
            selectedId++;
            if (selectedId >= listType.Count)
                selectedId -= listType.Count;

            GetSelectedConfig();
        }
        public void UIPreviousCar()
        {
            selectedId--;
            if (selectedId < 0)
                selectedId += listType.Count;

            GetSelectedConfig();
        }
        public void GetSelectedConfig()
        {
            VehicleConfig config = new VehicleConfig();
            EVehicleType carType = listType[selectedId];
            dataVehicle.VehicleDict[carType].CoppyTo(config);
            selectedConfig = config;
        }
        public bool IsUnlocked()
        {
            return Player.Instance.PLayerData.vehicleUpgrades.ContainsKey(listType[selectedId]);
        }
        public EUnlockType UnlockType()
        {
            return selectedConfig.UnlockType;
        }
        public bool CanUnlock()
        {
            switch (selectedConfig.UnlockType)
            {
                case EUnlockType.Coin:
                    return true;
                case EUnlockType.Kill_Count:
                    return Player.Instance.PLayerData.totalEnemyKill >= selectedConfig.ValueUnlock;
                case EUnlockType.Spend_Coin:
                    return Player.Instance.PLayerData.totalCoinSpend >= selectedConfig.ValueUnlock;
                case EUnlockType.Level_Reach:
                    return Player.Instance.PLayerData.maxMapUnlock >= selectedConfig.ValueUnlock;
                default:
                    break;
            }
            return false;
        }
        public string GetUnlockConditionCount()
        {
            switch (selectedConfig.UnlockType)
            {
                case EUnlockType.Coin:
                    return Player.Instance.currentCoin.ToString() + "/" + selectedConfig.ValueUnlock.ToString();
                case EUnlockType.Kill_Count:
                    return Player.Instance.PLayerData.totalEnemyKill.ToString() + "/" + selectedConfig.ValueUnlock.ToString();
                case EUnlockType.Spend_Coin:
                    return Player.Instance.PLayerData.totalCoinSpend.ToString() + "/" + selectedConfig.ValueUnlock.ToString();
                case EUnlockType.Level_Reach:
                    return Player.Instance.PLayerData.maxMapUnlock.ToString() + "/" + selectedConfig.ValueUnlock.ToString();
                default:
                    break;
            }
            return string.Empty;
        }
        public string GetUnlockConditionText()
        {
            switch (selectedConfig.UnlockType)
            {
                case EUnlockType.Coin:
                    break;
                case EUnlockType.Kill_Count:
                    break;
                case EUnlockType.Spend_Coin:
                    break;
                case EUnlockType.Level_Reach:
                    break;
                default:
                    break;
            }
            return string.Empty;
        }
        //public bool CanUpgrade()
        //{
        //    if (IsUnlocked())
        //    {
        //        if (!IsMaxLevel())
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        public void TestUnlock()
        {
            Player.Instance.UnlockCar(selectedConfig.VehicleType);
        }
        public void UnlockCar()
        {
            if (selectedConfig.UnlockType == EUnlockType.Coin)
            {
                if (Player.Instance.CanBuyItem(selectedConfig.ValueUnlock))
                {
                    Player.Instance.AddCoin(-selectedConfig.ValueUnlock);
                    Player.Instance.UnlockCar(selectedConfig.VehicleType);
                }
                else
                {
                    UIManager.Instance.ShowTextNotEnoughCoin();
                }
            }
            else
            {
                Player.Instance.UnlockCar(selectedConfig.VehicleType);
            }
        }
        public void UpgradeCar()
        {
            if (Player.Instance.CanBuyItem(GetUpgradeCost()))
            {
                Player.Instance.AddCoin(-GetUpgradeCost());
                Player.Instance.UpgradeCar(selectedConfig.VehicleType);
                SoundManager.Instance.PlaySoundFx(SoundManager.Instance.upgradeSfx);
                currentUpgrade = true;
            }
            else
            {
                UIManager.Instance.ShowTextNotEnoughCoin();
            }
        }
        public int SelectedLevel()
        {
            if (Player.Instance.PLayerData.vehicleUpgrades.ContainsKey(selectedConfig.VehicleType))
                return Player.Instance.PLayerData.vehicleUpgrades[selectedConfig.VehicleType];
            else return 1;
        }
        public bool IsSelected()
        {
            return Player.Instance.CurrentTank == selectedConfig.VehicleType;
        }
        //public VehicleUpgrade GetCurrentUpgrade()
        //{
        //    return selectedConfig.UpgadeDictionary[SelectedLevel() + 1];
        //}
        //public VehicleUpgrade GetLastUpgrade()
        //{
        //    return selectedConfig.UpgadeDictionary[SelectedLevel()];
        //}
        public int GetUpgradeCost()
        {
            return DataManager.Instance.dataGlobalUpgrade.GetCarUpgradeCost(SelectedLevel());
        }
        public float GetDamageUpgradeValue()
        {
            float percent = DataManager.Instance.dataGlobalUpgrade.CarValueIncreasePerLevel;
            return selectedConfig.GetCarDamage() / (100 + percent) * percent;
        }
        public float GetHpUpgradeValue()
        {
            float percent = DataManager.Instance.dataGlobalUpgrade.CarValueIncreasePerLevel;
            return selectedConfig.GetCarHp() / (100 + percent ) * percent;
        }
    }
}

