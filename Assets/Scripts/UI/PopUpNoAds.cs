using Dragon.SDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Unit;

namespace WE.UI
{
    public class PopUpNoAds : UIBase
    {
        public GameObject[] iconOn;
        public override void InitUI()
        {
            OnUpdateUI();
        }
        public void OnUpdateUI()
        {
            int adsCount = Player.Instance.GetNoAdsCount();
            if (adsCount >= 3)
            {
                Player.Instance.StartNoAds();
                Hide();
                return;
            }
            for (int i = 0; i < iconOn.Length; i++)
            {
                iconOn[i].SetActive(false);
            }
            for (int i = 0; i < adsCount; i++)
            {
                iconOn[i].SetActive(true);
            }
        }
        public void OnWatchAds()
        {
            AdsManager.Instance.ShowRewardedAd(() => {
                Player.Instance.AddNoAdsCount();
                OnUpdateUI();
            }, Analytics.rewarded_video_show);
        }
    }
}

