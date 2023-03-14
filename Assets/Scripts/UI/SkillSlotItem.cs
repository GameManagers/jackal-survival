using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Skill;
using UnityEngine.UI;
using TMPro;
using WE.Support;
using WE.Manager;
using WE.Utils;
using WE.Pooling;
using Dragon.SDK;
using WE.PVP.Manager;

namespace WE.UI
{
    public class SkillSlotItem : MonoBehaviour
    {
        public UISelectSkill uiSelectSkill;
        public TextMeshProUGUI textTile;
        public TextMeshProUGUI textDesc;
        public Image backGroundImage;
        public Image Icon;
        public List<IconStar> listStar;
        public GameObject evoleCondition;
        public List<Image> evoleSkillsIcon;
        public Animator anim;
        public GameObject newText;
        public Sprite activeBG, passiveBG, evoleBG;
        public bool isChestItem;
        bool isEvoleCondition;
        public string currentCode;
        public bool isEvoleSkill;
        public void InitSkill(string skillCode)
        {
            InitSkill(skillCode, SkillController.Instance.GetCurrentLevel(skillCode) + 1);
        }

        public void InitSkill(string skillCode, int level)
        {
            currentCode = skillCode;
            isEvoleSkill = SkillController.Instance.IsEvoleSkill(skillCode);
            if (isChestItem)
                textDesc.gameObject.SetActive(false);
            for (int i = 0; i < listStar.Count; i++)
            {
                listStar[i].InitIcon(false);
                if (SkillController.Instance.IsEvoleSkill(skillCode))
                    listStar[i].IconShow.color = Color.red;
                else
                    listStar[i].IconShow.color = Color.white;
                listStar[i].gameObject.SetActive(false);
            }
            if (evoleSkillsIcon != null)
            {
                for (int i = 0; i < evoleSkillsIcon.Count; i++)
                {
                    evoleSkillsIcon[i].gameObject.SetActive(false);
                }
            }

            if (skillCode == Constant.COIN_CHEST)
            {
                Icon.sprite = ObjectPooler.Instance.iconBigCoin;
                textTile.text = Helper.GetTranslation(skillCode + "_name").ToUpper();
                textDesc.gameObject.SetActive(true);
                textDesc.text = string.Format(Helper.GetTranslation(skillCode + "_desc"),
                    GameplayManager.Instance.GetBigCoinValue());
            }
            else if (skillCode == Constant.HP_CHEST)
            {
                Icon.sprite = ObjectPooler.Instance.iconHeal;
                textTile.text = Helper.GetTranslation(skillCode + "_name").ToUpper();
                textDesc.text = Helper.GetTranslation(skillCode + "_desc");
            }
            else
            {

                textTile.text = Helper.GetTranslation(skillCode + "_name").ToUpper();
                Icon.sprite = SkillController.Instance.GetSkillSprite(skillCode);

                int maxLevel = SkillController.Instance.GetMaxLevel(skillCode);
                for (int i = 0; i < maxLevel; i++)
                {
                    listStar[i].gameObject.SetActive(true);
                }
                if (SkillController.Instance.IsPassiveSkill(skillCode))
                {
                    EPassiveSkill psType;
                    Helper.TryToEnum(skillCode, out psType);
                    textDesc.text = string.Format(Helper.GetTranslation(skillCode + "_desc"), SkillController.Instance.dataSkill.PassiveSkillData[psType].ValuePerLevel);
                }
                else if (level == 1)
                {
                    textDesc.text = Helper.GetTranslation(skillCode + "_desc");
                }
                else if (SkillController.Instance.IsActiveSkill(skillCode))
                {
                    string outPut = string.Empty;
                    if (!isChestItem)
                    {
                        var data = SkillController.Instance.GetLevelUpEffect(skillCode);

                        foreach (string item in data)
                        {
                            outPut = outPut + Helper.GetTranslation(item + "_Upgrade_desc") + "\n";
                        }
                    }
                    else
                    {
                        var data = SkillController.Instance.GetLevelUpEffect(skillCode, level);

                        foreach (string item in data)
                        {
                            outPut = outPut + Helper.GetTranslation(item + "_Upgrade_desc") + "\n";
                        }
                    }
                    textDesc.text = outPut;
                }
                for (int i = 0; i < level; i++)
                {
                    listStar[i].InitIcon(true);
                }
                if (evoleCondition != null)
                {
                    evoleCondition.SetActive(SkillController.Instance.dataSkill.DicEvole.ContainsKey(skillCode));
                }
                if (evoleSkillsIcon != null)
                {
                    if (evoleSkillsIcon.Count > 0)
                    {

                        if (SkillController.Instance.dataSkill.DicEvole.ContainsKey(skillCode))
                        {
                            List<string> evoleDic = SkillController.Instance.dataSkill.DicEvole[skillCode];
                            int evolecount = 0;
                            for (int i = 0; i < evoleDic.Count; i++)
                            {
                                EActiveSkill _type = EActiveSkill.Lightning_Support;

                                if (Helper.TryToEnum(evoleDic[i], out _type))
                                {
                                    if (SkillController.Instance.dataSkill.ActiveSkillConfigData[_type].IsFreeToDrop || SkillController.Instance.dicStringCurrentSkill.ContainsKey(evoleDic[i]))
                                    {
                                        evolecount++;
                                        evoleSkillsIcon[i].gameObject.SetActive(true);
                                        evoleSkillsIcon[i].sprite = SkillController.Instance.GetSkillSprite(evoleDic[i]);
                                        evoleSkillsIcon[i].SetNativeSize();
                                    }
                                }

                            }
                            if (evolecount < 1)
                            {

                                evoleCondition.SetActive(false);
                            }
                        }
                    }
                }
            }
            //int c = isChestItem ? level - 1 : level;
            int c = level - 1;
            if (SkillController.Instance.IsPassiveSkill(skillCode))
            {
                backGroundImage.sprite = passiveBG;
                listStar[c].StarBlink();
                newText.SetActive(level == 1);
            }
            else if (SkillController.Instance.IsEvoleSkill(skillCode))
            {
                backGroundImage.sprite = evoleBG;
                listStar[c].StarBlink();
                newText.SetActive(true);
            }
            else if (SkillController.Instance.IsActiveSkill(skillCode))
            {
                backGroundImage.sprite = activeBG;
                listStar[c].StarBlink();
                newText.SetActive(level == 1);
            }
            else
            {
                backGroundImage.sprite = activeBG;
            }
            Icon.SetNativeSize();
        }
        public void OnClick()
        {
            if (GameplayManager.Instance.CanShowGameInter)
            {
                TimerSystem.Instance.StopTimeScale();
                AdsManager.Instance.ShowInterstitial(Analytics.Interstitial_show, () => {
                    uiSelectSkill.SelectSkill(this);
                });
                GameplayManager.Instance.OnAdsInterShow();
            }
            else
            {
                uiSelectSkill.SelectSkill(this);
            }
        }
        public void SetSelect()
        {
            anim.Play("Selected");
            DebugCustom.Log("on select skill test ");
            PVPManager.Instance.Room.SendAttackSkill(3);
        }
        public void SetUnSelect()
        {
            if(gameObject.activeInHierarchy)
                anim.Play("UnSelected");
        }
    }
}

