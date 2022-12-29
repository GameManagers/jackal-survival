using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MailController;
using WE.Unit;

namespace WE.UI
{
    public class UIMailDetail : UIBase
    {
        [SerializeField] private Text _txtSender;
        [SerializeField] private Text _txtTitle;
        [SerializeField] private Text _txtDes;
        [SerializeField] private Transform parent;

        [SerializeField] private GameObject gClaim, gDelete;

        [SerializeField] private GameObject gRewards;
        private string _idMail = "";
        private DataMailDetail _dataDetail;
        private int _typeMail;
        private UIMailPopup _uiMailPopup;

        [SerializeField] private ElementPack _prefabReward;

        private List<ElementPack> lstReward;

        private void Start()
        {
            _uiMailPopup = FindObjectOfType<UIMailPopup>();
        }
        public string IdMail => _idMail;


        public override void InitUI()
        {
        }

        public void LoadInfo(string idMail, int typeMail, DataMailDetail dataDetail)
        {

            _dataDetail = dataDetail;
            _idMail = idMail;
            _txtTitle.text = _dataDetail.title;
            _txtDes.text = _dataDetail.content;
            parent.gameObject.SetActive(false);
            int childs = parent.childCount;

            for (int i = 0; i < childs; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
            _typeMail = typeMail;
            _txtSender.text = _dataDetail.sender;


            /**
             * Reward hi?n t?i ch? có coin
             */



            if (_dataDetail.gifts != null && _dataDetail.gifts.Count > 0)
            {
                lstReward = new List<ElementPack>(1);

                ElementPack elementPack = Instantiate(_prefabReward);
                elementPack.transform.SetParent(parent, false);
                elementPack.gameObject.SetActive(true);

                elementPack.SetStatus(_dataDetail.status > 1);
                elementPack.item.SetValue(_dataDetail.gifts[0].value);

                lstReward.Add(elementPack);

                if (_dataDetail.status > 1)
                {
                    gClaim.SetActive(false);
                    gDelete.SetActive(true);
                }
                else
                {
                    gClaim.SetActive(true);
                    gDelete.SetActive(false);
                }

                parent.gameObject.SetActive(true);
                gRewards.SetActive(true);
            }
            else
            {
                gClaim.SetActive(false);
                gDelete.SetActive(true);
                gRewards.SetActive(false);
            }
        }

        public void OnClickClaim()
        {
            DebugCustom.LogColor("On click claim all");
            MailController.Instance.ClaimMail(_idMail, _typeMail,
            rewards =>
            {
                for (int i = 0; i < lstReward.Count; i++)
                {
                    lstReward[i].SetStatus(true);
                }
                gClaim.SetActive(false);
                gDelete.SetActive(true);

                if (rewards != null && rewards.Count > 0)
                {
                    for (int i = 0; i < rewards.Count; i++)
                    {
                        Player.Instance.AddCoin(rewards[i].value);
                    }
                }
            }, 
            () =>
            {
                DebugCustom.LogColor("Claimn Reward Error");
            });
        }

        public void OnClickDelete()
        {
            DebugCustom.LogColor("On click delete all");

            MailController.Instance.RemoveMail(_idMail, _typeMail, () =>
            {
                if (_uiMailPopup != null)
                {
                    _uiMailPopup.DeleteMail(_idMail);
                    Hide();
                }
            }, null);
        }
    }
}

