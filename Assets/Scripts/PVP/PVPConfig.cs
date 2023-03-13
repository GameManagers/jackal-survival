using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVPConfig
{
    public int RequestCompleteZone => 2;
    public float Time_Out_Reconnect => 20;
    public float Time_Out_Start_Game => 35;

    public string URL => RocketIO.Instance.GetPVPRoomUrl;

    public string URL_HTTP => RocketIO.Instance.GetPVPMatchMarker;
}
