using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using WE.Manager;
using WE.Data;
using WE.Unit;
using WE.Support;
using WE.Utils;
using DG.Tweening;
using WE.Effect;
using Dragon.SDK;

namespace WE.UI
{
    public class UISelectVehicle : UIBase
    {
        public Sprite coinSprite;
        public Sprite killSprite;
        [Header("GameObject assign")]
        public GameObject conditionText;
        public GameObject selectedButton;
        public Transform carVisualizeParent;
        public GameObject TestUnlockButton;
        public GameObject adsButton;
        [Header("Text assign")]
        public TextMeshProUGUI textName;
        public TextMeshProUGUI textLevel;
        public TextMeshProUGUI textBuyCost;
        public TextMeshProUGUI textUpgradeCost;
        public TextMeshProUGUI textUnlockCondition;
        public TextMeshProUGUI textUnlockValue;
        public TextMeshProUGUI textMaxHpCurrentValue;
        public TextMeshProUGUI textDamageCurrentValue;
        public TextMeshProUGUI textMaxHpAdd;
        public TextMeshProUGUI textDamageAdd;
        public TextMeshProUGUI textAdsCount;
        [Header("Button assign")]
        public Button previousCar;
        public Button nextCar;
        public Button SelectButton;
        public Button BuyButton;
        public Button UnlockButton;
        public Button UpgradeButton;
        [Header("Image assign")]
        public Image conditionImage;

        GameObject currentVisualize;
        CarController controller => CarController.Instance;
        VehicleConfig config => controller.SelectedConfig;
        bool currentUpgrade => controller.currentUpgrade;
        Tween hpTween;
        Tween dmgTween;
        public AnimationEffect fxUpgrade;

        int currentValueHp;
        int currentValueDmg;
        public override void InitUI()
        {

            if (Constant.IS_TESTER_JACKAL)
            {
                TestUnlockButton.SetActive(true);
            }
            else
            {
                TestUnlockButton.SetActive(false);
            }
            OnUpdateUI();
            previousCar.onClick.AddListener(PreviousCar);
            nextCar.onClick.AddListener(NextCar);
            SelectButton.onClick.AddListener(SelectCar);
            BuyButton.onClick.AddListener(UnlockCar);
            UnlockButton.onClick.AddListener(UnlockCar);
            UpgradeButton.onClick.AddListener(UpgradeCar);
            controller.currentUpgrade = false;

        }
        private void OnDisable()
        {

            previousCar.onClick.RemoveListener(PreviousCar);
            nextCar.onClick.RemoveListener(NextCar);
            SelectButton.onClick.RemoveListener(SelectCar);
            BuyButton.onClick.RemoveListener(UnlockCar);
            UnlockButton.onClick.RemoveListener(UnlockCar);
            UpgradeButton.onClick.RemoveListener(UpgradeCar);
        }
        public void OnUpdateUI()
        {
            //controller.GetSelectedConfig();
            if (currentVisualize != null)
            {
                Destroy(currentVisualize);
            }
            currentVisualize = Instantiate(config.iconVisual, carVisualizeParent);
            textName.text = config.nameCar.ToUpper();
            adsButton.SetActive(false);
            textDamageAdd.text = string.Empty;
            textMaxHpAdd.text = string.Empty;
            if (controller.SelectedLevel() > 0)
            {
                textLevel.text = "LEVEL " + controller.SelectedLevel().ToString();
            }
            else
            {
                textLevel.text = string.Empty;
            }
            SelectButton.gameObject.SetActive(false);
            selectedButton.SetActive(false);
            BuyButton.gameObject.SetActive(false);
            UnlockButton.gameObject.SetActive(false);
            conditionText.gameObject.SetActive(false);
            if (controller.IsSelected())
            {
                selectedButton.SetActive(true);
            }
            else if (controller.IsUnlocked())
            {
                SelectButton.gameObject.SetActive(true);
            }
            else if (controller.CanUnlock())
            {
                if (config.UnlockType == EUnlockType.Coin)
                {
                    BuyButton.gameObject.SetActive(true);
                    textBuyCost.text = config.ValueUnlock.ToString();
                }
                else
                {
                    UnlockButton.gameObject.SetActive(true);
                }
            }
            else
            {
                conditionText.gameObject.SetActive(true);
                switch (config.UnlockType)
                {
                    case EUnlockType.Coin:
                        break;
                    case EUnlockType.Kill_Count:
                        conditionImage.sprite = killSprite;
                        conditionImage.enabled = true;
                        textUnlockCondition.text = string.Format(Helper.GetTranslation("desc_condition_count_kill"), config.ValueUnlock);
                        break;
                    case EUnlockType.Spend_Coin:
                        conditionImage.sprite = coinSprite;
                        conditionImage.enabled = true;
                        textUnlockCondition.text = string.Format(Helper.GetTranslation("desc_condition_count_coin"), config.ValueUnlock);
                        break;
                    case EUnlockType.Level_Reach:
                        conditionImage.enabled = false;
                        textUnlockCondition.text = string.Format(Helper.GetTranslation("desc_condition_count_level"), config.ValueUnlock);
                        break;
                    case EUnlockType.Ads:
                        conditionImage.enabled = false;
                        conditionText.gameObject.SetActive(false);
                        int adsWatch = Player.Instance.GetAdsCount(config.VehicleType);
                        int adsUnlock = config.ValueUnlock;
                        if (adsWatch < adsUnlock)
                        {
                            adsButton.SetActive(true);
                            textAdsCount.text = $"{adsWatch} / {adsUnlock}";
                        }
                        else
                        {
                            UnlockButton.gameObject.SetActive(true);
                        }
                        break;
                    default:
                        break;
                }
                textUnlockValue.text = controller.GetUnlockConditionCount();
            }
            UpgradeButton.gameObject.SetActive(false);
            if (controller.IsUnlocked())
            {
                UpgradeButton.gameObject.SetActive(true);
                textUpgradeCost.text = controller.GetUpgradeCost().ToString();
            }
            dmgTween.Kill(true);
            hpTween.Kill(true);
            textDamageAdd.DOKill();
            textMaxHpAdd.DOKill();
            textDamageAdd.color = Color.green;
            textMaxHpAdd.color = Color.green;
            textDamageAdd.text = string.Empty;
            textMaxHpAdd.text = string.Empty;
            if (currentUpgrade)
            {
                Helper.SpawnEffect(fxUpgrade, carVisualizeParent.transform.position, null);
                textMaxHpCurrentValue.text = currentValueHp.ToString();
                textDamageCurrentValue.text = currentValueDmg.ToString();
                textMaxHpAdd.text = "+" + (int)controller.GetHpUpgradeValue();
                textDamageAdd.text = "+" + (int)controller.GetDamageUpgradeValue();
                textDamageAdd.DOFade(0f, 1f);
                dmgTween = DOTween.To(() => currentValueDmg, x => currentValueDmg = x, (int)config.GetCarDamage(), 1f).SetUpdate(true).OnUpdate(() => {
                    textDamageCurrentValue.text = currentValueDmg.ToString();
                }).OnComplete(() => {
                    textDamageCurrentValue.text = ((int)config.GetCarDamage()).ToString();
                    currentValueDmg = (int)config.GetCarDamage();
                });
                textMaxHpAdd.DOFade(0f, 1f);
                hpTween = DOTween.To(() => currentValueHp, x => currentValueHp = x, (int)config.GetCarHp(), 1f).SetUpdate(true).OnUpdate(() => {
                    textMaxHpCurrentValue.text = currentValueHp.ToString();
                }).OnComplete(() => {
                    textMaxHpCurrentValue.text = ((int)config.GetCarHp()).ToString();
                    controller.currentUpgrade = false;
                    currentValueHp = (int)config.GetCarHp();
                });
            }
            else
            {
                currentValueDmg = (int)config.GetCarDamage();
                currentValueHp = (int)config.GetCarHp();
                textMaxHpCurrentValue.text = currentValueHp.ToString();
                textDamageCurrentValue.text = currentValueDmg.ToString();
            }
            //if (!controller.IsMaxLevel())
            //{
            //switch (controller.GetCurrentUpgrade().UpgaradeType)
            //    {
            //        case EVehicleUpgrade.Hp_Increase:
            //            textMaxHpAdd.text = "+" + controller.GetCurrentUpgrade().UpgradeValue.ToString();
            //            break;
            //        case EVehicleUpgrade.Attack_Increase:
            //            textDamageAdd.text = "+" + controller.GetCurrentUpgrade().UpgradeValue.ToString();
            //            break;
            //        default:
            //            break;
            //    }
            //}

            }
        public void SelectCar()
        {
            Player.Instance.SelectCar(config.VehicleType);
            OnUpdateUI();
        }
        public void UnlockCar()
        {
            controller.UnlockCar();
            OnUpdateUI();
        }
        public void NextCar()
        {
            if (controller.currentUpgrade)
                return;
            controller.UINextCar();
            OnUpdateUI();
        }
        public void PreviousCar()
        {
            if (controller.currentUpgrade)
                return;
            controller.UIPreviousCar();
            OnUpdateUI();
        }
        public void UpgradeCar()
        {
            if (controller.currentUpgrade)
                return;
            controller.UpgradeCar();
            OnUpdateUI();
        }
        public void TestUnlock()
        {
            CarController.Instance.TestUnlock();
            OnUpdateUI();
        }
        public void AdsCar()
        {
            AdsManager.Instance.ShowRewardedAd(() => { AddAds(); }, Analytics.rewarded_video_show);
        }
        public void AddAds()
        {
            Player.Instance.AddVehicleAds(config.VehicleType);
            OnUpdateUI();
        }
    }

}


