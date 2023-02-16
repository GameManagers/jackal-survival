using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using DG.Tweening.Core.Easing;
using static MailController;
using WE.Unit;
using WE.Manager;

public class MailController
{
    public static MailController _instance;
    public Action<bool> actionNoti;

    public static MailController Instance
    {
        get
        {
            if (_instance == null)
                _instance = new MailController();
            return _instance;
        }
    }

    public bool ActiveNoti()
    {
        if (DataMailSystem != null && DataMailSystem.Count > 0)
        {
            foreach (var a in MailController.Instance.DataMailSystem)
            {
                var mail = a.Value;
                if (mail.status == MailStatus.NEW)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Dictionary<string, DataMail> DataMailSystem => _dataMailSystem;

    private Dictionary<string, DataMail> _dataMailSystem;
    private IEnumerator _ieGetMail;

    public void Awake()
    {

    }
    public static void Init()
    {
        _instance = new MailController();
    }

    public void GetMail(Action actionSuccess, Action actionError)
    {
        GetMailList getMailList = new GetMailList();
        UIManager.Instance.ShowWaitingCanvas(10, () => UIManager.Instance.ShowTextNotConnectServer());
        RocketIO.Instance.SendRequestMail(getMailList, 
        success =>
        {
            UIManager.Instance.HideWaitingCanvas();
            DebugCustom.LogColorJson("IEGetMail: ", success);
            try
            {
                if (_dataMailSystem == null)
                    _dataMailSystem = new Dictionary<string, DataMail>();
                else
                    _dataMailSystem.Clear();

                ListMail data = JsonConvert.DeserializeObject<ListMail>(success.Body.ToString());
                
                if(data != null)
                {
                    var mailList = data.data.OrderBy(o => o.GetEndTime_UTC()).ToList();
                    if (mailList != null && mailList.Count > 0)
                    {
                        for (int i = 0; i < mailList.Count; i++)
                        {
                            if (!_dataMailSystem.ContainsKey(mailList[i].mailId))
                                _dataMailSystem.Add(mailList[i].mailId, mailList[i]);
                            actionNoti?.Invoke(ActiveNoti());
                        }
                    }
                    actionSuccess?.Invoke();
                } 
                else
                {
                    actionError?.Invoke();
                    DebugCustom.LogError("Error");
                }
            }
            catch (Exception e)
            {
                actionError?.Invoke();
                DebugCustom.LogError(e);
            }
        },
        error =>
        {
            DebugCustom.LogError(error);
        });
    }

    public void GetMailDetail(string mailId, int typeMail, Action<DataMailDetail> actionSuccess, Action actionError)
    {
        GetMailDetail getMailDetail = new GetMailDetail();
        getMailDetail.MailId = mailId;
        getMailDetail.Type = typeMail;
        UIManager.Instance.ShowWaitingCanvas(10 , () => UIManager.Instance.ShowTextNotConnectServer());
        RocketIO.Instance.SendRequestMail(getMailDetail, success =>
        {
            UIManager.Instance.HideWaitingCanvas();
            try
            {
                DataMailDetail data = JsonConvert.DeserializeObject<DataMailDetail>(success.Body.ToString());
                if (data != null)
                {
                    actionSuccess(data);
                }
                else
                {
                    actionError?.Invoke();
                    DebugCustom.LogError("Error");
                }
            }
             catch(Exception ex)
            {
                DebugCustom.LogError(ex);

            }
        },
        error =>
        {
            DebugCustom.LogError(error);
        });
    }

    public void ClaimMail(string mailId, int typeMail, Action<List<RewardMail>> actionSuccess, Action actionError)
    {
        ClaimMail claimnMail = new ClaimMail();
        claimnMail.MailId = mailId;
        claimnMail.Type = typeMail;
        UIManager.Instance.ShowWaitingCanvas(10, () => UIManager.Instance.ShowTextNotConnectServer());
        RocketIO.Instance.SendRequestMail(claimnMail, success =>
        {
            UIManager.Instance.HideWaitingCanvas();
            try
            {
                List<RewardMail> data = JsonConvert.DeserializeObject<List<RewardMail>>(success.Body.ToString());

                if (data != null)
                {
                    actionSuccess(data);
                }
                else
                {
                    actionError?.Invoke();
                    DebugCustom.LogError("Error");
                }
            }
            catch (Exception ex)
            {
                DebugCustom.LogError(ex);

            }
        },
        error =>
        {
            DebugCustom.LogError(error);
        });
    }

    public void ClaimAllMail(Action<List<RewardMail>> actionSuccess, Action actionError)
    {
        UIManager.Instance.ShowWaitingCanvas(10, () => UIManager.Instance.ShowTextNotConnectServer());
        ClaimAllMail mail = new ClaimAllMail();
        RocketIO.Instance.SendRequestMail(mail, success =>
        {
            UIManager.Instance.HideWaitingCanvas();
            List<RewardMail> data = JsonConvert.DeserializeObject<List<RewardMail>>(success.Body.ToString());
            if (data != null)
            {
                actionSuccess(data);
            }
        },
        error =>
        {
            actionError?.Invoke();
            DebugCustom.LogError(error);
        });
    }

    public void RemoveMail(string mailId, int typeMail, Action actionSuccess, Action actionError)
    {
        UIManager.Instance.ShowWaitingCanvas(10, () => UIManager.Instance.ShowTextNotConnectServer());
        DeleteMail mail = new DeleteMail();
        mail.MailId = mailId;
        mail.Type = typeMail;
        RocketIO.Instance.SendRequestMail(mail, success =>
        {
            UIManager.Instance.HideWaitingCanvas();
            DebugCustom.LogColorJson("IERemoveMail: ", success);
            _dataMailSystem.Remove(mailId);
            actionSuccess?.Invoke();
        },
        error =>
        {
            actionError?.Invoke();
            DebugCustom.LogError(error);
        });
    }

    public void RemoveAllMail(Action actionSuccess, Action actionError)
    {
        UIManager.Instance.ShowWaitingCanvas(10, () => UIManager.Instance.ShowTextNotConnectServer());
        DeleteAllMail mail = new DeleteAllMail();
        RocketIO.Instance.SendRequestMail(mail, success =>
        {
            UIManager.Instance.HideWaitingCanvas();
            DebugCustom.LogColorJson("IERemoveMail: ", success);
            _dataMailSystem.Clear();
            actionSuccess?.Invoke();
        },
        error =>
        {
            actionError?.Invoke();
            DebugCustom.LogError(error);
        });
    }

    [Serializable]
    public class ListMail
    {
        public List<DataMail> data;
    }

    [Serializable]
    public class DataMail
    {
        public string mailId;
        public MailStatus status;
        public string title;
        public long timeEnd;
        public int type;

        public DateTime GetEndTime_UTC()
        {
            return DateTimeHelper.ParseUnixTimestampNormal(timeEnd);
        }
    }

    [Serializable]
    public class DataMailDetail
    {
        public string sender;
        public int status;
        public string title;
        public string content;
        public List<RewardMail> gifts;
        public long timeEnd;
        public int type;
    }

    public class RewardMail
    {
        public string key;
        public int value;
    }

}
