using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Manager;
using Sirenix.OdinInspector;
using WE.Support;
using WE.Game;

namespace WE.Data
{
    [CreateAssetMenu(fileName = "Data Zone Multiplier", menuName = "WE/Data/Data Zone Multiplier")]
    public class DataZone : SerializedScriptableObject
    {
        #region old Logic
        //public Dictionary<int, ZoneConfig> ZoneConfigs;
        //#if UNITY_EDITOR
        //        [Button("Get Data")]
        //        public void GetData()
        //        {
        //            ZoneConfigs = new Dictionary<int, ZoneConfig>();
        //            string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTBO_e7NjDFNq1K56OO5JB19OFqI3VyA8vIW345_YL1QnanxaV0MNvCKJBcpkkWF9DziZOWzuQmXqTG/pub?gid=478127169&single=true&output=csv";
        //            System.Action<string> actionComplete = new System.Action<string>((string str) =>
        //            {
        //                var data = CSVReader.ReadCSV(str);
        //                for (int i = 1; i < data.Count; i++)
        //                {
        //                    var _data = data[i];
        //                    ZoneConfig config = new ZoneConfig();
        //                    string zid = i.ToString();
        //                    if (i < 10)
        //                        zid = "0" + i.ToString();
        //                    string path = "Prefabs/Map/MapElement/Map_Element_" + zid;
        //                    Debug.Log(path);
        //                    config.mapPrefab = Resources.Load<MapElement>(path);
        //                    config.zoneId = i;
        //                    config.DamageMultiplier = Helper.ParseFloat(_data[1]);
        //                    config.HpMultiplier = Helper.ParseFloat(_data[2]);
        //                    config.InZoneMultiplier = Helper.ParseFloat(_data[3]);
        //                    config.CoinMultiplier = Helper.ParseFloat(_data[4]);
        //                    config.nameZone = _data[6];
        //                    int.TryParse(_data[5], out config.Timeplay);
        //                    ZoneConfigs.Add(i, config);
        //                }
        //                UnityEditor.EditorUtility.SetDirty(this);
        //            });
        //            EditorCoroutine.start(Helper.IELoadData(url, actionComplete));
        //        }
        //#endif
        #endregion

        [FoldoutGroup("Base Value Setup")]
        public int BGCount;
        [FoldoutGroup("Base Value Setup")]
        public string bgPath;
        [FoldoutGroup("Base Value Setup")]
        public int MapSpawnerCount;
        [FoldoutGroup("Base Value Setup")]
        public string MapSpawnerPath;


        [FoldoutGroup("Data Multiple Receipe")]
        [InfoBox("Multiple of Zone = Zone Multiple * id Zone")]
        public float zoneMultiple;
        [FoldoutGroup("Data Multiple Receipe")]
        public int zoneTimePlay;

        //[FoldoutGroup("Endless Move")]
        //[InfoBox("Multiple of Zone = minutePlaying * Multiple per Minute")]
        //public float multiplePerMinute;
        public ZoneConfig GetZoneConfig(int idZone)
        {
            ZoneConfig config = new ZoneConfig();
            config.zoneId = idZone;
            config.zoneMultiplier = zoneMultiple * idZone;
            config.mapIcon = GetMapIcon(idZone);


            return config;
        }
        public Sprite GetMapIcon(int idZone)
        {
            if (idZone > BGCount)
            {
                idZone = (idZone - 1) % BGCount + 1;
            }
            Sprite spr = Resources.Load<Sprite>(bgPath + idZone.ToString());
            return spr;
        }
        public MapSpawner GetMapSpawner(int idZone)
        {
            if (idZone > MapSpawnerCount)
            {
                idZone = (idZone - 1) % MapSpawnerCount + 1;
            }
            MapSpawner map = Resources.Load<MapSpawner>(MapSpawnerPath + idZone.ToString());
            return map;
        }
    }
    [Serializable]
    public class ZoneConfig
    {
        public Sprite mapIcon;
        public int zoneId;
        public float zoneMultiplier;
    }
}

