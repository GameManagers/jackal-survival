using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using WE.Manager;
using WE.Support;
using WE.Unit;
using UniRx;

namespace WE.Game
{
    public class MapSpawner : MonoBehaviour
    {
        [FoldoutGroup("Zone Object Config")]
        public int startWarningTime = 90;
        [FoldoutGroup("Zone Object Config")]
        public int chestDropInterval = 20;
        [FoldoutGroup("Enemy Spawner Config")]
        public List<EnemyInLevel> enemyInLevels;
        [FoldoutGroup("Enemy Spawner Config")]
        public float MaxEnemyInSec;
        [FoldoutGroup("Enemy Spawner Config")]
        public AnimationCurve spawnIntervalCurve;
        [FoldoutGroup("Enemy Spawner Config")]
        public float startMaxEnemyInMap, endMaxEnemyInMap;


        [FoldoutGroup("Zone Multiple")]
        [MinValue(1)]
        public float zoneEndTimeMultiple;
        [FoldoutGroup("Boss Multiple")]
        public float bossInterval = 60, hpMulti = 40, dmgMulti = 10, expMulti, weightMulti = 20;

        public float InZoneMultiple => inZoneMultiple;
        public float CurrentMaxEnemyInMap => currentMaxEnemyInMap;
        Enemy currentEnemyToSpawn;
        float inZoneMultiple;
        float currentMaxEnemyInMap;
        float currentSpawnInterval;
        int enemyNumb;
        CompositeDisposable disposables;
        int currentEnemy => EnemySpawner.Instance.CurrentEnemy;
        public void Init()
        {
            enemyNumb = enemyInLevels.Count;
            CalculateValue();
            disposables = new CompositeDisposable();
            Observable.Timer(System.TimeSpan.FromSeconds(currentSpawnInterval)).Subscribe(_ => SpawnEnemy()).AddTo(disposables);
            Observable.Interval(System.TimeSpan.FromSeconds(bossInterval)).Subscribe(_ => SpawnBoss()).AddTo(disposables);
            Observable.Timer(System.TimeSpan.FromSeconds(startWarningTime)).Subscribe(_ => StartWarning()).AddTo(disposables);
            Observable.Interval(System.TimeSpan.FromSeconds(chestDropInterval)).Subscribe(_ => SpawnChest()).AddTo(disposables);
        }
        public void OnTick()
        {
            CalculateValue();
        }
        public void Stop()
        {
            disposables.Dispose();
            Destroy(gameObject);
        }
        public void SpawnLastWave()
        {
            disposables.Dispose();
            for (int i = 0; i < enemyInLevels.Count; i++)
            {
                SpawnBoss(enemyInLevels[i].enemy);
            }
        }
        public void StartWarning()
        {
            GameplayManager.Instance.StartWarning();
        }
        public void SpawnChest()
        {
            Helper.SpawnChestIngame();
        }
        void CalculateValue()
        {
            float evaluate = GameplayManager.Instance.CurrentTimeEvaluate;
            inZoneMultiple = 1 + (evaluate * (zoneEndTimeMultiple - 1));
            currentMaxEnemyInMap = startMaxEnemyInMap + (evaluate * (endMaxEnemyInMap - startMaxEnemyInMap));
            float enemyOnSec = spawnIntervalCurve.Evaluate(evaluate) * MaxEnemyInSec;
            if (enemyOnSec < 1)
                enemyOnSec = 1;
            currentSpawnInterval = 1 / enemyOnSec;
            //int currentMin = Mathf.CeilToInt((float)GameplayManager.Instance.CurrentTimePlay / 63f);
            //if (currentMin < 1)
            //    currentMin = 1;
            //if (currentMin <= enemyNumb)
            //{
            //    currentEnemyToSpawn = enemyInLevels[currentMin - 1].enemy;
            //}
            //else
            //{
            //    Dictionary<Enemy, float> dicRate = new Dictionary<Enemy, float>();
            //    for (int i = 0; i < enemyInLevels.Count; i++)
            //    {
            //        dicRate.Add(enemyInLevels[i].enemy, enemyInLevels[i].GetPriority());
            //    }
            //    currentEnemyToSpawn = Helper.GetRandomByPercent(dicRate);
            //}
            Dictionary<Enemy, float> dicRate = new Dictionary<Enemy, float>();
            for (int i = 0; i < enemyInLevels.Count; i++)
            {
                dicRate.Add(enemyInLevels[i].enemy, enemyInLevels[i].GetPriority());
            }
            currentEnemyToSpawn = Helper.GetRandomByPercent(dicRate);

        }
        public void SpawnEnemy()
        {
            if(currentEnemy < currentMaxEnemyInMap)
                Helper.SpawnEnemy(currentEnemyToSpawn);
            Observable.Timer(System.TimeSpan.FromSeconds(currentSpawnInterval)).Subscribe(_ => SpawnEnemy()).AddTo(disposables);
        }
        public void SpawnBoss()
        {
            Enemy e = Helper.SpawnEmptyEnemy(currentEnemyToSpawn);
            e.Init(true, hpMulti, dmgMulti, expMulti, weightMulti, 2.5f);
        }
        public void SpawnBoss(Enemy _e)
        {
            Enemy e = Helper.SpawnEmptyEnemy(_e);
            e.Init(true, hpMulti, dmgMulti, expMulti, weightMulti, 2.5f);
        }
    }
    [System.Serializable]
    public class EnemyInLevel
    {
        public Enemy enemy;
        public AnimationCurve curvePriority;

        public float GetPriority()
        {
            return curvePriority.Evaluate(GameplayManager.Instance.CurrentTimeEvaluate);
        }
    }
}

