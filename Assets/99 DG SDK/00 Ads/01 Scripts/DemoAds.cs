using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dragon.SDK;

public class DemoAds : MonoBehaviour
{
    public void ShowAdsReward()
    {
        AdsManager.Instance.ShowRewardedAd(() =>
        {
            Debug.Log("AdsReward");
        }, "Test");
    }
    public void ShowInterstitial()
    {
        AdsManager.Instance.ShowInterstitial("Test");
    }

    public void ShowBanner()
    {
        AdsManager.Instance.ShowBanner();
    }
    public void HideBanner()
    {
        AdsManager.Instance.HideBanner();
    }
    public void ShowRewardedInterstitialAd()
    {
        AdsManager.Instance.ShowRewardedInterstitialAd(() =>
        {
            Debug.Log("RewardedInterstitialAd");
        }, "Test");
    }
    public void Purchase()
    {
        SDKDGManager.Instance.IAPManager.Purchase("com.test", () =>
        {
            Debug.Log("Pruchase Success");
        });
    }
}
