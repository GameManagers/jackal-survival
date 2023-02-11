using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using WE.Manager;
using WE.Unit;

namespace WE.UI.Avatar
{
    public class UIAvatar : UIBase
    {
        [SerializeField] Image imgAvatar;

        [SerializeField] private Text txtName;
        [SerializeField] private Text txtId;
        [SerializeField] private Button btnRename;
        [SerializeField] TabAvatar tabAvatar;
        [SerializeField] AvatarItem avatarItemPrefab;
        public Button saveButton;

        private void Start()
        {
            tabAvatar.InitInfo(this);
            tabAvatar.DefaultSelected();
            btnRename.onClick.AddListener(OpenRenamePopup);
            saveButton.onClick.AddListener(SaveChange);
            DebugCustom.LogColor("init start ui avatar");
        }

        public override void InitUI()
        {
            DebugCustom.LogColor("ini ui avatar");

            UpdateName();

            txtId.text = "---- ---- ----";

            var userId = Context.CurrentUserPlayfabProfile.UserId;
            if(!string.IsNullOrEmpty(userId) && userId.Length >= 12)
            {
                txtId.text = "ID: " + userId.Substring(0, 4) + "-" + userId.Substring(4, 4) + "-" + userId.Substring(8, 4);
            }
            GetAvatar();
        }
        public void GetAvatar()
        {
            Sprite avatar = SpriteManager.Instance.GetSpriteAvatar(Player.Instance.CurrentAvatar);
            imgAvatar.sprite = avatar;
        }
        private void SaveChange()
        {
            if(!Context.CheckNetwork())
            {
                UIManager.Instance.ShowTextNoInternet();
                return;
            }
            tabAvatar.Save();
        }

        public AvatarItem GetPrefabItem()
        {
            return avatarItemPrefab;
        }
        public void OpenRenamePopup()
        {
            Debug.Log("On click rename ");
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