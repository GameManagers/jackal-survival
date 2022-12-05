using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WE.Unit;

namespace WE.UI
{
    public class UIHomeSetting : UIBase
    {
        public Button MusicButton;
        public Image msBtnOn;
        public Image msBtnOff;
        public Button SoundButton;
        public Image sbtnOn;
        public Image sbtnOff;
        public Button VibrateButton;
        public Image vbtnOn;
        public Image vbtnOff;
        public override void InitUI()
        {
            SoundButton.onClick.AddListener(ToggleSound);
            VibrateButton.onClick.AddListener(ToggleVibratrion);
            MusicButton.onClick.AddListener(ToggleMusic);
            SetIcon();
        }
        public void SetIcon()
        {
            msBtnOn.gameObject.SetActive(Player.Instance.GetMusicSetting());
            msBtnOff.gameObject.SetActive(!Player.Instance.GetMusicSetting());
            sbtnOn.gameObject.SetActive(Player.Instance.GetSoundSetting());
            sbtnOff.gameObject.SetActive(!Player.Instance.GetSoundSetting());
            vbtnOn.gameObject.SetActive(Player.Instance.GetVibrateSetting());
            vbtnOff.gameObject.SetActive(!Player.Instance.GetVibrateSetting());
        }
        private void OnDisable()
        {
            SoundButton.onClick.RemoveListener(ToggleSound);
            VibrateButton.onClick.RemoveListener(ToggleVibratrion);
        }
        public void ToggleSound()
        {
            Player.Instance.ToggleSoundSetting();
            SetIcon();
        }
        public void ToggleVibratrion()
        {
            Player.Instance.ToggVibrateSetting();
            SetIcon();
        }
        public void ToggleMusic()
        {
            Player.Instance.ToggleMusicSetting();
            SetIcon();
        }
    }
}


