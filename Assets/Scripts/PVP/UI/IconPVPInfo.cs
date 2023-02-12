using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WE.Manager;
using WE.Unit;

namespace WE.UI.PVP
{
    public class IconPVPInfo : MonoBehaviour
    {
        [SerializeField]
        private Text lbPlayerName;
        [SerializeField]
        private Transform iconHpBar;
        [SerializeField]
        private TextMeshProUGUI lbValueHPBar;
        [SerializeField]
        private Image iconAvatar;

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void InitInfo()
        {
            iconHpBar.transform.localScale = new Vector3(1, 1, 1);
            LoadAvatar(Player.Instance.CurrentAvatar);
            LoadName(Context.CurrentUserPlayfabProfile.DisplayName);
            UpdateValueHP(1f, 1f);
        }

        public void InitInfo(ReadyPVPMessage opponents)
        {
            iconHpBar.transform.localScale = new Vector3(1, 1, 1);
            LoadName(opponents.DisplayName);
            LoadAvatar(opponents.AvatarUrl);
            UpdateValueHP(1f, 1f);
        }

        private void LoadAvatar(TypeAvatar _avatar)
        {
            iconAvatar.sprite = SpriteManager.Instance.GetSpriteAvatar(_avatar);
        }

        private void LoadName(string _name)
        {
            if (_name == null)
                return;
            if (_name.Length > 18)
                _name = _name.Substring(0, 18) + "...";
            lbPlayerName.text = _name;
        }

        public void UpdateValueHP(float _currentValue, float _maxHp)
        {
            float percent = _currentValue / _maxHp;
            if (percent < 0f)
                percent = 0f;
            if (percent > 1f)
                percent = 1f;
            iconHpBar.transform.localScale = new Vector3(percent, 1f, 1f);
            float strPercent = percent * 100f;
            if (strPercent < 1f && strPercent > 0f)
            {
                strPercent = 1f;
                lbValueHPBar.text = $"{strPercent}%";
            }
            else
            {
                lbValueHPBar.text = $"{Mathf.RoundToInt(strPercent)}%";
            }
        }
    }

}
