using Castle.Core.Internal;
using DG.Tweening.Core.Easing;
using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WE.Unit;

namespace WE.UI
{
    public class UIChangeName : UIBase
    {
        private const int PRICE_CHANGE_NAME = 1000;

        [SerializeField] private Text _txtPrice;
        [SerializeField] private GameObject gFree;
        [SerializeField] private InputField _ipfName;
        [SerializeField] private Text txtWarning;

        public Color ColorWarning;

        int turnChangeName = 0;

        public override void InitUI()
        {
            turnChangeName = CPlayerPrefs.GetInt("TURN_CHANGE_NAME", 0);
            if (turnChangeName == 0)
            {
                gFree.SetActive(true);
                _txtPrice.gameObject.SetActive(false);
            }
            else
            {
                gFree.SetActive(false);
                _txtPrice.gameObject.SetActive(true);
                _txtPrice.text = (turnChangeName * PRICE_CHANGE_NAME).ToString();
            }
        }

        public void OnClickChangeName()
        {

            if (Player.Instance.currentCoin < PRICE_CHANGE_NAME * turnChangeName) return;
            
            string strName = _ipfName.text.Trim();

            if (string.IsNullOrEmpty(strName) || strName.Trim().Length < 6)
            {
                Debug.Log(strName);
                Debug.Log("mot enough");
                txtWarning.text = "User name must be at least 6 charater";
            //    txtWarning.color = ColorWarning;
                return;
            }
            if (strName.Contains(" "))
            {
                txtWarning.text = "User name can't contain space";
         //       txtWarning.color = ColorWarning;
                return;
            }
            if (Context.CheckNetwork())
                RequestChangeName(strName);
        }

        private void RequestChangeName(string userName)
        {
            DebugCustom.LogColor("On change Name");
            SetNameRequest request = new SetNameRequest();
            request.DisplayName = userName;
            RocketIO.Instance.SendMessageG(request,
                success =>
                {
                    DebugCustom.LogColorJson("RequestChangeNameServer", success);
                    ChangeNameSuccess(success, userName);
                },
                error =>
                {
                    ChangeNameError(error);
                });
        }

        private void ChangeNameSuccess(MessageResponse msg, string userName)
        {
            Player.Instance.AddCoin(-PRICE_CHANGE_NAME * turnChangeName);
            Context.CurrentUserPlayfabProfile.DisplayName = userName;
            if (turnChangeName < 5)
            {
                turnChangeName++;
            }
            CPlayerPrefs.SetInt("TURN_CHANGE_NAME", turnChangeName);
            FindObjectOfType<WE.UI.UIAvatar>()?.UpdateName();
            Hide();
            Player.Instance.ChangeName();
        }

        private void ChangeNameError(MessageError error)
        {
            DebugCustom.LogErrorJson(error);
        }

    }

}
