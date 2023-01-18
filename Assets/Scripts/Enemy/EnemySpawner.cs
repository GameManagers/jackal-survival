using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Manager;
using WE.Support;
using WE.Unit;
using WE.Utils;
using WE.Game;
using Sirenix.OdinInspector;
using UniRx;
using WE.Data;
using TigerForge;

namespace WE.Manager
{
    public class EnemySpawner : MonoBehaviour
    {
        public static EnemySpawner Instance;
        private void Awake()
        {
            Instance = this;
        }
        public int CurrentEnemy
        {
            get
            {
                if (listActiveEnemy != null)
                    return listActiveEnemy.Count;
                else
                    return 0;
            }
        }
        public List<Enemy> listActiveEnemy;

        private int totalTickCount;
        public int TotalTickCount => totalTickCount;
        [FoldoutGroup("Material")]
        public Material defautMaterial;
        [FoldoutGroup("Material")]
        public Material bossMaterial;


        [FoldoutGroup("Spawning")]
        public float ReSpawnOffset;

        public EndlessMapSpawner endlessSpawner;
        MapSpawner currentMapSpawner;
        EndlessMapSpawner currentEndlessMapSpawner;

        public float IngameMulti
        {
            get
            {
                switch (GameplayManager.Instance.CurrentGameplayType)
                {
                    case GameType.Campaign:
                    case GameType.PVP:
                        return currentMapSpawner.InZoneMultiple;
                    case GameType.Tutorial:
                        return 1;
                    case GameType.Endless:
                        return currentEndlessMapSpawner.CurrentMultiple;
                    default:
                        return 1;
                }
            }
        }
        public float CoinMulti => currentEndlessMapSpawner.CurrentCoinMultiple;
        bool IsLastEnemy;
        public void StartGame()
        {
            listActiveEnemy = new List<Enemy>();
            EventManager.StartListening(Constant.TIMER_UPDATE_EVENT, OnUpdate);
            EventManager.StartListening(Constant.GAME_TICK_EVENT, Tick);
            switch (GameplayManager.Instance.CurrentGameplayType)
            {
                case GameType.Campaign:
                    currentMapSpawner = Instantiate(DataManager.Instance.dataZoneMultiplier.GetMapSpawner(Player.Instance.CurrentMap), this.transform);
                    IsLastEnemy = false;
                    currentMapSpawner.Init();
                    totalTickCount = 0;
                    break;
                case GameType.Tutorial:
                    break;
                case GameType.Endless:
                    currentEndlessMapSpawner = Instantiate(endlessSpawner, this.transform);
                    currentEndlessMapSpawner.Init();
                    break;
                case GameType.PVP:
                    currentMapSpawner = Instantiate(DataManager.Instance.dataZoneMultiplier.GetMapSpawner(Player.Instance.CurrentMap), this.transform);
                    IsLastEnemy = false;
                    currentMapSpawner.Init();
                    totalTickCount = 0;
                    break;
                default:
                    break;
            }

        }
        public void EndGame()
        {
            if (currentMapSpawner != null)
                currentMapSpawner.Stop();
            if (currentEndlessMapSpawner != null)
                currentEndlessMapSpawner.Stop();
            ClearEnemy();
            EventManager.StopListening(Constant.GAME_TICK_EVENT, Tick);
            EventManager.StopListening(Constant.TIMER_UPDATE_EVENT, OnUpdate);
        }
        [Button("Despawn Enemy")]
        public void ClearEnemy()
        {
            int count = listActiveEnemy.Count;
            DebugCustom.Log("listCount", count);
            for (int i = 0; i < count; i++)
            {
                listActiveEnemy[0].Despawn();
            }
        }
        public void SpawnLastWave()
        {
            currentMapSpawner.SpawnLastWave();
            IsLastEnemy = true;
        }
        public void Tick()
        {
            switch (GameplayManager.Instance.CurrentGameplayType)
            {
                case GameType.Campaign:
                case GameType.PVP:
                    totalTickCount++;
                    currentMapSpawner.OnTick();
                    if (IsLastEnemy)
                    {
                        if (listActiveEnemy.Count == 0)
                        {
                            IsLastEnemy = false;
                            Observable.Timer(System.TimeSpan.FromSeconds(3)).Subscribe(_ => WinGame()).AddTo(gameObject);
                        }
                    }
                    break;
                case GameType.Tutorial:
                    break;
                case GameType.Endless:
                    currentEndlessMapSpawner.OnTick();
                    break;
                default:
                    break;
            }
        }
        public void WinGame()
        {
            GameplayManager.Instance.ShowPopupEndGame(true);
        }
        public void OnUpdate()
        {
            float t = EventManager.GetFloat(Constant.TIMER_UPDATE_EVENT);
            foreach (Enemy item in listActiveEnemy)
            {
                item.OnUpdate(t);
            }
        }

        public void DealDamageToAllEnemy(float value)
        {
            int count = listActiveEnemy.Count;
            int id = 0;
            for (int i = 0; i < count; i++)
            {
                if(id < listActiveEnemy.Count)
                {
                    if (listActiveEnemy[id].IsBoss)
                    {
                        Enemy _boss = listActiveEnemy[id];
                        _boss.TakeDamage(value, 0);
                        if(_boss.IsAlive)
                            id++;
                    }
                    else
                    {
                        listActiveEnemy[id].Die();
                    }
                }
                
            }
        }
    }
}

