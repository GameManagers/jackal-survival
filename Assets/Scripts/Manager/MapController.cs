using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Data;
using Sirenix.OdinInspector;
using WE.Game;
using WE.Unit;
using WE.Utils;

namespace WE.Manager
{
    public class MapController : MonoBehaviour
    {
        public static MapController Instance;
        private void Awake()
        {
            Instance = this;
        }
        public DataZone zoneData;
        public MapLooper mapLooper;

        public int CurrentMap => Player.Instance.CurrentMap;
        public ZoneConfig currentMapConfig => currentConfig;
        ZoneConfig currentConfig;
        public void Init()
        {
            OnChangeZone();
            TigerForge.EventManager.StopListening(Constant.TIMER_UPDATE_EVENT, OnUpdate);
            TigerForge.EventManager.StopListening(Constant.ON_CHANGE_ZONE, OnChangeZone);
            LoadMap();
            TigerForge.EventManager.StartListening(Constant.TIMER_UPDATE_EVENT, OnUpdate);
            TigerForge.EventManager.StartListening(Constant.ON_CHANGE_ZONE, OnChangeZone);
        }
        private void OnDisable()
        {

            TigerForge.EventManager.StopListening(Constant.TIMER_UPDATE_EVENT, OnUpdate);
            TigerForge.EventManager.StopListening(Constant.ON_CHANGE_ZONE, OnChangeZone);
        }
        public void OnUpdate()
        {
            if (GameplayManager.Instance.State == GameState.Gameplay)
            {
                if (Player.Instance.tankMovement.IsTouching)
                {
                    mapLooper.CheckEdge();
                }
            }
        }
        public void LoadMap()
        {
            mapLooper.InitMap();
        }
        public void OnChangeZone()
        {
            currentConfig = zoneData.GetZoneConfig(CurrentMap);
        }

        public void OnChangeZone(int zoneId)
        {
            currentConfig = zoneData.GetZoneConfig(zoneId);
        }
    }
}

