using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WE.Manager;

namespace WE.UI.Avatar
{
    public class AvatarItem : MonoBehaviour
    {
        public Image AvatarImage;
        public Button Button;
        public GameObject objChoose;
        private void OnDisable()
        {
            objChoose.SetActive(false);
        }

        public void Init(TypeAvatar _avatar, System.Action<TypeAvatar, AvatarItem> OnSelectItem)
        {
            AvatarImage.sprite = SpriteManager.Instance.GetSpriteAvatar(_avatar);
            AvatarImage.raycastTarget = true;
            Button.onClick.AddListener(() =>
            {
                OnSelectItem?.Invoke(_avatar, this);
            });
        }

        public void Select()
        {
            objChoose?.SetActive(true);
        }
        public void UnSelect()
        {
            objChoose.SetActive(false);
        }
    }
}

