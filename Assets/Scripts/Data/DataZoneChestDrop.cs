using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Manager;
using Sirenix.OdinInspector;
using WE.Support;
namespace WE.Data
{
    [CreateAssetMenu(fileName = "Data Zone Chest Drop", menuName = "WE/Data/Data Zone Chest Drop")]
    public class DataZoneChestDrop : SerializedScriptableObject
    {
        public Dictionary<string, ChestDropItemConfig> ZoneChestDropData;
        public Dictionary<string, float> ZoneChestDropRate;
        public Dictionary<TypeChest, float> RateChestBoss;
#if UNITY_EDITOR
        [Button("Get Data")]
        public void GetData()
        {
            ZoneChestDropData = new Dictionary<string, ChestDropItemConfig>();
            ZoneChestDropRate = new Dictionary<string, float>();
            string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTBO_e7NjDFNq1K56OO5JB19OFqI3VyA8vIW345_YL1QnanxaV0MNvCKJBcpkkWF9DziZOWzuQmXqTG/pub?gid=1604362137&single=true&output=csv";
            System.Action<string> actionComplete = new System.Action<string>((string str) =>
            {
                var data = CSVReader.ReadCSV(str);
                for (int i = 1; i < data.Count; i++)
                {
                    var _data = data[i];
                    ChestDropItemConfig config = new ChestDropItemConfig();
                    config.dropType = _data[0];
                    int.TryParse(_data[1], out config.Quantity);
                    config.Priority = Helper.ParseFloat(_data[2]);
                    ZoneChestDropData.Add(_data[0], config);
                    ZoneChestDropRate.Add(_data[0], config.Priority);
                }
                UnityEditor.EditorUtility.SetDirty(this);
            });
            EditorCoroutine.start(Helper.IELoadData(url, actionComplete));
        }
#endif
    }
    [Serializable]
    public class ChestDropItemConfig
    {
        public string dropType;
        public int Quantity;
        public float Priority;
    }
}

