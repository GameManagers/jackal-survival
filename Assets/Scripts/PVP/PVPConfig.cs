using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVPConfig : MonoBehaviour
{
    public float Time_Out_Reconnect => 20;
    public float Time_Out_Start_Game => 35;

    public string URL
    {
        get
        {

            if (!RocketIO.Instance.IsPublicServer)
            {
                return URL_Test;
            }
            
            return URL_Publish;
        }
    }

    public string URL_HTTP
    {
        get
        {
            if (!RocketIO.Instance.IsPublicServer)
            {
                return URL_HTTP_Test;
            }

            return URL_HTTP_Publish;
        }
    }
    public string URL_Publish => "ws://34.87.155.178:3978";
    public string URL_Test => "ws://192.168.0.103:3978";

    public string URL_HTTP_Publish => "http://34.87.155.178:8000/api/match";
    public string URL_HTTP_Test => "http://192.168.0.103:8000/api/match";
}
