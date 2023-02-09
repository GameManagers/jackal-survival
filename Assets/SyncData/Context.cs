using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Profiling;
using WE.Manager;

public class Context 
{
    public static PersionModel profile;

    public static int PVPCurrentMap = 6;
    public static int PvPTimeBattle = 180;
    public static int PvPBattlePoint = 100;
    public static PersionModel CurrentUserPlayfabProfile
    {
        get
        {
            if (profile == null) return new PersionModel();
            return profile;
        }
        set
        {
            profile = value;
        }
    }

    public static bool CheckNetwork()
    {
        if (!RocketIO.IsLogined || Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }
        return true;
    }
    public static void LoginServer(Action actionSuccess, Action actionError = null, bool IsShowLoading = true)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //Show Dialog Connect Internet
            if (IsShowLoading)
            {
                UIManager.Instance.ShowTextNoInternet();
                actionError?.Invoke();
            }
            return;
        }

        RocketIO.Instance.LoginSequence(IsShowLoading).Subscribe(
            success =>
            {
                actionSuccess?.Invoke();
            },
            error =>
            {
                if (IsShowLoading)
                {
                    UIManager.Instance.ShowTextNotConnectServer();
                }
                actionError?.Invoke();
            });
    }
}
