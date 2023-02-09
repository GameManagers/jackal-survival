using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using WE.Manager;
using WE.PVP.Manager;
using WE.Support;
using WE.Utils;

namespace WE.UI.PVP
{
    public class UIPopupPVP : UIBase
    {
        [SerializeField]
        private ContentSizeFitter contentSizeFitter;

        [SerializeField]
        private ElementRankingPVP currentRankingPvp;
        [SerializeField]
        private ElementRankingPVP prefabsRanking;
        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private Transform parentTransform;
        [SerializeField]
        private PVPMatchingController _pvpMatchingController;
        [SerializeField]
        private GameObject notifyNotUserInLeaderboard, notifyUserNotPlayPvp;
        public Button PlayPVPButton;
        [SerializeField] private Text _txtRemainTime;
        private List<ElementRankingPVP> lstElementRankingPvp;

        public override void InitUI()
        {
            EventManager.StartListening(Constant.GAME_TICK_EVENT, OnTick);
        }

        public void InitPopupPVP(DataPVPRanking info) {
            PVPManager.Instance.InitTimeRemain(info.EndTime, info.CurrentTime);
            OnTick();
            LoadDataRanking(info);
        }

        private void OnTick()
        {
            Debug.Log("on tick ui");
            DebugCustom.Log("on tick ui", PVPManager.Instance.TimeRemain, PVPManager.Instance.TimeRemain.TotalSeconds);

            if (PVPManager.Instance.TimeRemain.TotalSeconds >= 0)
                _txtRemainTime.text = Helper.ConvertTimerLongTime(PVPManager.Instance.TimeRemain.TotalSeconds);
            else
            {
                _txtRemainTime.text = "0";
            }
        }


        private void LoadDataRanking(DataPVPRanking data)
        {
            if (data == null)
                return;
            if (data.YourScore != null)
            {
                Context.PvPBattlePoint = data.YourScore.Score;
                if (data.YourScore.RankNumber < 0)
                {
                    notifyUserNotPlayPvp.SetActive(true);
                    currentRankingPvp.gameObject.SetActive(false);
                } else
                {

                }
            }
            if (!data.TopUser.IsNullOrEmpty())
            {
                notifyNotUserInLeaderboard.SetActive(false);
            }
            else
            {
                notifyNotUserInLeaderboard.SetActive(true);
            }
        }

        public void StartPVP()
        {
            DebugCustom.LogColor("Matching UI");
            _pvpMatchingController?.Matching();
 
            base.Hide();
        }

        public override void AfterHideAction()
        {
            base.AfterHideAction();
            EventManager.StopListening(Constant.GAME_TICK_EVENT, OnTick);
        }
        public void OnDisable()
        {
            lstElementRankingPvp = null;
        }
    }
}