using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WE.Manager;

namespace WE.UI.PVP 
{
    public class UIInfoPlayerPVP : MonoBehaviour
    {
        
        [SerializeField] private Image _imgAvatar;
        [SerializeField] private Image _imgHonor;
        [SerializeField] private TextMeshProUGUI _txtHonor;
        [SerializeField] private Text _txtName;

        [SerializeField] private Image _tank;

        public void LoadInfo(ReadyPVPMessage data)
        {
            _txtName.text = data.DisplayName;
            _imgAvatar.sprite = SpriteManager.Instance.GetSpriteAvatar(data.AvatarUrl);
            ERankPVP eRankPVP = ERankPVP.Gold1;
            _imgHonor.sprite = SpriteManager.Instance.GetSpriteRankingPVP(eRankPVP);
            _imgHonor.SetNativeSize();

            _txtHonor.text = "Gold";

        }
    }

}

