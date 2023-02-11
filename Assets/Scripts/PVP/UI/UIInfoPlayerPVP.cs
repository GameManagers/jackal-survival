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
        [SerializeField] private Image _gun;

        [SerializeField] private GameObject info;
        [SerializeField] private Image _imgLoading;

        
        public void LoadInfo(ReadyPVPMessage data)
        {
            _txtName.text = data.DisplayName;
            _imgAvatar.sprite = SpriteManager.Instance.GetSpriteAvatar(data.AvatarUrl);
            ERankPVP eRankPVP = DataManager.Instance.dataRewardPVP.GetRankPvp(data.Elo);
            _imgHonor.sprite = SpriteManager.Instance.GetSpriteRankingPVP(eRankPVP);
            _txtHonor.text = DataManager.Instance.dataRewardPVP.GetLabelRank(eRankPVP);
            info.SetActive(true);
            _imgLoading.gameObject.SetActive(false);

            var vehicleVisual = CarController.Instance.dataVehicle.CarVisualizeDict[data.TankId];
            _tank.sprite = vehicleVisual.iconCar.sprite;
            _gun.sprite = vehicleVisual.iconGun.sprite;
        }
        public void HideOpponentInfo()
        {
            info.SetActive(false);
            _imgLoading.gameObject.SetActive(true);
        }
    }

}

