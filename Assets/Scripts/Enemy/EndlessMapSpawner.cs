using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Unit;
using WE.Manager;
using WE.Support;
using Sirenix.OdinInspector;
using UniRx;

namespace WE.Game
{
    public class EndlessMapSpawner : MonoBehaviour
    {
        [Header("BaseSetting")]
        public float startWarningTime;
        public float chestInterval;
        [InfoBox(" Multiple In Zone = Multiplier On Minutes * current playtime (mins)")]
        public float multiplierOnMinutes = 1.1f;
        public float bossSpawnInterval = 60f;
        [Header("Max Enemy In Map")]
        public float startMaxEnemyInMap = 50;
        public float maxEnemyAddPerSec = 0.5f;
        public float maxMaxEnemyInMAp = 700;
        [Header("Enemy Spawn Per Sec")]
        public float startEnemyPerSec = 1;
        public float enemyPerSecAddedPerSec = 0.2f;
        public float maxEnemyPerSec = 200;
        [FoldoutGroup("Boss Multiple")]
        public float bossInterval = 60, hpMulti = 40, dmgMulti = 10, expMulti, weightMulti = 20;


        [FoldoutGroup("Coin Multiple")]
        public float startCoinMultiple = 1, coinMultipleAddPerMin = 0.5f;

        [FoldoutGroup("Enemy Spawn Set")]
        public List<EndlessSpawnSet> spawnSets;

        CompositeDisposable disposables;

        float currentInterval => 1 / currentEnemyPerSec;
        float currentMaxEnemyInMap;
        float currentMultiple;
        float currentEnemyPerSec;
        float multipleStep;
        //float maxEnemyStep;
        //float enemyPerSecStep;
        Enemy currentEnemy;
        Dictionary<Enemy, float> dicRate;
        public float CurrentMultiple => currentMultiple;
        float currentCoinMultiple;
        public float CurrentCoinMultiple => currentCoinMultiple;
        public void Init()
        {
            for (int i = 0; i < spawnSets.Count; i++)
            {
                spawnSets[i].SetStart();
            }
            CalCulateValue();
            currentMaxEnemyInMap = startMaxEnemyInMap;
            currentMultiple = 1;
            currentEnemyPerSec = startEnemyPerSec;
            multipleStep = multiplierOnMinutes / 60;
            currentCoinMultiple = startCoinMultiple;
            disposables = new CompositeDisposable();
            Observable.Interval(System.TimeSpan.FromSeconds(bossSpawnInterval)).Subscribe(_ => SpawnBoss()).AddTo(disposables);
            Observable.Interval(System.TimeSpan.FromSeconds(chestInterval)).Subscribe(_ => Helper.SpawnChestIngame()).AddTo(disposables);
            Observable.Timer(System.TimeSpan.FromSeconds(startWarningTime)).Subscribe(_ => GameplayManager.Instance.StartWarning()).AddTo(disposables);
            Observable.Timer(System.TimeSpan.FromSeconds(currentInterval)).Subscribe(_ => SpawnEnemy()).AddTo(disposables);
            Observable.Interval(System.TimeSpan.FromMinutes(1)).Subscribe(_ => OnAddMinutes()).AddTo(disposables);
        }

        private void CalCulateValue()
        {
            if (currentMaxEnemyInMap < maxMaxEnemyInMAp)
                currentMaxEnemyInMap += maxEnemyAddPerSec;
            if (currentEnemyPerSec < maxEnemyPerSec)
                currentEnemyPerSec += enemyPerSecAddedPerSec;
            currentMultiple += multipleStep;


            dicRate = new Dictionary<Enemy, float>();
            for (int i = 0; i < spawnSets.Count; i++)
            {
                dicRate.Add(spawnSets[i].enemy, spawnSets[i].CurrentPriority);
            }
            currentEnemy = Helper.GetRandomByPercent(dicRate);
        }

        public void OnTick()
        {
            CalCulateValue();
        }
        public void Stop()
        {
            disposables.Dispose();
            Destroy(gameObject);
        }
        public void OnAddMinutes()
        {
            currentCoinMultiple += coinMultipleAddPerMin;
            for (int i = 0; i < spawnSets.Count; i++)
            {
                spawnSets[i].OnMinute();
            }
        }
        public void SpawnEnemy()
        {
            if (EnemySpawner.Instance.CurrentEnemy < currentMaxEnemyInMap)
            {
                Helper.SpawnEnemy(currentEnemy);
            }
            Observable.Timer(System.TimeSpan.FromSeconds(currentInterval)).Subscribe(_ => SpawnEnemy()).AddTo(disposables);
        }
        public void SpawnBoss()
        {
            Enemy e = Helper.SpawnEmptyEnemy(currentEnemy);
            e.Init(true, hpMulti, dmgMulti, expMulti, weightMulti, 2.5f);
        }
    }
    [System.Serializable]
    public class EndlessSpawnSet
    {
        public Enemy enemy;
        public float startPriority = 1;
        public int mintuteStartPriorityAdd = 0;
        public float prioriAddEachMinutes = 1;
        public float maxPriority = 100;

        int currentMinutes;
        public float CurrentPriority => currentPriority;
        float currentPriority;
        public void SetStart()
        {
            currentMinutes = 1;
            currentPriority = startPriority;
        }
        public void OnMinute()
        {
            currentMinutes++;
            if (currentMinutes >= mintuteStartPriorityAdd)
            {
                currentPriority = startPriority + (currentMinutes - mintuteStartPriorityAdd) * prioriAddEachMinutes;
                if (currentPriority > maxPriority)
                    currentPriority = maxPriority;
                if (currentPriority < 0)
                    currentPriority = 0;
            }
        }
    }
}

