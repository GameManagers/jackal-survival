using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WE.Manager;
using WE.Unit;

namespace WE.UI.Avatar
{
    public class TabAvatar : MonoBehaviour
    {
        [SerializeField] Image img;
        [SerializeField] Transform parent;
        List<AvatarItem> listItems;
        AvatarItem ItemSeleceted;

        UIAvatar _uIAvatarPopup;
        TypeAvatar _currentAvatar;
        public void InitInfo(UIAvatar uiAvatar)
        {
            _uIAvatarPopup = uiAvatar;
            listItems = new List<AvatarItem>();
            _currentAvatar = Player.Instance.CurrentAvatar;
            
            foreach (TypeAvatar avatar in Enum.GetValues(typeof(TypeAvatar)))
            {
                AvatarItem item = Instantiate(_uIAvatarPopup.GetPrefabItem(), parent) as AvatarItem;
                item.Init(avatar, OnChange);
                listItems.Add(item);
            }
        }

        public void DefaultSelected()
        {
            foreach (TypeAvatar avatar in Enum.GetValues(typeof(TypeAvatar)))
            {
                
                if(avatar == Player.Instance.CurrentAvatar)
                {
                    ItemSeleceted = listItems[(int)avatar];
                    ItemSeleceted.Select();
                }
            }
        }

        /**
         * Request change current avatar to server
         */
        public void Save()
        {
            if (_currentAvatar != Player.Instance.CurrentAvatar)
            {
                SetAvatarRequest request = new SetAvatarRequest();
                request.Avatar = _currentAvatar;

                RocketIO.Instance.SendMessageG(request,
                      success =>
                      {
                          Player.Instance.ChangeAvatar(_currentAvatar);
                          DebugCustom.LogColorJson("Doi avatar thanh cong", success);
                      },
                      error =>
                      {
                          UIManager.Instance.ShowTextNoInternet();
                      });
            }
        }

        private void OnChange(TypeAvatar avatar, AvatarItem item)
        {
            DebugCustom.LogColor("OnChangeAvatar", avatar);
            _currentAvatar = avatar;
            img.sprite = SpriteManager.Instance.GetSpriteAvatar(avatar);
            ItemSeleceted.UnSelect();
            ItemSeleceted = item;
            item.Select();
        }

        private void RequestChangeAvatar(TypeAvatar avatar)
        {

        }
    }

}
