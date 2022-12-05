using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Manager;
using WE.Support;
using WE.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UniRx;
using WE.Utils;
using WE.Pooling;
using WE.Unit;
using Dragon.SDK;

namespace WE.UI
{
    public class UIPopupOpenChest : UIBase
    {
        public Transform chestUI;
        public UIChest WoodenChest, SilverChest, GoldenChest;
        public GameObject buttonUpgrade, buttonOpen, buttonClose, panelSkill;


        public float iconStartScale = 0.1f;
        public float scaleDuration = 1.5f;
        public int timerDelay = 3;
        public SkillSlotItem[] skillSlotItems;
        TypeChest currentChestType;
        int timer;
        CompositeDisposable disposables;
        List<UpgradeResult> dicSkill;
        bool takeSkill;
        UIChest currentChest;
        public Transform content;

        public TextMeshProUGUI textCountOpen;
        public TextMeshProUGUI textCountUpgrade;
        public override bool CanBack
        {
            get => !isClosing && buttonClose.activeInHierarchy;
        }

        public override void InitUI()
        {
            takeSkill = false;
            timer = timerDelay;
            currentChestType = Helper.GetRandomByPercent(DataManager.Instance.dataZoneChestDrop.RateChestBoss);
            if (Helper.IsLuckApply())
            {
                float rate1 = DataManager.Instance.dataZoneChestDrop.RateChestBoss[currentChestType];
                TypeChest result2 = Helper.GetRandomByPercent(DataManager.Instance.dataZoneChestDrop.RateChestBoss);
                float rate2 = DataManager.Instance.dataZoneChestDrop.RateChestBoss[result2];
                if (rate2 > rate1)
                    currentChestType = result2;
            }
            buttonUpgrade.SetActive(true);
            buttonOpen.SetActive(true);
            buttonClose.SetActive(false);
            panelSkill.SetActive(false);
            textCountOpen.text = string.Empty;
            textCountUpgrade.text = string.Empty;

            for (int i = 0; i < skillSlotItems.Length; i++)
            {
                skillSlotItems[i].gameObject.SetActive(false);
                skillSlotItems[i].transform.localScale = Vector3.one * iconStartScale;
                content.transform.localScale = Vector3.one * iconStartScale;
            }
            switch (currentChestType)
            {
                case TypeChest.Wooden:
                    currentChest = Instantiate(WoodenChest, chestUI);
                    textCountOpen.text = "GET 1 SKILL";
                    textCountUpgrade.text = "GET 3 SKILLS";
                    //StartCoroutine(IEHideButtonAds());
                    break;
                case TypeChest.Silver:
                    currentChest = Instantiate(SilverChest, chestUI);
                    textCountOpen.text = "GET 3 SKILLS";
                    textCountUpgrade.text = "GET 5 SKILLS";
                    //StartCoroutine(IEHideButtonAds());
                    break;
                case TypeChest.Golden:
                    textCountOpen.text = "GET 5 SKILLS";
                    currentChest = Instantiate(GoldenChest, chestUI);
                    buttonUpgrade.SetActive(false);
                    break;
                default:
                    break;
            }
        }
        //IEnumerator IEHideButtonAds()
        //{
        //    buttonUpgrade.SetActive(true); 
        //    textCountDown.gameObject.SetActive(true);
        //    while (timer >= 0)
        //    {
        //        textCountDown.text = $"({timer})";
        //        yield return new WaitForSecondsRealtime(1.2f);
        //        timer--;

        //    }
        //    textCountDown.gameObject.SetActive(false);
        //    buttonOpen.SetActive(true);
        //    buttonUpgrade.SetActive(false);
        //}
        public void OnAdsClick()
        {
            //StopAllCoroutines();
            //textCountDown.gameObject.SetActive(false);
            AdsManager.Instance.ShowRewardedAd(() => {
                UpgareChest();
            }, Analytics.rewarded_video_show);
        }
        void UpgareChest()
        {
            Destroy(currentChest.gameObject);
            buttonUpgrade.SetActive(false);
            GameplayManager.Instance.OnAdsInterShow();
            textCountUpgrade.text = string.Empty;
            switch (currentChestType)
            {
                case TypeChest.Wooden:
                    currentChestType = TypeChest.Silver;
                    currentChest = Instantiate(SilverChest, chestUI);
                    textCountOpen.text = "GET 3 SKILLS";
                    break;
                case TypeChest.Silver:
                    currentChestType = TypeChest.Golden;
                    currentChest = Instantiate(GoldenChest, chestUI);
                    textCountOpen.text = "GET 5 SKILLS";
                    break;
                case TypeChest.Golden:
                    break;
                default:
                    break;
            }
        }
        public void OnOpenClick()
        {
            buttonOpen.SetActive(false);
            buttonUpgrade.SetActive(false);
            OpenChest();
        }
        void OpenChest()
        {
            blockPanel.SetActive(true);
            currentChest.OnOpenChest += OnOpenChest;
            currentChest.OnClickedOpenChest();
        }
        public void OnOpenChest()
        {
            if (GameplayManager.Instance.CanShowGameInter)
            {
                AdsManager.Instance.ShowInterstitial(Analytics.Interstitial_show);
                GameplayManager.Instance.OnAdsInterShow();
            }
            StartCoroutine(IEShowSkill());
        }
        void OnDisable()
        {
            Destroy(currentChest.gameObject);
        }
        IEnumerator IEShowSkill()
        {
            buttonOpen.SetActive(false);
            yield return new WaitForSecondsRealtime(0.3f);
            panelSkill.SetActive(true);
            blockPanel.SetActive(true);
            int skillNumb = 0;
            currentChest.OnOpenChest -= OnOpenChest;
            switch (currentChestType)
            {
                case TypeChest.Wooden:
                    skillNumb = 1;
                    break;
                case TypeChest.Silver:
                    skillNumb = 3;
                    break;
                case TypeChest.Golden:
                    skillNumb = 5;
                    break;
                default:
                    break;
            }
            content.DOScale(1, scaleDuration).SetUpdate(true);
            dicSkill = SkillController.Instance.GetChestSkill(skillNumb);
            for (int i = 0; i < skillNumb; i++)
            {
                skillSlotItems[i].transform.DOScale(1, scaleDuration).SetUpdate(true);
                skillSlotItems[i].gameObject.SetActive(true);
                skillSlotItems[i].InitSkill(dicSkill[i].key, dicSkill[i].level);
            }
            if (!takeSkill)
            {
                takeSkill = true;
                foreach (var item in dicSkill)
                {
                    if (item.key == Constant.COIN_CHEST)
                    {
                        GameplayManager.Instance.ReviceItem(EItemInGame.Big_Coin);
                    }
                    else if (item.key == Constant.HP_CHEST)
                    {
                        GameplayManager.Instance.ReviceItem(EItemInGame.Heal);
                    }
                    else
                    {
                        SkillController.Instance.AddSkillByString(item.key);
                    }
                }
            }

            blockPanel.SetActive(false);
            buttonClose.SetActive(true);

        }
        public override void AfterHideAction()
        {
            Helper.SpawnEffect(ObjectPooler.Instance.fxReviceSkill, Player.Instance.transform.position, Player.Instance.transform);
            base.AfterHideAction();
        }
    }
}

