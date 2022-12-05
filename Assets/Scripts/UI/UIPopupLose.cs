using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using WE.Utils;
using WE.Manager;
using WE.Unit;
using WE.Support;
using Dragon.SDK;

namespace WE.UI
{
    public class UIPopupLose : UIBase
    {
        public GameObject newRecord;
        public GameObject RewardButton;
        public GameObject noThanks;
        public TextMeshProUGUI noThanksText;
        public TextMeshProUGUI textZone;
        public TextMeshProUGUI textRecord;
        public TextMeshProUGUI texthighest;
        public TextMeshProUGUI textKillCount;
        public TextMeshProUGUI textCoin;
        public TextMeshProUGUI textMultiple;

        int multiple = 2;
        int currentCoin;
        int currentKill;
        bool showAds;
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
            SoundManager.Instance.PlaySoundFx(SoundManager.Instance.loseSfx);
            RewardButton.SetActive(true);
            StartCoroutine(IEActiveNoThanks());
            currentCoin = GameplayManager.Instance.CurrentCoinCount;
            currentKill = GameplayManager.Instance.CurrentKillCount;
            textKillCount.text = currentKill.ToString();
            textCoin.text = currentCoin.ToString();
            textMultiple.text = multiple.ToString();
            int currentMap = Player.Instance.CurrentMap;
            switch (GameplayManager.Instance.CurrentGameplayType)
            {
                case GameType.Campaign:
                    textZone.text = "CHAPTER " + currentMap.ToString();
                    int timeHighest = Player.Instance.PLayerData.GetRecord(currentMap);
                    textRecord.text = Helper.ConvertTimer(timePlay);
                    texthighest.text = "HIGHEST : " + Helper.ConvertTimer(timeHighest);
                    newRecord.SetActive(timePlay > timeHighest);
                    if (timePlay > timeHighest)
                    {
                        Player.Instance.AddRecord(timePlay);
                    }
                    break;
                case GameType.Tutorial:
                    textZone.text = string.Empty;
                    textRecord.text = Helper.ConvertTimer(timePlay);
                    texthighest.text = Helper.ConvertTimer(timePlay);
                    newRecord.SetActive(true);
                    break;
                case GameType.Endless:
                    textZone.text = "ENDLESS MODE";
                    int _timeHighest = Player.Instance.GetRecordTimeEndless();
                    textRecord.text = Helper.ConvertTimer(timePlay);
                    texthighest.text = "HIGHEST : " + Helper.ConvertTimer(_timeHighest);
                    newRecord.SetActive(timePlay > _timeHighest);
                    Player.Instance.AddEndlessRecord(GameplayManager.Instance.CurrentTimePlay, currentKill);
                    break;
                default:
                    break;
            }
            Analytics.LogLevelLose(Player.Instance.CurrentMap, timePlay);
            showAds = false;
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
            RewardButton.SetActive(false);
            int val = currentCoin * multiple;
            SoundManager.Instance.PlaySoundFx(SoundManager.Instance.coinMultiSfx);
            DOTween.To(() => currentCoin, x => currentCoin = x, val, 2f).OnUpdate(() => {
                textCoin.text = currentCoin.ToString();
            }).SetUpdate(true).OnComplete(() => {
                textCoin.text = currentCoin.ToString();

                blockPanel.gameObject.SetActive(false);
            });
            noThanksText.text = "RETURN HOME";
            showAds = true;
        }
        public void TakeReward()
        {
            Player.Instance.AddCoin(currentCoin);
            Player.Instance.AddEnemyKill(currentKill);
            GameplayManager.Instance.EndGame(false);
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

