using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using WE.Unit;
using System;

namespace WE.Manager
{
    [CreateAssetMenu(fileName = "Enemy Data", menuName = "WE/Data/Data Enemy")]
    public class EnemyData : SerializedScriptableObject
    {
        public List<Enemy> listEnemy;
        public Dictionary<string, Enemy> enemyDictData;
        [Button("Gen Dic")]
        public void GenerateDictionary()
        {
            enemyDictData = new Dictionary<string, Enemy>();
            for (int i = 0; i < listEnemy.Count; i++)
            {
                enemyDictData.Add(listEnemy[i].EnemyId, listEnemy[i]);
            }
        }
    }
}

