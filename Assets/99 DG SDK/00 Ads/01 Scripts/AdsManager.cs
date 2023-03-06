using AppsFlyerSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Manager;
using WE.Unit;
using WE.Utils;

namespace Dragon.SDK
{
    [Flags]
    public enum TypeAdsMax
    {
        Inter = 1 << 0,
        Banner = 1 << 1,
        Reward = 1 << 2,
        Inter_Reward = 1 << 3,
        MRec = 1 << 4,
        AOA = 1 << 5
    }
    public class AdsManager : MonoBehaviour
    {
        public TypeAdsMax TypeAdsUse;

        public static AdsManager Instance => _instance;
        public AppOpenAdManager AppOpenAdManager;
        public string MaxSdkKey = "";
        public string InterstitialAdUnitId = "";
        public string RewardedAdUnitId = "";
        public string RewardedInterstitialAdUnitId = "";
        public string BannerAdUnitId = "";
        public string MRecAdUnitId = "";

        private static AdsManager _instance;
        private Action _actionReward;
        private Action _actionHideInterAds;

        private bool _isBannerShowing;
        private bool _isMRecShowing;
        private int _interstitialRetryAttempt;
        private int _rewardedRetryAttempt;
        private int _rewardedInterstitialRetryAttempt;
        private string _appsflyerID;
        bool _isInited = false;


        public void InitInfo()
        {
            StartCoroutine(RetryInit());
        }

        private IEnumerator RetryInit()
        {
            while (!_isInited)
            {
                if (Application.internetReachability != NetworkReachability.NotReachable)
                {
                    _isInited = true;
                    Init();
                }
                else
                {
                    yield return new WaitForSeconds(2);
                }

            }
            yield return null;
        }
        
        /**
         * comment no ads
         */

        private void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                if (!AppOpenAdManager.ResumeFromAds)
                {
                //    AppOpenAdManager.ShowAdIfReady();
                }
            }

        }
        private void Init()
        {
            _instance = this;
            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
               
                if (TypeAdsUse.HasFlag(TypeAdsMax.Inter))
                 // InitializeInterstitialAds();

                if (TypeAdsUse.HasFlag(TypeAdsMax.Reward))
                 // InitializeRewardedAds();

                if (TypeAdsUse.HasFlag(TypeAdsMax.Inter_Reward))
                 // InitializeRewardedInterstitialAds();

                if (TypeAdsUse.HasFlag(TypeAdsMax.Banner))
                  //InitializeBannerAds();

                if (TypeAdsUse.HasFlag(TypeAdsMax.MRec))
                  //InitializeMRecAds();

                if (TypeAdsUse.HasFlag(TypeAdsMax.AOA))
                {
                //  InitializeAOA();
                }
                //MaxSdk.ShowMediationDebugger();
            };
            MaxSdk.SetUserId(AppsFlyer.getAppsFlyerId());
            MaxSdk.SetSdkKey(MaxSdkKey);
            MaxSdk.InitializeSdk();
            _appsflyerID = AppsFlyer.getAppsFlyerId();
        }
        #region Interstitial Ad Methods
        public void ShowInterstitial(string placement, Action actionHide = null)
        {
            _actionHideInterAds = actionHide;
            if (IsLoadInterstitial())
            {
                AppOpenAdManager.ResumeFromAds = true;
                MaxSdk.ShowInterstitial(InterstitialAdUnitId, placement);
                Analytics.LogInterstitialShow(Player.Instance.CurrentMap, placement);
                //if (GameplayManager.Instance.State == GameState.Gameplay)
                //{
                //    TimerSystem.Instance.StopTimeScale();
                //}
            }
            else
            {
                _actionHideInterAds?.Invoke();
            }
        }
        private void InitializeInterstitialAds()
        {
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            LoadInterstitial();
        }
        private void LoadInterstitial()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return;
            if (IsLoadInterstitial())
                return;
            MaxSdk.LoadInterstitial(InterstitialAdUnitId);
        }
        private bool IsLoadInterstitial()
        {
            return MaxSdk.IsInterstitialReady(InterstitialAdUnitId);
        }
        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Reset retry attempt
            _interstitialRetryAttempt = 0;
        }
        private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            _interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _interstitialRetryAttempt));
            Invoke("LoadInterstitial", (float)retryDelay);
            //_actionHideInterAds?.Invoke();
        }
        private void OnInterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. We recommend loading the next ad
            //DebugCustom.Log("Interstitial failed to display with error code: " + errorCode);
            LoadInterstitial();
            //_actionHideInterAds?.Invoke();
        }
        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad
            //DebugCustom.Log("Interstitial dismissed");
            LoadInterstitial();
            //TimerSystem.Instance.ReturnTimeScale();
            AppOpenAdManager.ResumeFromAds = false;
            _actionHideInterAds?.Invoke();
        }
        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Reset retry attempt
            _interstitialRetryAttempt = 0;
            //if (_actionReward != null)
            //    _actionReward();
            AppsFlyer.sendEvent("event_interstitial_ad_clicked", new Dictionary<string, string>() { { "interstitial_ad_clicked", "interstitial_ad_clicked" } });
        }
        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Reset retry attempt
            //interstitialRetryAttempt = 0;  
            //Debug.Log("InterstitialDisplayedEvent");
            AppsFlyer.sendEvent("event_interstitial_ad_impression", new Dictionary<string, string>() { { "event_interstitial_ad_impression", "event_interstitial_ad_impression" } });
        }
        private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            //Interstitial ad revenue paid. Use this callback to track user revenue.
            double revenue = adInfo.Revenue;
            // Miscellaneous data
            string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
            var data = new ImpressionData();
            data.AdFormat = "interstitial";
            data.AdUnitIdentifier = adUnitIdentifier;
            data.CountryCode = countryCode;
            data.NetworkName = networkName;
            data.Placement = placement;
            data.Revenue = revenue;
            Analytics.SendEvent(data, AdFormat.interstitial, _appsflyerID, FirebaseManager.FirebaseID, DragonConfig.PackageName);
        }
        #endregion
        #region Rewarded Ad Methods
        /// <summary>
        ///  actionError is Action with ads return status, 0 is No internet conection, 1 is no ads available;
        /// </summary>
        public void ShowRewardedAd(System.Action actionReward, string placement, Action<int> actionError = null)
        {
            if (Constant.IS_TESTER_JACKAL)
            {
                actionReward?.Invoke();
                return;
            }
            _actionReward = actionReward;
            if (actionError == null)
            {
                actionError = (int i) => { UIManager.Instance.ShowTextAds(i); };
            }
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                actionError?.Invoke(0);
                return;
            }
            if (IsLoadRewardedAd())
            {
                AppOpenAdManager.ResumeFromAds = true;
                ShowRewardedAd(placement);
                Analytics.LogRewardedVideoShow(Player.Instance.CurrentMap, placement);
            }
            else
            {
                actionError?.Invoke(1);
            }
        }
        private void InitializeRewardedAds()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            // Load the first RewardedAd
            LoadRewardedAd();
        }
        private void LoadRewardedAd()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return;
            if (IsLoadRewardedAd())
                return;
            MaxSdk.LoadRewardedAd(RewardedAdUnitId);
        }
        private bool IsLoadRewardedAd()
        {
            return MaxSdk.IsRewardedAdReady(RewardedAdUnitId);
        }
        private void ShowRewardedAd(string placeId)
        {
            if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
                MaxSdk.ShowRewardedAd(RewardedAdUnitId, placeId);
        }
        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Reset retry attempt
            _rewardedRetryAttempt = 0;
        }
        private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            _rewardedRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _rewardedRetryAttempt));
            Invoke("LoadRewardedAd", (float)retryDelay);
        }
        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. We recommend loading the next ad
            LoadRewardedAd();
        }
        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            //Debug.Log("Rewarded ad displayed");
        }
        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            AppsFlyer.sendEvent("event_video_reward_clicked", new Dictionary<string, string>() { { "clicked_video", "clicked_video" } });
        }
        private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            //Debug.Log("Rewarded ad dismissed");
            LoadRewardedAd();
            AppOpenAdManager.ResumeFromAds = false;
        }
        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad was displayed and user should receive the reward
            if (_actionReward != null)
                _actionReward();
        }
        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad revenue paid. Use this callback to track user revenue.
            //Debug.Log("Rewarded ad revenue paid");

            // Ad revenue
            //double revenue = adInfo.Revenue;

            //// Miscellaneous data
            //string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            //string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            //string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            //string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

            //var data = new ImpressionData();
            //data.AdFormat = "video_reward";
            //data.AdUnitIdentifier = adUnitIdentifier;
            //data.CountryCode = countryCode;
            //data.NetworkName = networkName;
            //data.Placement = placement;
            //data.Revenue = revenue;
            //Analytics.SendEvent(data, AdFormat.video_rewarded);
        }
        #endregion
        #region Rewarded Interstitial Ad Methods
        private void InitializeRewardedInterstitialAds()
        {
            // Attach callbacks
            MaxSdkCallbacks.RewardedInterstitial.OnAdLoadedEvent += OnRewardedInterstitialAdLoadedEvent;
            MaxSdkCallbacks.RewardedInterstitial.OnAdLoadFailedEvent += OnRewardedInterstitialAdFailedEvent;
            MaxSdkCallbacks.RewardedInterstitial.OnAdDisplayFailedEvent += OnRewardedInterstitialAdFailedToDisplayEvent;
            MaxSdkCallbacks.RewardedInterstitial.OnAdDisplayedEvent += OnRewardedInterstitialAdDisplayedEvent;
            MaxSdkCallbacks.RewardedInterstitial.OnAdClickedEvent += OnRewardedInterstitialAdClickedEvent;
            MaxSdkCallbacks.RewardedInterstitial.OnAdHiddenEvent += OnRewardedInterstitialAdDismissedEvent;
            MaxSdkCallbacks.RewardedInterstitial.OnAdReceivedRewardEvent += OnRewardedInterstitialAdReceivedRewardEvent;
            MaxSdkCallbacks.RewardedInterstitial.OnAdRevenuePaidEvent += OnRewardedInterstitialAdRevenuePaidEvent;
            // Load the first RewardedInterstitialAd
            LoadRewardedInterstitialAd();
        }
        private void LoadRewardedInterstitialAd()
        {
            if (IsRewardedInterstitialAdReady())
                return;
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return;
            MaxSdk.LoadRewardedInterstitialAd(RewardedInterstitialAdUnitId);
        }
        private bool IsRewardedInterstitialAdReady()
        {
            return MaxSdk.IsRewardedInterstitialAdReady(RewardedInterstitialAdUnitId);
        }
        public void ShowRewardedInterstitialAd(System.Action actionReward, string placement)
        {
            _actionReward = actionReward;
            if (IsRewardedInterstitialAdReady())
            {
                MaxSdk.ShowRewardedInterstitialAd(RewardedInterstitialAdUnitId, placement);
            }
        }
        private void OnRewardedInterstitialAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Reset retry attempt
            _rewardedInterstitialRetryAttempt = 0;
        }
        private void OnRewardedInterstitialAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            _rewardedInterstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _rewardedInterstitialRetryAttempt));
            Invoke("LoadRewardedInterstitialAd", (float)retryDelay);
        }
        private void OnRewardedInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded interstitial ad failed to display. We recommend loading the next ad
            LoadRewardedInterstitialAd();
        }
        private void OnRewardedInterstitialAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            //Debug.Log("Rewarded interstitial ad displayed");
        }
        private void OnRewardedInterstitialAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            //Debug.Log("Rewarded interstitial ad clicked");
        }
        private void OnRewardedInterstitialAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded interstitial ad is hidden. Pre-load the next ad
            //Debug.Log("Rewarded interstitial ad dismissed");
            LoadRewardedInterstitialAd();
        }
        private void OnRewardedInterstitialAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded interstitial ad was displayed and user should receive the reward
            //Debug.Log("Rewarded interstitial ad received reward");
            if (_actionReward != null)
                _actionReward();
        }
        private void OnRewardedInterstitialAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Ad revenue
            //double revenue = adInfo.Revenue;
            //// Miscellaneous data
            //string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            //string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            //string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            //string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

            //var data = new ImpressionData();
            //data.AdFormat = "rewarded_interstitial";
            //data.AdUnitIdentifier = adUnitIdentifier;
            //data.CountryCode = countryCode;
            //data.NetworkName = networkName;
            //data.Placement = placement;
            //data.Revenue = revenue;
            //Analytics.SendEvent(data, AdFormat.rewarded_interstitial);
        }
        #endregion
        #region Banner Ad Methods
        private void InitializeBannerAds()
        {
            // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
            // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
            MaxSdk.CreateBanner(BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

            // Set background or background color for banners to be fully functional.
            MaxSdk.SetBannerBackgroundColor(BannerAdUnitId, Color.black);
        }
        private void ToggleBannerVisibility()
        {
            if (!_isBannerShowing)
            {
                MaxSdk.ShowBanner(BannerAdUnitId);
            }
            else
            {
                MaxSdk.HideBanner(BannerAdUnitId);
            }

            _isBannerShowing = !_isBannerShowing;
        }
        public bool ShowBanner()
        {
            MaxSdk.ShowBanner(BannerAdUnitId);
            return true;
        }
        public void HideBanner()
        {
            MaxSdk.HideBanner(BannerAdUnitId);
        }
        #endregion
        #region MREC Ad Methods
        private void InitializeMRecAds()
        {
            // MRECs are automatically sized to 300x250.
            MaxSdk.CreateMRec(MRecAdUnitId, MaxSdkBase.AdViewPosition.BottomCenter);
        }
        private void ToggleMRecVisibility()
        {
            if (!_isMRecShowing)
            {
                MaxSdk.ShowMRec(MRecAdUnitId);
            }
            else
            {
                MaxSdk.HideMRec(MRecAdUnitId);
            }

            _isMRecShowing = !_isMRecShowing;
        }
        #endregion
        #region AOA
        private bool isFirstLoad =true;
        private void InitializeAOA()
        {
            isFirstLoad = true;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAppOpenLoadedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAppOpenLoadFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += OnAppOpenFailedToDisplayEvent;
            AppOpenAdManager.ShowAdIfReady();
        }
        public void OnAppOpenDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            MaxSdk.LoadAppOpenAd(AppOpenAdManager.AppOpenAdUnitId);
        }
        public void OnAppOpenLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("AOA Loaded");
            if (!isFirstLoad)
                return;
            isFirstLoad = false;
            AppOpenAdManager.ShowAdIfReady();
            LoadInterstitial();
            LoadRewardedAd();
        }
        private void OnAppOpenLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Debug.Log("AOA Load Failed");
            if (!isFirstLoad)
                return;
            isFirstLoad = false;

            LoadInterstitial();
            LoadRewardedAd();
        }
        private void OnAppOpenFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. We recommend loading the next ad
            Debug.Log("AOA ad failed to display with error code: " + errorInfo.Code);
            MaxSdk.LoadAppOpenAd(AppOpenAdManager.AppOpenAdUnitId);
        }
        #endregion
    }
}
