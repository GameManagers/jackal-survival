using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using WE.Manager;
using WE.PVP;
using WE.PVP.Manager;

namespace WE.UI.PVP.EndGame
{
    public class UIEndGamePVP : UIBase
    {
        public GameObject RewardButton;
        public GameObject noThanks;
        public TextMeshProUGUI noThanksText;
        public FrameResultEndGamePVP frameWiner, frameLoser, frameEqual, frameDisconnectPVP;
        
        private EndGameData _dataPlayer = null;
        private EndGameData _dataOpponents = null;
        private EndGameMessage cachedMessage = null;
        private enum EResultPVP
        {
            Win = 0,
            Loser = 1,
            Equal = 2
        }

        private EResultPVP eResult;


        public override void InitUI()
        {
            frameWiner.gameObject.SetActive(false);
            frameLoser.gameObject.SetActive(false);

            frameDisconnectPVP.gameObject.SetActive(false);
        }

        public void ShowPopup(EndGameMessage message)
        {
            base.Show();
            cachedMessage = message;
            if (message.WinnerData.SessionId == PVPManager.Instance.DataPlayer.SessionId)
            {
                _dataPlayer = message.WinnerData;
                _dataOpponents = message.LoserData;
                eResult = EResultPVP.Win;
            }
            else
            {
                _dataPlayer = message.LoserData;
                _dataOpponents = message.WinnerData;
                eResult = EResultPVP.Loser;
            }
            if (message.IsDraw)
                eResult = EResultPVP.Equal;
            InitInfo();
        }

        private void InitInfo()
        {
            switch (eResult)
            {
                case EResultPVP.Win:
                    frameWiner.gameObject.SetActive(true);
                    frameLoser.gameObject.SetActive(true);
                    _dataPlayer.Score = PVPMode.Instance.PlayerScore;
                    frameWiner.ShowInfo(_dataPlayer);
                    frameWiner.SetCurrentBattlePoint(_dataPlayer.eloCur);
                    frameLoser.ShowInfo(_dataOpponents);
                    frameLoser.SetCurrentBattlePoint(_dataOpponents.eloCur);
                    frameWiner.SetTitle("You win");
                    frameLoser.SetTitle("Lose");
                    if (cachedMessage != null)
                        frameLoser.SetTitleLose(cachedMessage.WinType, false);
                    break;
                case EResultPVP.Loser:
                    frameWiner.gameObject.SetActive(true);
                    frameLoser.gameObject.SetActive(true);
                    Vector3 winerPosition = frameWiner.transform.position;
                    frameWiner.transform.position = frameLoser.transform.position;
                    frameLoser.transform.position = winerPosition;
                    if (cachedMessage != null && cachedMessage.WinType == EWinTypePVP.DIE)
                        _dataPlayer.Score = PVPMode.Instance.PlayerScore;
                    frameLoser.ShowInfo(_dataPlayer);
                    frameLoser.SetCurrentBattlePoint(_dataPlayer.eloCur);
                    frameWiner.ShowInfo(_dataOpponents);
                    frameWiner.SetCurrentBattlePoint(_dataOpponents.eloCur);
                    frameWiner.SetTitle("Win");
                    frameLoser.SetTitle("You lose");
                    if (cachedMessage != null)
                        frameLoser.SetTitleLose(cachedMessage.WinType, true);
                    break;
                case EResultPVP.Equal:
                    frameEqual.gameObject.SetActive(true);
                    frameLoser.gameObject.SetActive(true);
                    frameEqual.transform.position = frameWiner.transform.position;
                    frameLoser.ShowInfo(_dataOpponents);
                    frameEqual.ShowInfo(_dataPlayer);
                    frameLoser.SetCurrentBattlePoint(_dataOpponents.eloCur);
                    frameEqual.SetCurrentBattlePoint(_dataPlayer.eloCur);
                    frameEqual.SetTitle("Draw");
                    frameLoser.SetTitle("Draw");

                    frameEqual.SetColorBattlePoint(true);
                    frameLoser.SetColorBattlePoint(true);
                    break;
                default:
                    break;
            }
        }

        public void ShowFrameDisconnect()
        {
            frameWiner.gameObject.SetActive(false);
            frameLoser.gameObject.SetActive(false);

            frameDisconnectPVP.gameObject.SetActive(true);
            frameDisconnectPVP.SetPlayerAvatar();
            frameDisconnectPVP.SetTitle(string.Empty);
            frameDisconnectPVP.SetName(Context.CurrentUserPlayfabProfile.DisplayName);
            ERankPVP _rankPVP = DataManager.Instance.dataRewardPVP.GetRankPvp(Context.PvPBattlePoint);
            frameDisconnectPVP.SetRank(_rankPVP);
            base.Show();
        }
        public override void Hide()
        {
            base.Hide();
            _dataPlayer = null;
            _dataOpponents = null;
        }

        public void TakeRewardPVP()
        {
            GameplayManager.Instance.EndGame(true);
        }
    }

}
