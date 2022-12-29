using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WE.UI
{
    public class UIChangeName : UIBase
    {
        private const int PRICE_CHANGE_NAME = 1000;

        [SerializeField] private Text _txtPrice;
        [SerializeField] private GameObject gFree;
        [SerializeField] private InputField _ipfName;
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
        
        public void OnClickChange()
        {
            DebugCustom.LogColor("On change Name");
        }
    }

}
