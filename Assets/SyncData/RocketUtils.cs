using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketUtils
{
    public static string GetDeviceId()
    {
#if UNITY_EDITOR
        return SystemInfo.deviceUniqueIdentifier;
#elif UNITY_ANDROID
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
		AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
		string android_id = secure.CallStatic<string>("getString", contentResolver, "android_id");
		return android_id;
#elif UNITY_IOS
        string uuid = UUIDiOS.GetKeyChainValue("uuid");
        if(string.IsNullOrEmpty(uuid))
        {
            uuid = SystemInfo.deviceUniqueIdentifier;
            UUIDiOS.SaveKeyChainValue("uuid", uuid);
        }

        return uuid;
#endif
    }
}
