using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WE.Unit;

namespace WE.UI.PVP.EndGame
{
    public class RewardsEndGamePVP : MonoBehaviour
    {
        [SerializeField]
        private Text titleReward;
        [SerializeField]
        private Text lbBattlePoint;
        [SerializeField]
        private TextMeshProUGUI rewards;
        [SerializeField]
        private Color colorWin, colorLose;

        public void Dispose()
        {
            lbBattlePoint.text = string.Empty;
            rewards.gameObject.SetActive(false);
        }

        public void InitRewards(Dictionary<string, int> dictRewards)
        {
            if (dictRewards == null || dictRewards.Count == 0)
            {
                titleReward.text = string.Empty;
                return;
            }
            rewards.gameObject.SetActive(true);
            if(dictRewards["Gold"] > 0)
            {
                rewards.text = (dictRewards["Gold"]).ToString();
                Player.Instance.AddCoin(dictRewards["Gold"]);
            }
        }
        public void SetArrow(bool isUp)
        {
            lbBattlePoint.color = isUp ? colorWin : colorLose;
        }

        public void SetBattlePoint(string _battlePoint)
        {
            lbBattlePoint.text = _battlePoint;
        }

        public void SetColorText(bool isWin)
        {
            lbBattlePoint.color = isWin ? colorWin : colorLose;
        }
    }
}
