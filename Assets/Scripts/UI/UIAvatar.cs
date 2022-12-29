using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using WE.Manager;

namespace WE.UI
{
    public class UIAvatar : UIBase
    {
        [SerializeField] private Text txtName;
        [SerializeField] private Text txtId;
        [SerializeField] private Button btnRename;

        public override void InitUI()
        {
            btnRename.onClick.AddListener(OpenRenamePopup);

        }

        public void OpenRenamePopup()
        {
            Debug.Log("ON click rename ");
            UIManager.Instance.ShowPopupRename();
        }
    }
}