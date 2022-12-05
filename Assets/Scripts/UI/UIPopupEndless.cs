using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WE.Unit;
using WE.Manager;
using WE.Support;
using Dragon.SDK;

namespace WE.UI
{
    public class UIPopupEndless : UIBase
    {
        public GameObject buttonAds;
        public GameObject buttonKeys;
        public TextMeshProUGUI textKeyCount;

        public TextMeshProUGUI textTimer;
        public TextMeshProUGUI textKill;
        public override void InitUI()
        {
            buttonKeys.SetActive(Player.Instance.EndlessKeyCount >= 1);
            buttonAds.SetActive(Player.Instance.EndlessKeyCount < 1);
            textKeyCount.text = Player.Instance.EndlessKeyCount.ToString();
            textTimer.text = Helper.ConvertTimer(Player.Instance.GetRecordTimeEndless());
            textKill.text = Player.Instance.GetRecordKillEndless().ToString();
        }
        public void OnAdsClick()
        {
            AdsManager.Instance.ShowRewardedAd(() => {
                Hide();
                GameplayManager.Instance.StartGame(GameType.Endless);
            }, Analytics.rewarded_video_show);
        }
        public void OnKeyClick()
        {
            Hide();
            Player.Instance.ConsumeKey();
            GameplayManager.Instance.StartGame(GameType.Endless);
        }
        public void HackKey()
        {
            Player.Instance.AddEndlessKey(1);
            InitUI();
        }
    }
}

