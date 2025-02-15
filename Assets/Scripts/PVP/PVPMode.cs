using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using WE.Manager;
using WE.Pooling;
using WE.PVP.Manager;
using WE.UI.PVP;
using WE.Unit;

namespace WE.PVP
{
    public class PVPMode : MonoBehaviour
    {
        public static PVPMode _instance;
        public static PVPMode Instance => _instance;

        private bool isStartGame = false;
        private ObscuredInt playerScore, oppenentsScore;
        private UIInGamePVP uiGameplayPVP;
        public int PlayerScore => playerScore;
        private bool isDispose = false;

        private void Awake()
        {
            _instance = this;
            isDispose = false;
            PVPManager.Instance.Room.OnStartGamePVP += OnStartGamePVP;
            PVPManager.Instance.Room.OnEndGamePVP += OnEndGame_PVP;
            PVPManager.Instance.Room.OnHpChange += Opponents_HPChange;
            PVPManager.Instance.Room.OnScoreChange += Opponents_ChangeScore;
            PVPManager.Instance.Room.OnChangeTimePlay += OnChangeTimePlay;
            PVPManager.Instance.Room.OnAttackHide += OnAttackSkill;

            PVPManager.Instance.Room.OnEndGamePVPByDisconnect += OnEndGameByDisconnect;
            Player.Instance.OnHpChange += Current_OnHpChange;
        }

        private void Start()
        {
            DebugCustom.LogColorJson("Init PVP mode");
            UIManager.Instance.uIPopupPVP.Hide();
            uiGameplayPVP = UIManager.Instance.GetUIPVP();
        }

        private void OnStartGamePVP(StartGamePVPMessage data)
        {
            DebugCustom.LogColorJson("on start game pvp");

            if (!isStartGame)
            {
                DebugCustom.LogColor("OnStartGamePVP success");
                PVPManager.Instance.StateMachine.TriggerPlaying();
                GameplayManager.Instance.ActiveBattlePvp();
             //   GameplayManager.Instance.StartGame(GameType.PVP);
            }
        }

        private void OnDisable()
        {
            Dispose();
            _instance = null;
        }
        public void SetStartGame()
        {
            isStartGame = true;
        }

        private void OnChangeTimePlay(int _time)
        {
            uiGameplayPVP.UpdateTimePlay(_time);
        }

        private void OnAttackSkill(int type)
        {
            uiGameplayPVP.HideInfoOtherPlayer();
        }

        private void Dispose()
        {
            if (isDispose)
                return;
            StopAllCoroutines();

            PVPManager.Instance.Room.OnStartGamePVP -= OnStartGamePVP;
            PVPManager.Instance.Room.OnEndGamePVP -= OnEndGame_PVP;
            PVPManager.Instance.Room.OnScoreChange -= Opponents_ChangeScore;
            PVPManager.Instance.Room.OnHpChange -= Opponents_HPChange;
            PVPManager.Instance.Room.OnChangeTimePlay -= OnChangeTimePlay;
            PVPManager.Instance.Room.OnEndGamePVPByDisconnect -= OnEndGameByDisconnect;
            PVPManager.Instance.Room.OnAttackHide -= OnAttackSkill;

            if (GameplayManager.Instance != null)
            {
                Player.Instance.OnHpChange -= Current_OnHpChange;
            }

            isDispose = true;
        }

        public void UpdateCurrentScore(int playerScore)
        {
            // DebugCustom.LogColor("Score", playerScore);
            this.playerScore = playerScore;
            UpdateScore(this.PlayerScore, oppenentsScore);
        }
        private void Current_OnHpChange()
        {
            if (!Player.Instance.IsAlive) CurrenTank_OnUnitDie();
            uiGameplayPVP.UpdateHpBarCurrentPlayer(Player.Instance.CurrentHp, Player.Instance.MaxHp);
        }

        private void Opponents_ChangeScore(int _score, int _myScore)
        {
            UpdateOppenentsScore(_score, _myScore);
        }
        private void Opponents_HPChange(float _currentHp, float _maxHp)
        {
            uiGameplayPVP.Opponents_UpdateHpBar(_currentHp, _maxHp);
        }
        public void UpdateOppenentsScore(int _oppenentsScore, int _myScore)
        {
            oppenentsScore = _oppenentsScore;
            //playerScore = _myScore;
            if (playerScore != _myScore)
                SendScore();
            UpdateScore(this.playerScore, oppenentsScore);
        }

        private void UpdateScore(int playerScore, int opponentsScore)
        {
            uiGameplayPVP.UpdateScore(playerScore, opponetsScore: opponentsScore);
        }

        private void CurrenTank_OnUnitDie()
        {
            SendScore();
            PlayerDie();
        }

        public void SendLeaveRoom()
        {
            PVPManager.Instance.Room.LeaveRoom(false, true);
        }

        public void SendScore()
        {
            SendScorePVPMessage data = new SendScorePVPMessage();
            data.Score = playerScore;
            data.CurHP = Player.Instance.CurrentHp;
            data.MaxHP = Player.Instance.MaxHp;

            if(!Player.Instance.IsAlive)
            {
                if(PVPManager.Instance.StateMachine.CurrentState() == PVPSate.PLAYING)
                {
                    PlayerDie();
                }
            }
            PVPManager.Instance.Room.SendScore(data);
        }

        private void OnEndGame_PVP(EndGameMessage endGameMessage)
        {
            DebugCustom.LogColor("[PVP Mode] Receive Message End Game PVP");
            PVPManager.Instance.Room.LeaveRoom(false, false);

            Player.Instance.tankMovement.Stop();
            TimerSystem.Instance.StopTimeScale(1, () => {});

            UIManager.Instance.ShowPopupEndGamePVP(endGameMessage);

            if (endGameMessage.WinnerData.SessionId == PVPManager.Instance.DataPlayer.SessionId)
            {
                DebugCustom.LogColor("[PVP Mode] Receive Message End Game PVP", playerScore);
                UpdateScore(playerScore, endGameMessage.LoserData.Score);
            }
            else
            {
                DebugCustom.LogColor("[PVP Mode] Receive Message End Game PVP", endGameMessage.WinnerData.Score);

                UpdateScore(endGameMessage.LoserData.Score, endGameMessage.WinnerData.Score);
            }
            Dispose();
        }
        private void OnEndGameByDisconnect()
        {
            UIManager.Instance.ShowPopupPVPUserDisconnect();
        }
        public void PlayerDie()
        {
            PVPManager.Instance.Room.SendPlayerDie();
        }
    }

}
