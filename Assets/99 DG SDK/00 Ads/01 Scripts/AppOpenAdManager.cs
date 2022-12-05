using Dragon.SDK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppOpenAdManager : MonoBehaviour
{
    [SerializeField] private bool _defaulOpenApp = false;
    public string AppOpenAdUnitId = "";
    public static bool ResumeFromAds = false;
    private bool _isActiveConfigAOA
    {
        get
        {
            return FireBaseRemoteConfig.GetBoolConfig("Active_AOA_Jackal_Survival", _defaulOpenApp);
        }
    }
    public void ShowAdIfReady()
    {
        if (MaxSdk.IsAppOpenAdReady(AppOpenAdUnitId))
        {
            MaxSdk.ShowAppOpenAd(AppOpenAdUnitId);
            //Debug.LogError("AOA Load Success");
        }
        else
        {
            MaxSdk.LoadAppOpenAd(AppOpenAdUnitId);
            //Debug.LogError("AOA Load False");
        }
    }
}
