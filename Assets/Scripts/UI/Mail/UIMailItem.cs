using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using static MailController;
using System;
using WE.Manager;

namespace WE.UI
{
    public class UIMailItem : MonoBehaviour
    {
        [SerializeField] private GameObject _gNoti, _gOldMail, _gNewMail;
        [SerializeField] private Text _txtTitle;
        [SerializeField] private Text _txtTime;
        private string _idMail;
        private int _type;
        private double _timeRemain;
        public double TimeRemain => _timeRemain;
        public string MailID => _idMail;

        public void InitInfo(DataMail dataMail)
        {
            bool isRead = dataMail.status == MailStatus.NEW ? false : true;
            _txtTitle.text = dataMail.title;
            _idMail = dataMail.mailId;
            _type = dataMail.type;
            UpdateStatus(isRead);

            _txtTime.text = GetTextRemainTime(dataMail.GetEndTime_UTC());
        }

        private string GetTextRemainTime(DateTime time)
        {
            _timeRemain = DateTimeHelper.ToUnixTimestampNormal(time) - DateTimeHelper.ToUnixTimestampNormal(DateTime.UtcNow);
           
            if (_timeRemain > 86400)
            {
                return Math.Round(_timeRemain / 86400).ToString() + "d to expiry";
            }
            else if(_timeRemain > 3600)
            {
                return Math.Round(_timeRemain / 3600).ToString() + "h to expiry";
            }
            else if(_timeRemain > 0)
            {
                return Math.Round(_timeRemain / 60).ToString() + "m to expiry";
            }
            return "Out of Date";
        }

        public void UpdateTime(float seconds)
        {
            _timeRemain -= seconds;
        }

        public void UpdateStatus(bool isRead)
        {
            _gOldMail.SetActive(isRead);
            _gNewMail.SetActive(!isRead);
            _gNoti.SetActive(!isRead);
        }

        public void OnClickMail()
        {
            MailController.Instance.GetMailDetail(_idMail, _type, data =>
            {
                var dataMailSystem = MailController.Instance.DataMailSystem;
                if (dataMailSystem.ContainsKey(_idMail))
                {
                    if (dataMailSystem[_idMail].status == MailStatus.NEW)
                        dataMailSystem[_idMail].status = MailStatus.READ;
                }
                UIManager.Instance.OpenPopupMailDetail(_idMail, _type, data);
                _gNoti.SetActive(false);
                _gOldMail.SetActive(true);
                _gNewMail.SetActive(false);
            },
            () =>
            {
                Debug.Log("Error Get Mail Details");
            });
        }
    }
}
