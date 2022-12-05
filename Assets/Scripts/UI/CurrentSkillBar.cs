using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WE.Manager;
namespace WE.UI
{
    public class CurrentSkillBar : MonoBehaviour
    {
        public Image[] activeIconSlots;
        public Image[] passiveIconSlots;
        public void InitUI()
        {
            for (int i = 0; i < activeIconSlots.Length; i++)
            {
                activeIconSlots[i].gameObject.SetActive(false);
                passiveIconSlots[i].gameObject.SetActive(false);
            }
            List<string> activeList = SkillController.Instance.activeSkillList;
            List<string> passiveList = SkillController.Instance.passiveSkillList;
            for (int i = 0; i < activeList.Count; i++)
            {
                activeIconSlots[i].gameObject.SetActive(true);
                activeIconSlots[i].sprite = SkillController.Instance.GetSkillSprite(activeList[i]);
            }
            for (int i = 0; i < passiveList.Count; i++)
            {
                passiveIconSlots[i].gameObject.SetActive(true);
                passiveIconSlots[i].sprite = SkillController.Instance.GetSkillSprite(passiveList[i]);
            }
        }
    }
}

