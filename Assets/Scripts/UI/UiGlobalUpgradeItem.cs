using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WE.Unit;
using TigerForge;
using WE.Utils;
using WE.Support;
using TMPro;
using Dragon.SDK;
using WE.Manager;

namespace WE.UI
{
    public class UiGlobalUpgradeItem : MonoBehaviour
    {
        public Sprite damageIcon;
        public Sprite hpIcon;
        public Sprite rewardIcon;
        public GolobalUpgrade UpgradeType;
        public Image icon;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI valueText;
        public TextMeshProUGUI costText;

        public GameObject iconCost;
        public GameObject iconAds;
        public void InitUI()
        {
            switch (UpgradeType)
            {
                case GolobalUpgrade.Hp_Increase:
                    icon.sprite = hpIcon;
                    titleText.text = Helper.GetTranslation("tit_glbUpgrade_hp");
                    break;
                case GolobalUpgrade.Attack_Increase:
                    titleText.text = Helper.GetTranslation("tit_glbUpgrade_damage");
                    icon.sprite = damageIcon;
                    break;
                case GolobalUpgrade.Reward_Increase:
                    titleText.text = Helper.GetTranslation("tit_glbUpgrade_reward");
                    icon.sprite = rewardIcon;
                    break;
                default:
                    break;
            }
            OnUpgradeChange();
            EventManager.StartListening(Constant.ON_ADD_GLOBAL_UPGRADE, OnUpgradeChange);
        }
        private void OnDisable()
        {
            EventManager.StopListening(Constant.ON_ADD_GLOBAL_UPGRADE, OnUpgradeChange);
        }
        public void OnUpgradeChange()
        {
            int cost = Player.Instance.GetGlobalUpgradeCost(UpgradeType);
            if(Player.Instance.currentCoin >= cost)
            {
                iconCost.SetActive(true);
                iconAds.SetActive(false);
            }
            else
            {
                iconCost.SetActive(false);
                iconAds.SetActive(true);
            }
            valueText.text = "+" + Player.Instance.GetGlobalUpgradeValue(UpgradeType).ToString() + "%";
            costText.text = cost.ToString();
        }
        public void BuyUpgrade()
        {
            int cost = Player.Instance.GetGlobalUpgradeCost(UpgradeType);
            if (Player.Instance.currentCoin >= cost)
            {
                Player.Instance.AddGlobalUpgrade(UpgradeType);
                Player.Instance.AddCoin(-cost);
            }
            else
            {
                //Ads
                AdsManager.Instance.ShowRewardedAd(() => {
                    Player.Instance.AddGlobalUpgrade(UpgradeType);
                }, Analytics.rewarded_video_show);
            }
        }
        
    }
}

