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
            lstElementRankingPvp = new List<ElementRankingPVP>();
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
                    notifyUserNotPlayPvp.SetActive(false);
                    currentRankingPvp.gameObject.SetActive(true);
                    ERankPVP _currentRank = DataManager.Instance.dataRewardPVP.GetRankPvp(data.YourScore.Score);
                    currentRankingPvp.SetRank(data.YourScore.RankNumber);
                    currentRankingPvp.SetBattlePoint(data.YourScore.Score);
                    currentRankingPvp.SetUserName(Context.CurrentUserPlayfabProfile.DisplayName);
                    currentRankingPvp.SetAvatar(TypeAvatar.Avatar8);
                    currentRankingPvp.SetRankPVP(_currentRank);
                    currentRankingPvp.SetReward(data.YourScore.RankNumber);
                    DebugCustom.LogColorJson("Current ranking pvp", _currentRank);
                }
            }
            if (!data.TopUser.IsNullOrEmpty())
            {
                notifyNotUserInLeaderboard.SetActive(false);

                for(int i = 0; i < data.TopUser.Count; i++)
                {
                    InfoPVPRanking _infoRanking = data.TopUser[i];
                    int _score = _infoRanking.Score;
                    int _ranking = _infoRanking.RankNumber;
                    string _name = "abc";
                    TypeAvatar _avatar = TypeAvatar.Default;
                    if(_infoRanking.PlayerData != null)
                    {
                        _name = _infoRanking.PlayerData.DisplayName;
                        _avatar = _infoRanking.PlayerData.AvatarUrl;
                    }

                    ElementRankingPVP _element = null;
                    if (i > lstElementRankingPvp.Count - 1)
                    {
                        _element = Instantiate(prefabsRanking, parentTransform);
                        lstElementRankingPvp.Add(_element);
                    }
                    else
                    {
                        _element = lstElementRankingPvp[i];
                        _element.gameObject.SetActive(true);
                    }
                    _element.transform.localScale = Vector3.one;
                    _element.transform.localPosition = Vector3.zero;
                    _element.SetInfo(_ranking, _name, DataManager.Instance.dataRewardPVP.GetRankPvp(_score), _score, _avatar);
                    _element.SetColorRank(_ranking);
                }
                DebugCustom.LogColorJson("top user", lstElementRankingPvp.Count, data.TopUser.Count);
                if (data.TopUser.Count < lstElementRankingPvp.Count)
                {
                    for (int i = data.TopUser.Count; i < lstElementRankingPvp.Count; i++)
                        lstElementRankingPvp[i].gameObject.SetActive(false);
                }
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
 
          //  base.Hide();
        }

        public override void AfterHideAction()
        {
            Debug.Log("pvp popup hide");
            base.AfterHideAction();
            lstElementRankingPvp = null;
            EventManager.StopListening(Constant.GAME_TICK_EVENT, OnTick);
        }
        public void OnDisable()
        {
            Debug.Log("on disable popup pvp");
        }
    }
}