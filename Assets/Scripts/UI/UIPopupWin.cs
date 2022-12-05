using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WE.Unit;
using WE.Manager;
using WE.Utils;
using DG.Tweening;
using WE.Support;
using Dragon.SDK;

namespace WE.UI
{
    public class UIPopupWin : UIBase
    {
        public GameObject RewardButton;
        public GameObject noThanks;
        public TextMeshProUGUI noThanksText;
        public TextMeshProUGUI textZone;
        public TextMeshProUGUI textKillCount;
        public TextMeshProUGUI textCoin;
        public TextMeshProUGUI textMultiple;

        int multiple;
        int currentCoin;
        int currentKill;
        bool showAds = false;
        public override void InitUI()
        {
            int timePlay = GameplayManager.Instance.CurrentTimePlay;
            if (timePlay < 180)
            {
                multiple = 10;
            }
            else if (timePlay < 300)
            {
                multiple = 5;
            }
            else if (timePlay < 540)
            {
                multiple = 3;
            }
            else
            {
                multiple = 2;
            }
            textMultiple.text = multiple.ToString();
            SoundManager.Instance.PlaySoundFx(SoundManager.Instance.winSfx);
            if (GameplayManager.Instance.CurrentGameplayType != GameType.Tutorial)
            {

                RewardButton.SetActive(true);
                StartCoroutine(IEActiveNoThanks());
            }
            else
            {
                RewardButton.SetActive(false);
                noThanks.SetActive(true);
                noThanksText.text = "RETURN HOME";
            }
            int currentMap = Player.Instance.CurrentMap;
            textZone.text = "CHAPTER " + currentMap.ToString();
            Player.Instance.AddRecord(GameplayManager.Instance.CurrentTimePlay);
            currentKill = GameplayManager.Instance.CurrentKillCount;
            textKillCount.text = currentKill.ToString();
            currentCoin = 0;
            int val = GameplayManager.Instance.CurrentCoinCount;
            DOTween.To(() => currentCoin, x => currentCoin = x, val, 1).SetUpdate(true).OnUpdate(() =>
            {
                textCoin.text = currentCoin.ToString();
            }).OnComplete(() =>
            {
                currentCoin = val;
                textCoin.text = currentCoin.ToString();
            });
            showAds = false;
            Analytics.LogLevelWin(Player.Instance.CurrentMap, timePlay);
        }
        IEnumerator IEActiveNoThanks()
        {
            noThanksText.text = "NO THANKS";
            noThanks.gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(Constant.TIME_DELAY_NO_THANKS);
            noThanks.gameObject.SetActive(true);
        }
        public void OnClickAds()
        {
            AdsManager.Instance.ShowRewardedAd(() => {
                DoubleReward();
            }, Analytics.rewarded_video_show);
        }
        public void DoubleReward()
        {
            //blockPanel.gameObject.SetActive(true);
            blockPanel.SetActive(true);
            RewardButton.SetActive(false);
            int val = currentCoin * multiple;
            SoundManager.Instance.PlaySoundFx(SoundManager.Instance.coinMultiSfx);
            DOTween.To(() => currentCoin, x => currentCoin = x, val, 2).SetUpdate(true).OnUpdate(() =>
            {
                textCoin.text = currentCoin.ToString();
            }).OnComplete(() =>
            {
                currentCoin = val;
                textCoin.text = currentCoin.ToString();
                blockPanel.gameObject.SetActive(false);
            });
            showAds = true;
            noThanksText.text = "RETURN HOME";
        }
        public void TakeReward()
        {

            Player.Instance.AddCoin(currentCoin);
            Player.Instance.AddEnemyKill(currentKill);
            GameplayManager.Instance.EndGame(true);
            if (!showAds && GameplayManager.Instance.CurrentGameplayType != GameType.Tutorial && GameplayManager.Instance.CanShowGameInter)
            {
                AdsManager.Instance.ShowInterstitial(Analytics.Interstitial_show);
                GameplayManager.Instance.OnAdsInterShow();
            }
        }
        public override void Hide()
        {
            base.Hide();
        }

    }
}

