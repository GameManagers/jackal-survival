using Dragon.SDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WE.Manager;
using WE.Pooling;
using WE.Support;
using WE.Unit;
using WE.Utils;

namespace WE.UI
{
    public class UISelectSkill : UIBase
    {
        
        public SkillSlotItem[] slotItems;

        public CurrentSkillBar skillBar;
        public GameObject rerollButton;

        bool Selected;
        string currentCode;

        public override void InitUI()
        {
            Selected = false;
            skillBar.InitUI();
            for (int i = 0; i < slotItems.Length; i++)
            {
                slotItems[i].uiSelectSkill = this;
                slotItems[i].gameObject.SetActive(false);
            }
            if (GameplayManager.Instance.CurrentGameplayType == GameType.Tutorial)
            {
                int level = GameplayManager.Instance.CurrentLevel - 1;
                rerollButton.SetActive(false);
                List<string> listSkill = SkillController.Instance.GetTutSkill(level);
                for (int i = 0; i < listSkill.Count; i++)
                {
                    slotItems[i].gameObject.SetActive(true);
                    slotItems[i].InitSkill(listSkill[i]);
                }
            }
            else
            {
                Dictionary<string, float> dicRate = SkillController.Instance.GetAviableSkillRateDic();
                rerollButton.SetActive(dicRate.Count > 3);
                if (dicRate.Count == 0)
                {
                    slotItems[0].gameObject.SetActive(true);
                    slotItems[0].InitSkill(Constant.COIN_CHEST);
                    slotItems[1].gameObject.SetActive(true);
                    slotItems[1].InitSkill(Constant.HP_CHEST);
                }
                else
                {
                    if (Helper.IsLuckApply() && dicRate.Count > 3)
                    {
                        ReRoll();
                    }
                    else
                    {

                        if (dicRate.Count < 3)
                        {
                            for (int i = 0; i < dicRate.Count; i++)
                            {
                                slotItems[i].gameObject.SetActive(true);
                                slotItems[i].InitSkill(dicRate.ElementAt(i).Key);
                            }
                        }
                        else
                        {
                            List<string> listSkill = Helper.GetRandomListByPercent<string>(dicRate, 3);
                            for (int i = 0; i < 3; i++)
                            {
                                slotItems[i].gameObject.SetActive(true);
                                slotItems[i].InitSkill(listSkill[i]);
                            }
                        }
                    }
                }
                
            }
            
        }
        public override void ActionAfterShow()
        {
            base.ActionAfterShow();
            if (GameplayManager.Instance.CurrentGameplayType == GameType.Tutorial)
            {

                int level = GameplayManager.Instance.CurrentLevel - 1;
                if (level == 1)
                {
                    TigerForge.EventManager.EmitEvent(Constant.TUT_ON_FIRST_LEVEL_REACH);
                }
                else if (level == 2)
                {

                    TigerForge.EventManager.EmitEvent(Constant.TUT_ON_SECONDS_LEVEL_REACH);
                }
                else if (level == 3)
                {

                    TigerForge.EventManager.EmitEvent(Constant.TUT_ON_THIRD_LEVEL_REACH);
                }
                for (int i = 0; i < slotItems.Length; i++)
                {
                    if (slotItems[i].isEvoleSkill)
                    {

                        TigerForge.EventManager.EmitEvent(Constant.TUT_ON_FIRST_SKILL_MAX);
                        break;
                    }
                }
            }
        }
        public void SelectSkill(SkillSlotItem item)
        {
            if (!Selected)
            {
                Selected = true;
                for (int i = 0; i < slotItems.Length; i++)
                {
                    if (slotItems[i] == item)
                        item.SetSelect();
                    else
                        slotItems[i].SetUnSelect();
                }
            }
            currentCode = (item.currentCode);
            Hide();
        }

                /**
         * Comment no ads
         */
        public void OnClick()
        {
            if (Selected)
                return;
            //AdsManager.Instance.ShowRewardedAd(() => {
            //    ReRoll();
            //    GameplayManager.Instance.OnAdsInterShow();
            //}, Analytics.rewarded_video_show);
        }
        public void ReRoll()
        {
            List<string> listSkill = SkillController.Instance.GetListSkillReRoll();
            for (int i = 0; i < listSkill.Count - 1; i++)
            {
                var c = listSkill[i];
                int rand = UnityEngine.Random.Range(0, listSkill.Count);
                listSkill[i] = listSkill[rand];
                listSkill[rand] = c;
            }
            for (int i = 0; i < listSkill.Count; i++)
            {
                slotItems[i].gameObject.SetActive(true);
                slotItems[i].InitSkill(listSkill[i]);
            }
        }
        public override void AfterHideAction()
        {

            Helper.SpawnEffect(ObjectPooler.Instance.fxReviceSkill, Player.Instance.transform.position, Player.Instance.transform);
            if (currentCode == Constant.COIN_CHEST)
            {
                GameplayManager.Instance.ReviceItem(EItemInGame.Big_Coin);
            }
            else if (currentCode == Constant.HP_CHEST)
            {
                GameplayManager.Instance.ReviceItem(EItemInGame.Heal);
            }
            else
            {
                SkillController.Instance.AddSkillByString(currentCode);
            }
        }
    }
}

