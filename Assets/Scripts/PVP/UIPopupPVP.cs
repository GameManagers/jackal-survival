using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WE.Manager;

namespace WE.UI.PVP
{
    public class UIPopupPVP : UIBase
    {
        public Button PlayPVPButton;

        public override void InitUI()
        {
        }

        public void StartPVP()
        {
            base.Hide();
            GameplayManager.Instance.StartGame(GameType.PVP);
        }
    }
}