using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Manager;
using Sirenix.OdinInspector;
using WE.Support;

namespace WE.Data
{
    [CreateAssetMenu(fileName = "Data Global Upgrade", menuName = "WE/Data/Data Global Upgrade")]
    public class DataGlobalUpgrade : SerializedScriptableObject
    {
        #region Old Logic

        //public GlobalUpgradeConfig hpUpgradeConfig;
        //public GlobalUpgradeConfig attackUpgradeConfig;
        //public GlobalUpgradeConfig rewardUpgradeConfig;
        //public int[] InGameUpgradeExp;
        //#if UNITY_EDITOR
        //        [Button("Load Data Interaction")]
        //        public void LoadDataInteractive()
        //        {
        //            hpUpgradeConfig = new GlobalUpgradeConfig();
        //            attackUpgradeConfig = new GlobalUpgradeConfig();
        //            rewardUpgradeConfig = new GlobalUpgradeConfig();
        //            string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTBO_e7NjDFNq1K56OO5JB19OFqI3VyA8vIW345_YL1QnanxaV0MNvCKJBcpkkWF9DziZOWzuQmXqTG/pub?gid=1785443572&single=true&output=csv";
        //            System.Action<string> actionComplete = new System.Action<string>((string str) =>
        //            {
        //                var data = CSVReader.ReadCSV(str);
        //                int.TryParse(data[1][1], out hpUpgradeConfig.valuePerLevel);
        //                int.TryParse(data[1][2], out attackUpgradeConfig.valuePerLevel);
        //                int.TryParse(data[1][3], out rewardUpgradeConfig.valuePerLevel);
        //                List<int> ingameExpConfig = new List<int>();
        //                List<int> hpConfig = new List<int>();
        //                List<int> attackConfig = new List<int>();
        //                List<int> rewardConfig = new List<int>();
        //                for (int i = 2; i < data.Count; i++)
        //                {
        //                    var _data = data[i];
        //                    hpConfig.Add(int.Parse(_data[1]));
        //                    attackConfig.Add(int.Parse(_data[2]));
        //                    rewardConfig.Add(int.Parse(_data[3]));
        //                    ingameExpConfig.Add(int.Parse(_data[4]));
        //                }
        //                InGameUpgradeExp = ingameExpConfig.ToArray();
        //                hpUpgradeConfig.costPerLevel = hpConfig.ToArray();
        //                attackUpgradeConfig.costPerLevel = attackConfig.ToArray();
        //                rewardUpgradeConfig.costPerLevel = rewardConfig.ToArray();
        //                UnityEditor.EditorUtility.SetDirty(this);
        //            });
        //            EditorCoroutine.start(Helper.IELoadData(url, actionComplete));
        //        }
        //#endif

        #endregion

        [FoldoutGroup("EXP")]
        public float nextLevelExpMultiple = 10;

        [FoldoutGroup("Global Upgrade")]
        public int GUValueIncreasePerLevel = 10;
        [FoldoutGroup("Global Upgrade")]
        public int GUCostMultiple = 1000;

        [FoldoutGroup("Car Upgrade")]
        public int CarValueIncreasePerLevel = 10;
        [FoldoutGroup("Car Upgrade")]
        public int CarCostMultiple = 2000;




        public int GetCarUpgradeCost(int currentLevel)
        {
            return CarCostMultiple * (currentLevel);
        }
        public int GetGlobalUpgradeCost(int currentlevel)
        {
            return GUCostMultiple * (currentlevel + 1);
        }
        public float GetNextLevelExp(float currentLevelExp, int currentLevel)
        {
            return currentLevelExp + nextLevelExpMultiple * Mathf.CeilToInt((float)currentLevel / 10);
        }
    }
    //[Serializable]
    //public class GlobalUpgradeConfig
    //{
    //    public int valuePerLevel;
    //    public int[] costPerLevel;
    //}
}

