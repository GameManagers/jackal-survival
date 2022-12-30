using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using WE.Manager;
using WE.Unit;

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

            UpdateName();

            txtId.text = "---- ---- ----";

            var userId = Context.CurrentUserPlayfabProfile.UserId;
            if(!string.IsNullOrEmpty(userId) && userId.Length >= 12)
            {
                txtId.text = "ID: " + userId.Substring(0, 4) + "-" + userId.Substring(4, 4) + "-" + userId.Substring(8, 4);
            }
        }

        public void OpenRenamePopup()
        {
            Debug.Log("ON click rename ");
            UIManager.Instance.ShowPopupRename();
        }
        public void UpdateName()
        {
            txtName.text = Context.CurrentUserPlayfabProfile.DisplayName;
        }

        public void CopyId()
        {
            GUIUtility.systemCopyBuffer = Context.CurrentUserPlayfabProfile.UserId;
        }
    }
}