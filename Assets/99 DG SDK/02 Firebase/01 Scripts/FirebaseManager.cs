using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

enum VerifyFirebase
{
    Verifying,
    Done,
    Error
}
namespace Dragon.SDK
{
    public class FirebaseManager : MonoBehaviour
    {
        public static string FirebaseID;
        public bool FirebaseInitialized => _firebaseInitialized;
        public Action OnInitSuccess;
        private bool _firebaseInitialized = false;
        private VerifyFirebase firebaseReady = VerifyFirebase.Verifying;
        public void InitInfo()
        {
            try
            {
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    DependencyStatus dependencyStatus = task.Result;
                    if (dependencyStatus == Firebase.DependencyStatus.Available)
                    {
                        //DebugCustom.LogError("Firebase is ready for use.");
                        firebaseReady = VerifyFirebase.Done;
                    }
                    else
                    {
                        //DebugCustom.LogError("Firebase is not ready for use.");
                        firebaseReady = VerifyFirebase.Error;
                        Debug.LogError("firebase Ready  Error");
                    }
                });
            }
            catch (Exception e)
            {
                firebaseReady = VerifyFirebase.Error;
                Debug.LogError("firebase Ready Error: " + e.ToString());
            }
            StartCoroutine(InitFirebase());
        }
        IEnumerator InitFirebase()
        {
            int _num = 0;
            while (firebaseReady == VerifyFirebase.Verifying)
            {
                _num++;
                if (_num > 5)
                {
                    Debug.LogError("Init Firebase Error");
                    break;
                }
                yield return new WaitForSeconds(1);
            }
            if (firebaseReady == VerifyFirebase.Done)
            {
                InitializeFirebase();
                yield return new WaitForSeconds(1);
                if (_firebaseInitialized)
                {
                    try
                    {
                        GetFirebaseID();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("GetFirebaseID Error: " + ex.ToString());
                    }
                }
            }

        }
        void InitializeFirebase()
        {
            try
            {
                _firebaseInitialized = true;
                FireBaseRemoteConfig.FetchData();
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                //Analytics.SetUserProperty("last_login", DateTime.Now.DayOfYear.ToString());
                //Analytics.SetUserProperty("app_version", RocketConfig.VersionName);
                //Analytics.SetUserProperty("first_install_version", _first_install_version);

            }
            catch (Exception e)
            {
                Debug.LogError("Init Firebase Error: " + e.ToString());
                _firebaseInitialized = false;
            }
            if (_firebaseInitialized)
                OnInitSuccess?.Invoke();
        }
        public static Task<string> GetAnalyticsInstanceId()
        {
            return FirebaseAnalytics.GetAnalyticsInstanceIdAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    //DebugLog("App instance ID fetch was canceled.");
                }
                else if (task.IsFaulted)
                {
                    //DebugLog(String.Format("Encounted an error fetching app instance ID {0}",
                    //task.Exception.ToString()));
                }
                else if (task.IsCompleted)
                {
                    //DebugLog(String.Format("App instance ID: {0}", task.Result));
                }
                return task;
            }).Unwrap();
        }
        private async void GetFirebaseID()
        {
            try
            {
                FirebaseID = await GetAnalyticsInstanceId();
            }
            catch (Exception ex)
            {
                Debug.LogError("GetFirebaseID Error: " + ex.ToString());
            }

        }
    }
}
