using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dragon.SDK;

namespace WE.UI
{
    public class UIPopupMoreGames : UIBase
    {
        public string jackalURL 
            {
            get
            {
                return FireBaseRemoteConfig.GetStringConfig("linkmoregame", "market://details?id=com.rocket.jackal.squad");
            }
        } 
        public override void InitUI()
        {
        }
        public void OnClickedJackal()
        {
            Analytics.LogEventByName("JackalSurvivalClickMoreGame");
            Application.OpenURL(jackalURL);
        }
    }
}

