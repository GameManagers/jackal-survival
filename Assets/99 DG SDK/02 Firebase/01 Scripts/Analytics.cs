using System;
using UnityEngine;
using System.Collections.Generic;
using Firebase.Analytics;
using AppsFlyerSDK;
namespace Dragon.SDK
{
    public partial class Analytics
    {
        #region ACTION
        public const string level_start = "level_start";
        public const string level_win = "level_win";
        public const string level_lose = "level_lose";
        public const string rewarded_video_show = "rewarded_video_show";
        public const string Interstitial_show = "Interstitial_show";
        public const string coin_earn = "coin_earn";
        public const string coin_spent = "coin_spent";
        public const string in_app_purchase = "in_app_purchase";
        public const string level_reach = "level_reach";
        public const string days_playing = "days_playing";
        public const string total_spent = "total_spent";
        public const string total_earn = "total_earn";
        #endregion
        public static bool IsLog()
        {
            if (!SDKDGManager.Instance.FirebaseManager.FirebaseInitialized) return false;
            return true;
        }
        public static void LogEventByName(string name)
        {
            if (!IsLog())
                return;
            FirebaseAnalytics.LogEvent(name);
        }
        public static void SetUserProperty(string key, string value)
        {
            if (!IsLog())
                return;
            FirebaseAnalytics.SetUserProperty(key, value);
        }
        #region CASUAL
        public static void LogLevelStart(int level)
        {
            if (!IsLog())
                return;
            FirebaseAnalytics.LogEvent(level_start, "level", level);
        }
        public static void LogLevelWin(int level, float time)
        {
            if (!IsLog())
                return;
            Parameter[] parameters = {
             new Parameter("level", level),
             new Parameter("time", time),
             };
            FirebaseAnalytics.LogEvent(level_win, parameters);
        }
        public static void LogLevelLose(int level, float time)
        {
            if (!IsLog())
                return;
            Parameter[] parameters = {
                new Parameter("level", level),
                new Parameter("time", time),
             };
            FirebaseAnalytics.LogEvent(level_lose, parameters);
        }
        public static void LogCoinEarn(int value, string position)
        {
            if (!IsLog())
                return;
            Parameter[] parameters = {
                new Parameter("value", value),
                new Parameter("position", position),
             };
            FirebaseAnalytics.LogEvent(coin_earn, parameters);
        }
        public static void LogCoinSpent(int value, string position, string item)
        {
            if (!IsLog())
                return;
            Parameter[] parameters = {
                new Parameter("value", value),
                new Parameter("position", position),
                new Parameter("item", item),
             };
            FirebaseAnalytics.LogEvent(coin_spent, parameters);
        }
        public static void LogInappPurcahse(string package, string amount)
        {
            if (!IsLog())
                return;
            Parameter[] parameters = {
                new Parameter("package", package),
                new Parameter("amount", amount),
             };
            FirebaseAnalytics.LogEvent(coin_spent, parameters);
        }
        public static void LogRewardedVideoShow(int level, string placement)
        {
            if (!IsLog())
                return;
            Parameter[] parameters = {
                new Parameter("level", level),
                new Parameter("placement", placement),
             };
            FirebaseAnalytics.LogEvent(rewarded_video_show, parameters);
            AppsflyerFilter(rewarded_video_show, true);
        }
        public static void LogInterstitialShow(int level, string placement)
        {
            if (!IsLog())
                return;
            Parameter[] parameters = {
                new Parameter("level", level),
                new Parameter("placement", placement),
             };
            FirebaseAnalytics.LogEvent(Interstitial_show, parameters);
            AppsflyerFilter(Interstitial_show, true);
        }
        #endregion
        #region REVENUE ADS
        public static void SendEvent(ImpressionData data, AdFormat type, string appsflyerID, string firebaseID, string package_name)
        {
            SendEventRealtime(data, type);
            if (type == AdFormat.interstitial)
                SendEventThresholdInterstitial(data);
            SendEventServer(data, type, appsflyerID, firebaseID, package_name);
        }
        private static void SendEventServer(ImpressionData data, AdFormat type, string appsflyerID, string firebaseID, string package_name)
        {
//            var form = new WWWForm(); //here you create a new form connection

//#if UNITY_EDITOR
//            form.AddField("platform", 2);
//#elif UNITY_IOS
//            form.AddField("platform", 1);
//#else
//            form.AddField("platform", 0);
//#endif
//            form.AddField("packagename", package_name);
//            form.AddField("ad_platform", "applovin");
//            form.AddField("ad_source", data.NetworkName);
//            form.AddField("ad_unit_name", data.AdUnitIdentifier);
//            form.AddField("ad_format", data.AdFormat);
//            form.AddField("currency", "USD");
//            form.AddField("value", data.Revenue.ToString());
//            form.AddField("appsflyer_id", appsflyerID);
//            form.AddField("firebase_id", firebaseID);
//            //send
//            ObservableWWW.Post("http://analytics.rocketstudio.com.vn:2688/api/firebase_analystic", form).Subscribe(
//                x =>
//                {
//                    Debug.Log("SendAnalystic Done");
//                }, // onSuccess
//                ex =>
//                {
//                    Debug.Log("SendAnalystic ex" + ex.Message);
//                    //MobileNativeMessage msg = new MobileNativeMessage("TAPP.vn", "Có lỗi xảy ra");
//                }// onError
//            );
        }
        private static void SendEventRealtime(ImpressionData data, AdFormat type)
        {
            if (!IsLog())
                return;
            Parameter[] AdParameters = {
             new Parameter("ad_platform", "applovin"),
             new Parameter("ad_source", data.NetworkName),
             new Parameter("ad_unit_name", data.AdUnitIdentifier),
             new Parameter("currency","USD"),
             new Parameter("value",data.Revenue),
             new Parameter("placement",data.Placement),
             new Parameter("country_code",data.CountryCode),
             new Parameter("ad_format",data.AdFormat),
             };
            FirebaseAnalytics.LogEvent("ad_impression_rocket", AdParameters);
        }
        private static void SendEventThresholdInterstitial(ImpressionData data)
        {
            if (!IsLog())
                return;
            AdFormat adFormat = AdFormat.interstitial;
            var rev = GetRevenueCache(adFormat);
            rev += data.Revenue;
            var time = GetTimeLogin(adFormat);
            bool isMaxDay = CheckConditionDay(time, FireBaseRemoteConfig.GetIntConfig("config_max_day_send_revenue", 1));

            if (rev >= FireBaseRemoteConfig.GetFloatConfig("min_value_revenue", 1) || isMaxDay)
            {
                // send event
                Parameter[] AdParameters = {
                    new Parameter("ad_platform", "applovin"),
                    new Parameter("ad_source", data.NetworkName),
                    new Parameter("ad_unit_name", data.AdUnitIdentifier),
                    new Parameter("currency","USD"),
                    new Parameter("value",rev),
                    new Parameter("placement",data.Placement),
                    new Parameter("country_code",data.CountryCode),
                    new Parameter("ad_format",data.AdFormat),
                      };
                FirebaseAnalytics.LogEvent("Interstitial_threshold", AdParameters);
                AppsflyerFilter("Interstitial_threshold", true);
                SetRevenueCache(adFormat, 0);
                SetTimeLogin(adFormat, DateTime.Now.ToString());
            }
            else
            {
                SetRevenueCache(adFormat, rev);
            }
        }
        private static double GetRevenueCache(AdFormat type)
        {
            return PlayerPrefs.GetFloat("revenueAd" + type, 0);
        }
        private static void SetRevenueCache(AdFormat type, double rev)
        {
            PlayerPrefs.SetFloat("revenueAd" + type, (float)rev);
        }
        private static bool CheckConditionDay(string stringTimeCheck, int maxDays)
        {
            if (string.IsNullOrEmpty(stringTimeCheck))
            {
                return false;
            }
            try
            {
                DateTime timeNow = DateTime.Now;
                DateTime timeOld = DateTime.Parse(stringTimeCheck);
                DateTime timeOldCheck = new DateTime(timeOld.Year, timeOld.Month, timeOld.Day, 0, 0, 0);
                long tickTimeNow = timeNow.Ticks;
                long tickTimeOld = timeOldCheck.Ticks;

                long elapsedTicks = tickTimeNow - tickTimeOld;
                TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
                double totalDay = elapsedSpan.TotalDays;

                if (totalDay >= maxDays)
                {
                    return true;
                }
            }
            catch
            {
                return true;
            }

            return false;
        }
        private static string GetTimeLogin(AdFormat type)
        {
            return PlayerPrefs.GetString("time_login_check_rev" + type, DateTime.Now.ToString());
        }
        private static void SetTimeLogin(AdFormat type, string time)
        {
            PlayerPrefs.SetString("time_login_check_rev" + type, time);
        }
        #endregion
        #region Appsflyer Filter
        private static void AppsflyerFilter(string key, bool multi = false)
        {
            if (multi)
            {
                Dictionary<string, string> paramas = new Dictionary<string, string>();
                paramas.Add(AFInAppEvents.QUANTITY, "1");
                paramas.Add("a_rocket", "1");
                AppsFlyer.sendEvent(key, paramas);
                return;
            }
            if (!IS_Pushed(key))
            {

                Dictionary<string, string> paramas = new Dictionary<string, string>();
                paramas.Add(AFInAppEvents.QUANTITY, "1");
                paramas.Add("a_rocket", "1");
                AppsFlyer.sendEvent(key, paramas);
                Temp_Save(key);
            }
            //}
        }

        private static void Temp_Save(string key)
        {
            PlayerPrefs.SetInt("af_" + key, 1);
            PlayerPrefs.Save();
        }

        private static bool IS_Pushed(string key)
        {
            return PlayerPrefs.HasKey("af_" + key);
        }

        #endregion Appsflyer Filter
    }
}
public class ImpressionData
{
    public string CountryCode;
    public string NetworkName;
    public string AdUnitIdentifier;
    public string Placement;
    public double Revenue;
    public string AdFormat;

}
public enum AdFormat
{
    interstitial,
    video_rewarded,
    rewarded_interstitial,
}