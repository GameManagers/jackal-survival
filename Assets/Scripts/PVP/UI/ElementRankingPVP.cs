using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using WE.Manager;

namespace WE.UI.PVP
{
    public class ElementRankingPVP : MonoBehaviour
    {
        [SerializeField]
        private Image iconRanking;
        [SerializeField]
        private TextMeshProUGUI labelRanking;
        [SerializeField]
        private Text labelPlayerName;

        [SerializeField]
        private Text labelBattlePoint;
        [SerializeField]
        private Image background;
        [SerializeField]
        private Image iconAvatar;
        [SerializeField]
        private Color colorRankTop, colorRankTopNormal;
        [SerializeField]
        private Image iconRank;
        [SerializeField]
        private Sprite[] spriteCups;
        [SerializeField]
        private UIResourceItem[] uiRewards;
        public void SetInfo(int _ranking, string _userName, ERankPVP eRankPVP, int _battlePoint, TypeAvatar _avatar = 0)
        {
            SetRank(_ranking);
            SetBattlePoint(_battlePoint);
            SetUserName(_userName);
            SetAvatar(_avatar);
            SetRankPVP(eRankPVP);
            SetReward(_ranking);
        }
        public void SetRankPVP(ERankPVP eRankPVP)
        {
            //labelPlayerRanking.text = Utils.Utils.GetTranslation("lb_pvp_rank_" + eRankPVP.ToString());
            iconRank.sprite = SpriteManager.Instance.GetSpriteRankingPVP(eRankPVP);
        }

        public void SetColorRank(int _ranking)
        {
            background.color = _ranking < 4 ? colorRankTop : colorRankTopNormal;
        }

        public void SetAvatar(TypeAvatar avatar)
        {
            iconAvatar.sprite = SpriteManager.Instance.GetSpriteAvatar(avatar);
            iconAvatar.SetNativeSize();
        }

        /**
         * Set reward 
         */
        public void SetReward(int ranking)
        {
            var reward = DataManager.Instance.dataRewardPVP.GetRewardsTopPVP(ranking);
            uiRewards[0].gameObject.SetActive(true);
            uiRewards[0].SetValue(reward);
        }


        public void SetRank(int _ranking)
        {
            if (_ranking < 4)
            {
                iconRanking.gameObject.SetActive(true);
                labelRanking.text = string.Empty;
                iconRanking.sprite = spriteCups[_ranking - 1];
            }
            else
            {
                iconRanking.gameObject.SetActive(false);
                labelRanking.text = _ranking.ToString();
            }
        }
        public void SetUserName(string _userName)
        {
            if (_userName.Length > 17)
                _userName = _userName.Substring(0, 17) + "...";
            labelPlayerName.text = _userName;
        }

        public void SetBattlePoint(int _battlePoint)
        {
            labelBattlePoint.text = _battlePoint.ToString();
        }
    }
}


