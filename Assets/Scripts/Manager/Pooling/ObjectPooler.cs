using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Game;
using Sirenix.OdinInspector;
using WE.Effect;

namespace WE.Pooling
{
    public class ObjectPooler : MonoBehaviour
    {
        #region Singleton
        public static ObjectPooler Instance;
        private void Awake()
        {
            Instance = this;
        }
        #endregion

        [Header("======= Text Impact Unit =========")]
        [FoldoutGroup("Text Impact Unit")]
        public Transform textImpactParent;
        [FoldoutGroup("Text Impact Unit")]
        public TextDamage textDamagePrefabs;
        [FoldoutGroup("Text Impact Unit")]
        public float fadeTime = 0.2f;
        [FoldoutGroup("Text Impact Unit")]
        public Color normalHitColor = Color.white;
        [FoldoutGroup("Text Impact Unit")]
        public Color critHitColor = Color.red;
        [FoldoutGroup("Text Impact Unit")]
        public Color healTextColor = Color.green;
        [FoldoutGroup("Text Impact Unit")]
        public float normalTextSize = 18;
        [FoldoutGroup("Text Impact Unit")]
        public float critTextSize = 25;
        [FoldoutGroup("Text Impact Unit")]
        public float healTextSize = 20;
        [FoldoutGroup("Text Impact Unit")]
        public float normalTextTime = 1.5f;
        [FoldoutGroup("Text Impact Unit")]
        public float critTextTime = 2.5f;
        [FoldoutGroup("Text Impact Unit")]
        public float textRandomOffset = 0.25f;


        [FoldoutGroup("Ingame Item Drop")]
        public ItemInGame ingameItemPrefabs;
        [FoldoutGroup("Ingame Item Drop")]
        public ChestInGame chestInGamePrefabs;
        [FoldoutGroup("Ingame Item Drop")]
        public BossChestDirection bossChestDirection;
        [FoldoutGroup("Ingame Item Drop")]
        public AnimationEffect fxItem;
        [FoldoutGroup("Ingame Item Drop")]
        public AnimationEffect fxReviceChest;
        [FoldoutGroup("Ingame Item Drop")]
        public float itemFlySpeed;
        [FoldoutGroup("Ingame Item Drop")]
        public float itemFlySpeedIncrease;
        [FoldoutGroup("Ingame Item Drop")]
        public float itemFlyTime;
        [FoldoutGroup("Ingame Item Drop")]
        public float flyBackDistance = 1.5f;

        [FoldoutGroup("Ingame Item Drop")]
        public Sprite iconSmallExp, iconNormalExp, iconLargeExp, iconHugeExp, iconSmallCoin, iconNormalCoin, iconBigCoin, iconMagnetic, iconBomb, iconHeal, iconBossChest, iconKey;
        [FoldoutGroup("Ingame Item Drop")]
        public int expSO, coinSO, itemSO, bossChestSO;


        [FoldoutGroup("Player Fx")]
        public AnimationEffect fxReviceSkill, fxUiUpgare;


        [FoldoutGroup("Enemy Sprite")]
        public Sprite[] ElectricShockSprite;
        [FoldoutGroup("Enemy Sprite")]
        public AnimationEffect fxEnemyDie;
        [System.Serializable]
        public class Pool
        {
            public List<PoolingObject> activeList = new List<PoolingObject>();
            public List<PoolingObject> deactiveList = new List<PoolingObject>();
            public string poolingKey;
        }
        public Dictionary<string, Pool> poolingDict;
        public List<ItemInGame> listExp;
        public List<ItemInGame> listItemInGames;





        public void Init()
        {
            //ClearPool();
            transform.position = new Vector3(0, 0, -100);
            listExp = new List<ItemInGame>();
            poolingDict = new Dictionary<string, Pool>();
            listItemInGames = new List<ItemInGame>();

        }
        public PoolingObject Spawn(PoolingObject refPrefabs)
        {
            string key = refPrefabs.poolingKey;
            if (poolingDict.ContainsKey(key))
            {
                if (poolingDict[key].deactiveList.Count > 0)
                {
                    PoolingObject obj = poolingDict[key].deactiveList[0];
                    poolingDict[key].deactiveList.RemoveAt(0); 
                    poolingDict[key].activeList.Add(obj); 
                    obj.gameObject.SetActive(true);
                    //obj.Available = false;
                    obj.OnSpawn();
                    return obj;
                }
                else
                {
                    PoolingObject obj = Instantiate(refPrefabs);
                    poolingDict[key].activeList.Add(obj);
                    //obj.Available = false;
                    obj.OnSpawn();
                    return obj;
                }
            }
            else
            {
                Pool newPool = new Pool();
                newPool.poolingKey = key;
                PoolingObject obj = Instantiate(refPrefabs);
                newPool.activeList.Add(obj);
                poolingDict.Add(key, newPool);
                //obj.Available = false;
                obj.OnSpawn();
                return obj;
            }
            
        }
        public void Despawn(PoolingObject obj)
        {
            if (obj == null)
                return;
            //obj.Available = false;
            if (obj is ItemInGame)
            {
                listItemInGames.Remove((ItemInGame)obj);
            }
            if (obj.dontNeedDespawn)
            {
                obj.transform.SetParent(this.transform);
                obj.transform.localPosition = Vector3.zero;
            }
            else
            {
                obj.gameObject.SetActive(false);
            }
            string key = obj.poolingKey;
            if (poolingDict.ContainsKey(key))
            {
                if (poolingDict[key].activeList.Contains(obj))
                    poolingDict[key].activeList.Remove(obj);
                poolingDict[key].deactiveList.Add(obj);
            }
            else
            {
                Pool newPool = new Pool();
                newPool.poolingKey = key;
                newPool.deactiveList.Add(obj);
                poolingDict.Add(key, newPool);
            }
        }
        public void ClearPool()
        {
            if(poolingDict != null)
            {
                foreach (KeyValuePair<string, Pool> item in poolingDict)
                {
                    List<PoolingObject> aList = item.Value.activeList;
                    int count = aList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        aList[0].Despawn();
                    }
                    List<PoolingObject> dList = item.Value.deactiveList;
                    for (int i = 0; i < dList.Count; i++)
                    {
                        Destroy(dList[i].gameObject);
                    }
                }
            }
            DebugCustom.Log("Clear Poll");
            poolingDict = new Dictionary<string, Pool>();
        }
        public void DestroyGameObject(GameObject obj)
        {
            Destroy(obj);
        }

        public Sprite GetSpriteItem(EItemInGame type)
        {
            switch (type)
            {
                case EItemInGame.Small_Coin:
                    return iconSmallCoin;
                case EItemInGame.Normal_Coin:
                    return iconNormalCoin;
                case EItemInGame.Big_Coin:
                    return iconBigCoin;
                case EItemInGame.Magnetic:
                    return iconMagnetic;
                case EItemInGame.Bomb:
                    return iconBomb;
                case EItemInGame.Heal:
                    return iconHeal;
                case EItemInGame.Boss_Chest:
                    return iconBossChest;
                case EItemInGame.EndlessKey:
                    return iconKey;
                default:
                    return null;
            }
        }
        public Sprite GetSpriteExp(int type)
        {
            if (type <= 5)
                return iconSmallExp;
            else if (type <= 10)
                return iconNormalExp;
            else if (type <= 20)
                return iconLargeExp;
            else 
                return iconHugeExp;
        }
        public int GetSortingOder(EItemInGame type)
        {
            switch (type)
            {
                case EItemInGame.Small_Coin:
                    return coinSO;
                case EItemInGame.Normal_Coin:
                    return coinSO;
                case EItemInGame.Big_Coin:
                    return coinSO;
                case EItemInGame.Magnetic:
                    return itemSO;
                case EItemInGame.Bomb:
                    return itemSO;
                case EItemInGame.Heal:
                    return itemSO;
                case EItemInGame.Exp:
                    return expSO;
                case EItemInGame.Boss_Chest:
                    return bossChestSO;
                case EItemInGame.EndlessKey:
                    return itemSO;
                default:
                    break;
            }
            return 0;
        }
        public void OnUpdate(float t)
        {
            for (int i = 0; i < listItemInGames.Count; i++)
            {
                listItemInGames[i].OnUpdate(t);
            }
        }
    }
}
