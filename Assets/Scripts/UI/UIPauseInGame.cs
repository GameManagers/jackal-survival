using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Manager;
using WE.Unit;
using WE.Utils;

namespace WE.UI
{
    public class UIPauseInGame : UIBase
    {
        public UIPauseSkillContent[] skillContents;

        public GameObject IconSoundOff;
        public GameObject IconMusicOff;
        public GameObject IconVibrationOff;
        public GameObject IconHome;

        public Animator popupConfirmAnim;

        public override void InitUI()
        {
            //IconHome.SetActive(GameplayManager.Instance.CurrentGameplayType != GameType.Tutorial);
            InitSound();
            for (int i = 0; i < skillContents.Length; i++)
            {
                skillContents[i].InitUI();
            }
            TigerForge.EventManager.StartListening(Constant.ON_SOUND_SETTING_CHANGE, InitSound);
        }
        public void InitSound()
        {
            IconSoundOff.SetActive(!Player.Instance.GetSoundSetting());
            IconMusicOff.SetActive(!Player.Instance.GetMusicSetting());
            IconVibrationOff.SetActive(!Player.Instance.GetVibrateSetting());
        }
        public void Continute()
        {
            Hide();
        }
        public void GoHomeClicked()
        {
            popupConfirmAnim.Play("Open");
        }
        public void OnQuitClicked()
        {
            Hide();
            GameplayManager.Instance.ShowPopupEndGame(false);
        }
        public void OnReturnClicked()
        {
            popupConfirmAnim.Play("Close");
        }
        public void ToggleSound()
        {
            Player.Instance.ToggleSoundSetting();
            //InitSound();
        }
        public void ToggleMusic()
        {
            Player.Instance.ToggleMusicSetting();
        }
        public void ToggVibration()
        {
            Player.Instance.ToggVibrateSetting();
        }
        private void OnDisable()
        {
            TigerForge.EventManager.StopListening(Constant.ON_SOUND_SETTING_CHANGE, InitSound);
        }
    }
}

