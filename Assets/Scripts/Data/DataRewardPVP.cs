using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WE.Data
{
    [CreateAssetMenu(fileName = "Data Reward PVP", menuName = "WE/Data/Data Reward PVP")]

    public class DataRewardPVP : SerializedScriptableObject
    {
        public Dictionary<ERankPVP, RankPVP> RankPVP;
        public List<RewardsTopPVP> RewardsTopPVP;
#if UNITY_EDITOR
        [Button("Get Data PVP")]

        public void GetDataPVP() {
           // RankPVP = new Dictionary<ERankPVP, RankPVP>();
           // RewardsTopPVP = new List<RewardsTopPVP>();
        }
#endif

        public ERankPVP GetRankPvp(int battlePoint)
        {
            ERankPVP _rank = ERankPVP.Bronze3;
            foreach (var item in RankPVP)
            {
                if (battlePoint >= item.Value.battlePoint)
                    _rank = item.Key;
            }
            return _rank;
        }

        public string GetLabelRank(ERankPVP eRank)
        {
            return RankPVP[eRank].name;
        }


        public int GetRewardsTopPVP(int rank)
        {

            for (int i = 0; i < RewardsTopPVP.Count; i++)
            {
                if (rank > RewardsTopPVP[i].rankMax)
                    continue;
                else
                    return RewardsTopPVP[i].gold;
            }
            return RewardsTopPVP[RewardsTopPVP.Count - 1].gold;
        }

    }

    [System.Serializable]
    public class RankPVP
    {
        [FoldoutGroup("$eRank")]
        public ERankPVP eRank;
        [FoldoutGroup("$eRank")]
        public string name;
        [FoldoutGroup("$eRank")]
        public int battlePoint;
    }

    public class RewardsTopPVP
    {
        public int rankMin;
        public int rankMax;
        public int gold;
    }
}

