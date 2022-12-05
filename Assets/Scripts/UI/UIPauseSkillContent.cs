using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Manager;
namespace WE.UI
{
    public class UIPauseSkillContent : MonoBehaviour
    {
        public enum SkillLotsType { 
            ActiveSkill,
            PassiveSkill,
        }
        public SkillLotsType SlotType;
        public List<UIPauseSkillSlot> listSlot;

        public void InitUI()
        {
            for (int i = 0; i < listSlot.Count; i++)
            {
                listSlot[i].InitUI(string.Empty);
            }
            switch (SlotType)
            {
                case SkillLotsType.ActiveSkill:
                    List<string> activeList = SkillController.Instance.activeSkillList;
                    for (int i = 0; i < activeList.Count; i++)
                    {
                        listSlot[i].InitUI(activeList[i]);
                    }
                    break;
                case SkillLotsType.PassiveSkill:
                    List<string> passiveList = SkillController.Instance.passiveSkillList;
                    for (int i = 0; i < passiveList.Count; i++)
                    {
                        listSlot[i].InitUI(passiveList[i]);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

