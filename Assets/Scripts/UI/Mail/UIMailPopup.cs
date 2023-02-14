using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;
using WE.Unit;

namespace WE.UI
{
    public class UIMailPopup : UIBase
    {
        [SerializeField] private UIMailItem _prefMailItem;
        [SerializeField] private RectTransform _content;
        [SerializeField] private List<UIMailItem> _mails;
        [SerializeField] private GameObject gDeleteAll, gClaimAll, gNoMail;

        private bool isReloadingMail = false;
        private float tmpLastTimeInApp;
        private bool isFocusApp = false;
        private float tmpTime = 0;

        public override void InitUI()
        {

        }

        private void Start()
        {
            isReloadingMail = false;
            isFocusApp = false;
        }

        public void InitMail()
        {
            int countMailSystem = 0;
            var dataMailSystem = MailController.Instance.DataMailSystem;
            if (dataMailSystem != null)
                countMailSystem = dataMailSystem.Count;
            if (_mails == null)
                _mails = new List<UIMailItem>();
            else
            {
                int count = _mails.Count;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        DeleteMail(0);
                    }
                }
            }

            if (countMailSystem > 0)
            {
                foreach (var a in dataMailSystem)
                {
                    var mail = Instantiate(_prefMailItem);
                    mail.transform.SetParent(_content, false);
                    mail.InitInfo(a.Value);
                    _mails.Add(mail);
                }
                gDeleteAll.SetActive(true);
                gClaimAll.SetActive(true);
                gNoMail.SetActive(false);
            }
            else
            {
                gDeleteAll.SetActive(false);
                gClaimAll.SetActive(false);
                gNoMail.SetActive(true);
            }
        }

        public void DeleteMail(int index)
        {
            if (_mails != null && _mails.Count > index)
            {
                var mail = _mails[index];
                _mails.RemoveAt(index);
                DestroyImmediate(mail.gameObject);
            }
        }

        public void DeleteMail(string idMail)
        {
            if (_mails != null)
            {
                for (int i = 0; i < _mails.Count; i++)
                {
                    if (string.Equals(_mails[i].MailID, idMail))
                    {
                        DeleteMail(i);
                        break;
                    }
                }
            }
        }

        public void ClaimAllMail()
        {
            MailController.Instance.ClaimAllMail(
               rewards =>
               {
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

        public void DeleteAllMail()
        {
            this.ProcessDeleteAllMail();
        }

        private void ProcessDeleteAllMail()
        {
            MailController.Instance.RemoveAllMail(
            () =>
            {
                int count = _mails.Count;
                for (int i = 0; i < count; i++)
                {
                    DeleteMail(0);
                }
                gDeleteAll.SetActive(false);
                gClaimAll.SetActive(false);
                gNoMail.SetActive(true);
            }, null);
        }

        private void LateUpdate()
        {
            if (!isReloadingMail && !isFocusApp)
            {
                tmpTime += Time.unscaledDeltaTime;
                if (tmpTime >= 1)
                {
                    tmpTime -= 1;
                    UpdateTimeMail(1);
                }
            }
        }

        private void UpdateTimeMail(float seconds) 
        {
            for(int i = 0; i < _mails.Count; i++)
            {
                _mails[i].UpdateTime(seconds);
                if (_mails[i].TimeRemain < -5)
                {
                    isReloadingMail = true;
                    MailController.Instance.GetMail(
                    () =>
                    {
                        InitMail();
                        isReloadingMail = false;
                        var uiDetail = FindObjectOfType<UIMailDetail>();
                        if (uiDetail != null)
                        {
                            var dataMailSystem = MailController.Instance.DataMailSystem;
                            if (dataMailSystem != null && !dataMailSystem.ContainsKey(uiDetail.IdMail))
                            {
                                uiDetail.Hide();
                            }
                        }
                    }, 
                    () =>
                    {
                        var uiDetail = FindObjectOfType<UIMailDetail>();
                        if (uiDetail != null)
                        {
                            uiDetail.Hide();
                        }
                        Hide();
                    });
                }
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                StopCoroutine(IEFocusApp());
                tmpLastTimeInApp = Time.realtimeSinceStartup;
            }
            else
            {
                float deltaTime = Time.realtimeSinceStartup - tmpLastTimeInApp;
                UpdateTimeMail(deltaTime);
                StartCoroutine(IEFocusApp());
            }
        }

        private IEnumerator IEFocusApp()
        {
            yield return new WaitForEndOfFrame();
            isFocusApp = false;
        }

        public override void AfterHideAction()
        {
            base.AfterHideAction();
            MailController.Instance.actionNoti.Invoke(MailController.Instance.ActiveNoti());
        }
    }

}
