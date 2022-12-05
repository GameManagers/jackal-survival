using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WE.Manager;

namespace WE.UI
{
    public class UIPauseSkillSlot : MonoBehaviour
    {
        public Image Icon;
        public List<IconStar> listStars;
        
        public void InitUI(string skillCode)
        {
            for (int i = 0; i < listStars.Count; i++)
            {
                listStars[i].InitIcon(false);
                if (SkillController.Instance.IsEvoleSkill(skillCode))
                    listStars[i].IconShow.color = Color.red;
                else
                    listStars[i].IconShow.color = Color.white;
                listStars[i].gameObject.SetActive(false);

            }
            
            if (skillCode == string.Empty)
            {
                Icon.gameObject.SetActive(false);
            }
            else
            {
                Icon.gameObject.SetActive(true);
                Icon.sprite = SkillController.Instance.GetSkillSprite(skillCode);
                Icon.SetNativeSize();
                int maxLevel = SkillController.Instance.GetMaxLevel(skillCode);
                for (int i = 0; i < maxLevel; i++)
                {
                    listStars[i].gameObject.SetActive(true);
                }
                int currentLevel = SkillController.Instance.GetCurrentLevel(skillCode);
                for (int i = 0; i < currentLevel; i++)
                {
                    listStars[i].InitIcon(true);
                }
            }
        }
    }
}

