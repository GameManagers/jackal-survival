using AppsFlyerSDK;
using System.Collections.Generic;
using UnityEngine;
namespace Dragon.SDK
{
    public class AppFlyerManager : MonoBehaviour, IAppsFlyerConversionData
    {
        public string devKeyAndroid;
        public string devKeyIOS;
        public string appIDIOS;
        public bool isDebug;
        public bool getConversionData;
        //******************************//

        public void InitInfo()
        {
            // These fields are set from the editor so do not modify!
            //******************************//
            AppsFlyer.setIsDebug(isDebug);
#if UNITY_ANDROID
            AppsFlyer.initSDK(devKeyAndroid, null, getConversionData ? this : null);
#else
        AppsFlyer.initSDK(devKeyIOS, appIDIOS, getConversionData ? this : null);
#endif
            //******************************//

            AppsFlyer.startSDK();
            //Debug.Log("start Appfflyer");
        }
        public void onConversionDataSuccess(string conversionData)
        {
            AppsFlyer.AFLog("didReceiveConversionData", conversionData);
            Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
            // add deferred deeplink logic here
        }

        public void onConversionDataFail(string error)
        {
            AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
        }

        public void onAppOpenAttribution(string attributionData)
        {
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
            Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
            // add direct deeplink logic here
        }

        public void onAppOpenAttributionFailure(string error)
        {
            AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
        }
    }
}
