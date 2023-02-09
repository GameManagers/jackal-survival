using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WE.Manager;

namespace WE.UI.PVP.EndGame
{
    public class FrameResultEndGamePVP : MonoBehaviour
    {

        [SerializeField]
        private TextMeshProUGUI lbTitleResult;
        [SerializeField]
        private TextMeshProUGUI pvpScore;
        [SerializeField]
        private InfoPlayerEndGamePVP infoPlayer;
        [SerializeField]
        private RewardsEndGamePVP rewardsEndGame;
        [SerializeField]
        private Text lbTextReasonLose;

        public void SetEmptyTitle()
        {
            lbTitleResult.text = string.Empty;
        }

        public void SetTitle(string title)
        {
            lbTitleResult.text = title;
        }
        public void SetRank(ERankPVP _rankPVP)
        {
            infoPlayer.SetRank(_rankPVP);
        }

        public void SetPlayerAvatar()
        {
            infoPlayer.SetAvatar(TypeAvatar.Avatar4);

        }

        public void ShowInfo(EndGameData message)
        {
            infoPlayer.SetAvatar(message.AvatarUrl);      
            infoPlayer.SetName(message.DisplayName);
            pvpScore.text = message.Score.ToString();
            ERankPVP _rankPVP = DataManager.Instance.dataRewardPVP.GetRankPvp(message.eloCur);
            infoPlayer.SetRank(_rankPVP);
            SetRewards(message);
        }

        public void SetRewards(EndGameData data)
        {
            rewardsEndGame.Dispose();
            rewardsEndGame.InitRewards(data.Rewards);
            if (data.eloPre != data.eloCur)
            {
                bool isUp = data.eloPre < data.eloCur;
                string _point = (data.eloCur - data.eloPre).ToString();
                rewardsEndGame.SetArrow(isUp);
                rewardsEndGame.SetBattlePoint(_point);
            }
            else
            {
                rewardsEndGame.SetBattlePoint("0");
                rewardsEndGame.SetColorText(true);
            }
        }

        public void SetBattlePoint(string point, bool isUp)
        {
            rewardsEndGame.SetArrow(isUp);
            rewardsEndGame.SetBattlePoint(point);
        }

        public void SetColorBattlePoint(bool isWin)
        {
            rewardsEndGame.SetColorText(isWin);
        }

        public void SetScore(int score)
        {
            pvpScore.text = score.ToString();
        }
        public void SetName(string _name)
        {
            infoPlayer.SetName(_name);
        }

        public void SetCurrentBattlePoint(int currentPoint)
        {
          //  infoPlayer.SetBattlePoint(currentPoint);
        }
        public void SetTitleLose(EWinTypePVP eWin, bool isCurrentPlayer)
        {
            if (lbTextReasonLose != null)
            {
                string message = string.Empty;
                switch (eWin)
                {
                    case EWinTypePVP.DIE:
                        message = isCurrentPlayer ? "You died" : "Opponent die";
                        break;
                    case EWinTypePVP.SCORE:
                        message = isCurrentPlayer ? "You lose score to your opponent" : "You score more than your opponent";
                        break;
                    case EWinTypePVP.DISCONNECT:
                        message = isCurrentPlayer ? "Check network before play" : "Opponent disconnect";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
                lbTextReasonLose.text = message;
            }

        }
    }
}
