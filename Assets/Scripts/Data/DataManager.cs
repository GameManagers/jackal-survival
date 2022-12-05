using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Data;
namespace WE.Manager
{
    public class DataManager : MonoBehaviour
    {
        public static DataManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        //public DataVehicle dataVehicle;
        public DataZone dataZoneMultiplier;
        public DataZoneChestDrop dataZoneChestDrop;
        public DataGlobalUpgrade dataGlobalUpgrade;
        public DataEffect dataEffect;
    }
}

