using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Support;
using Sirenix.OdinInspector;
using WE.Data;
using WE.Manager;
using WE.Unit;

namespace WE.Game
{
    public class MapLooper : MonoBehaviour
    {
        public ResolutionManager resolutionManager => ResolutionManager.Instance;
        public MapElement mapPrefab;
        public Sprite[] endlessBG;
        float mapWidth;
        float mapHeight;
        public Transform _Player;
        float ScreenLeft => resolutionManager.ScreenLeftEdge;
        float ScreenRight => resolutionManager.ScreenRightEdge;
        float ScreenTop => resolutionManager.ScreenTopEdge;
        float ScreenBottom => resolutionManager.ScreenBottomEdge;
        int currentTopId, currentBottomId, currentLeftId, currentRightId, lastTopId, lastBottomId, lastLeftId, lastRightId;
        public Dictionary<Vector2Int, MapElement> childMaps;
        ZoneConfig currentConfig => MapController.Instance.currentMapConfig;
        Sprite currentEndlessBG;
        public void InitMap()
        {
            mapWidth = mapPrefab.mapWidth;
            mapHeight = mapPrefab.mapHeight;
            if (childMaps != null)
            {
                foreach (KeyValuePair<Vector2Int, MapElement> item in childMaps)
                {
                    if (item.Value != null)
                        Helper.Despawn(item.Value);
                }
            }
            childMaps = new Dictionary<Vector2Int, MapElement>();
            lastBottomId = 0;
            lastLeftId = 0;
            lastRightId = 0;
            lastTopId = 0;
            currentEndlessBG = endlessBG[Random.Range(0, endlessBG.Length)];
            CheckEdge();
        }
        public void CheckEdge()
        {
            currentTopId = GetGridId(ScreenTop, mapHeight, true);
            currentBottomId = GetGridId(ScreenBottom, mapHeight, false);
            currentLeftId = GetGridId(ScreenLeft, mapWidth, false);
            currentRightId = GetGridId(ScreenRight, mapWidth, true);
            if (currentTopId != lastTopId || currentRightId != lastRightId || currentBottomId != lastBottomId || currentLeftId != lastLeftId)
            {
                Dictionary<Vector2Int, MapElement> newDic = new Dictionary<Vector2Int, MapElement>();
                for (int i = currentLeftId; i < currentRightId + 1; i++)
                {
                    for (int j = currentBottomId; j < currentTopId + 1; j++)
                    {
                        Vector2Int id = new Vector2Int(i, j);
                        if (childMaps.ContainsKey(id))
                        {
                            newDic.Add(id, childMaps[id]);
                            childMaps.Remove(id);
                        }
                        else
                        {
                            newDic.Add(id, AddMap(id));
                        }
                    }
                }
                if (childMaps.Count > 0)
                {
                    foreach (KeyValuePair<Vector2Int, MapElement> item in childMaps)
                    {
                        Helper.Despawn(item.Value);
                    }
                }
                childMaps = newDic;
                lastBottomId = currentBottomId;
                lastLeftId = currentLeftId;
                lastRightId = currentRightId;
                lastTopId = currentTopId;
            }
        }
        public int GetGridId(float value, float step, bool add)
        {
            if (add)
                return Mathf.CeilToInt(value / step) + 1;
            else
                return Mathf.FloorToInt(value / step) - 1;
        }
        public MapElement AddMap(Vector2Int gridId)
        {
            MapElement e = Helper.Spawn<MapElement>(mapPrefab, new Vector3(gridId.x * mapWidth, gridId.y * mapHeight), Quaternion.identity, this.transform);
            if(GameplayManager.Instance.CurrentGameplayType == GameType.Endless && GameplayManager.Instance.State == GameState.Gameplay)
                e.icon.sprite = currentEndlessBG;
            else
                e.icon.sprite = currentConfig.mapIcon;
            return e;
        }
    }
}

