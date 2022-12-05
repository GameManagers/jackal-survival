using Firebase.RemoteConfig;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
namespace Dragon.SDK
{
    public class FireBaseRemoteConfig
    {
        public static bool IsFetchDataNow
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }
        public static bool IsInitSuccess
        {
            get
            {
                return _isInitSuccess;
            }
        }
        private static bool _isInitSuccess = false;
        public static void FetchData()
        {
            _isInitSuccess = true;
            if (IsFetchDataNow)
                FetchDataNow();
            else
                FetchDataReal();
        }
        public static string GetStringConfig(string key, string defaultValue)
        {
            if (!IsInitSuccess) return defaultValue;
            if (!FirebaseRemoteConfig.DefaultInstance.Keys.Contains(key)) return defaultValue;
            return FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
        }


        public static bool GetBoolConfig(string key, bool defaultValue)
        {
            if (!IsInitSuccess) return defaultValue;
            if (!FirebaseRemoteConfig.DefaultInstance.Keys.Contains(key)) return defaultValue;
            return FirebaseRemoteConfig.DefaultInstance.GetValue(key).BooleanValue;
        }
        public static long GetLongConfig(string key, long defaultValue)
        {
            if (!IsInitSuccess) return defaultValue;
            if (!FirebaseRemoteConfig.DefaultInstance.Keys.Contains(key)) return defaultValue;
            return FirebaseRemoteConfig.DefaultInstance.GetValue(key).LongValue;
        }
        public static double GetDoubleConfig(string key, double defaultValue)
        {
            if (!IsInitSuccess) return defaultValue;
            if (!FirebaseRemoteConfig.DefaultInstance.Keys.Contains(key)) return defaultValue;
            return FirebaseRemoteConfig.DefaultInstance.GetValue(key).DoubleValue;
        }
        public static float GetFloatConfig(string key, float defaultValue)
        {
            if (!IsInitSuccess) return defaultValue;
            if (!FirebaseRemoteConfig.DefaultInstance.Keys.Contains(key)) return defaultValue;
            string val = FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
            try
            {
                return float.Parse(val, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }


        public static int GetIntConfig(string key, int defaultValue)
        {
            if (!IsInitSuccess) return defaultValue;
            if (!FirebaseRemoteConfig.DefaultInstance.Keys.Contains(key)) return defaultValue;
            string val = FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
            try
            {
                return int.Parse(val);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static bool GetJsonConfig<T>(string key, out T result)
        {
            string input;
            if (!FirebaseRemoteConfig.DefaultInstance.Keys.Contains(key) ||
                string.IsNullOrEmpty(input = FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue))
            {
                result = default;
                return false;

            }

            try
            {
                result = JsonUtility.FromJson<T>(input);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetJsonConfig {typeof(T)} , key {key}, exception: {ex.Message}");
                result = default;
                return false;
            }
        }


        private static void FetchDataNow()
        {
            Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                TimeSpan.FromSeconds(0));
            fetchTask.ContinueWith(FetchComplete);
        }
        private static void FetchDataReal()
        {
            Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                 TimeSpan.FromHours(6));
            fetchTask.ContinueWith(FetchComplete);
        }
        private static void FetchComplete(Task fetchTask)
        {
            if (fetchTask.IsCanceled)
            {
                DebugLog("Fetch canceled.");
            }
            else if (fetchTask.IsFaulted)
            {
                DebugLog("Fetch encountered an error.");
            }
            else if (fetchTask.IsCompleted)
            {
                DebugLog("Fetch completed successfully!");
            }
            //Context.Waiting.HideWaiting();
            var info = FirebaseRemoteConfig.DefaultInstance.Info;
            switch (info.LastFetchStatus)
            {
                case LastFetchStatus.Success:
                    FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWith(_ =>
                    {
                        Dragon.SDK.DebugCustom.LogColor("Remote data loaded and ready (last fetch time )", info.FetchTime);
                    });
                    break;
                case LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason)
                    {
                        case FetchFailureReason.Error:
                            DebugLog("Fetch failed for unknown reason");
                            break;

                        case FetchFailureReason.Throttled:
                            DebugLog("Fetch throttled until " + info.ThrottledEndTime);
                            break;
                    }
                    break;

                case LastFetchStatus.Pending:
                    DebugLog("Latest Fetch call still pending.");
                    break;
            }

        }
        private static void DebugLog(string log)
        {
            Dragon.SDK.DebugCustom.LogColor("[FireBaseRemoteConfig]",log);
        }
    }
}
