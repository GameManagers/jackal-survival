using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Manager;
using Sirenix.OdinInspector;
using WE.Unit;

namespace WE.Data
{
    [CreateAssetMenu(fileName = "Data Zone Action", menuName = "WE/Data/Data Zone Action")]
    public class DataZoneAction : SerializedScriptableObject
    {
        public Dictionary<int, List<ActionConfig>> ZoneActionDict;
    }
    [Serializable]
    public class ActionConfig
    {
        public int tickCount;
        public ZoneAction action;
        public int value;
        public float interval;
        public Enemy[] EnemyPrefabs;
    }

    public enum ZoneAction
    {
        HugeWave,
        SpawnRandom,
        SpawnPack,
        BossFight,
    }
}

