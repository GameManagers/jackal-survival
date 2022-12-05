using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WE.Manager;
using UnityEngine.UI;
using WE.Unit;
using Dragon.SDK;

namespace WE.UI
{
    public class UIDie : UIBase
    {
        public GameObject buttonFreeRive;
        public GameObject buttonRivaAds;
        public TextMeshProUGUI textCountDown;
        public int CountDown = 9;
        int countDown;
        public override void InitUI()
        {
            bool freeRivie = Player.Instance.RevivalCount > 0;
            buttonFreeRive.SetActive(freeRivie);
            buttonRivaAds.SetActive(!freeRivie);
            StartCoroutine(IECountDown());
        }

        IEnumerator IECountDown()
        {
            countDown = CountDown;
            while (countDown > 0)
            {
                textCountDown.text = countDown.ToString();
                countDown--;
                yield return new WaitForSecondsRealtime(1.1f);
            }
            OnCloseClicked();
        }
        public void OnClicked()
        {
            Player.Instance.Revival();
            Hide();
        }
        public void OnClickAds()
        {
            AdsManager.Instance.ShowRewardedAd(() => {
                OnClicked();
            }, Analytics.rewarded_video_show);
        }
        public void OnCloseClicked()
        {
            GameplayManager.Instance.ShowPopupEndGame(false);
            Hide();
        }
    }

}
