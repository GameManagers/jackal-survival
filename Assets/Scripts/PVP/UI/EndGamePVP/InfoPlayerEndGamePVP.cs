using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WE.Manager;

namespace WE.UI.PVP.EndGame
{
    public class InfoPlayerEndGamePVP : MonoBehaviour
    {
        [SerializeField]
        private Image imgAvatar, imgRank;
        [SerializeField]
        private Text lbPlayerName;
        [SerializeField]
        private TextMeshProUGUI lbRanking, valueCurrentBattlePoint;

        public void SetAvatar(TypeAvatar avatar)
        {
            Sprite _avatar = SpriteManager.Instance.GetSpriteAvatar(avatar);
            imgAvatar.sprite = _avatar;
            imgAvatar.SetNativeSize();
        }
        public void SetName(string _name)
        {
            lbPlayerName.text = _name;
        }

        public void SetRank(ERankPVP _rank)
        {
            lbRanking.text = "Test";
            imgRank.sprite = SpriteManager.Instance.GetSpriteRankingPVP(_rank);
        }

        public void SetBattlePoint(int currentPoint)
        {
            valueCurrentBattlePoint.text = currentPoint.ToString();
        }
    }
}
