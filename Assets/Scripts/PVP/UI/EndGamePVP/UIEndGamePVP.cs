using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WE.Manager;

namespace WE.UI.PVP.EndGame
{
    public class UIEndGamePVP : UIBase
    {
        public GameObject RewardButton;
        public GameObject noThanks;

        public TextMeshProUGUI noThanksText;

        public override void InitUI()
        {
        }
        public override void Hide()
        {
            base.Hide();
        }

        public void TakeRewardPVP()
        {
            GameplayManager.Instance.EndGame(true);
        }
    }

}
