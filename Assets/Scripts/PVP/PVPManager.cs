using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using WE.Manager;
using WE.Unit;

namespace WE.PVP.Manager
{
    public class PVPManager : MonoBehaviour
    {
        public TimeSpan TimeRemain { private set; get; }
        private static PVPManager _instance;
        private IEnumerator _ieMatchMaking;
        private PVPStateMachine _stateMachine;
        private PVPConfig _pvpConfig;
        private PVPRoomController _room;
        private int _countMatching = 0;
        private bool isActive = false;

        public ReadyPVPMessage DataPlayer;
        public ReadyPVPMessage DataOtherPlayer;
        public PreparePVPMessage DataMap;
        public static PVPManager Instance => _instance;

        public PVPRoomController Room => _room;
        public PVPStateMachine StateMachine => _stateMachine;

        public int CountMatching
        {
            get
            {
                return _countMatching;
            }
            set
            {
                _countMatching = value;
            }
        }
        private void Awake()
        {
            _instance = this;
            _countMatching = 0;
        }

        public PVPConfig Config
        {
            get
            {
                if (_pvpConfig == null)
                    _pvpConfig = new PVPConfig();
                return _pvpConfig;
            }
        }

        public void InitStateMachine()
        {
            _stateMachine = new PVPStateMachine();
            _stateMachine.InitStateMachine();
        }
        public void InitClient()
        {
            _room = new PVPRoomController(Config.URL);
            _room.OnReconnect = _ => { _stateMachine.TriggerReconnect(); };
            //_room.OnEndGamePVPByDisconnect = () => { GameManager.Instance.OpenHomeUI(); };
        }

        public static void Init()
        {
            _instance = new PVPManager();
        }
        public async Task<bool> JoinRoom(int type, string roomName, ReadyPVPMessage data, Action<ErrorMatching> actionError = null)
        {
            bool isSuccess = await _room.JoinRoom(type, roomName, data, actionError);
            if (isSuccess)
            {
                if (DataPlayer != null)
                {
                    DataPlayer.SessionId = Room.Room.SessionId;
                }
            }
            return isSuccess;
        }

        private IEnumerator IEMatchMaking(Action<MatchMakingPVPResponse> actionDone = null, Action<string> actionError = null)
        {
            WWWForm form = new WWWForm();
            form.AddField("room", "pvp");
            form.AddField("Elo", Context.PvPBattlePoint);
            form.AddField("Atk", Player.Instance.AttackDamage.ToString());
            using (UnityWebRequest www = UnityWebRequest.Post(Config.URL_HTTP, form))
            {
                yield return www.SendWebRequest();
                DebugCustom.LogColorJson(www.downloadHandler.text);
                if (www != null && string.IsNullOrEmpty(www.error))
                {
                    try
                    {
                        MatchMakingPVPResponse response = JsonConvert.DeserializeObject<MatchMakingPVPResponse>(www.downloadHandler.text);
                        if (response != null)
                        {
                            actionDone?.Invoke(response);
                        }
                        else
                        {
                            actionError?.Invoke("Error");
                            Debug.LogError("Error");
                        }
                    }
                    catch (Exception ex)
                    {
                        actionError?.Invoke("Error");
                        Debug.LogError(ex);
                    }
                }
                else
                {
                    actionError?.Invoke(www.error);
                    Debug.LogError(www.error);
                }
            }
        }
        public void MatchMaking(Action<MatchMakingPVPResponse> actionDone = null, Action<string> actionError = null)
        {
            if (_ieMatchMaking != null)
            {
                StopCoroutine(_ieMatchMaking);
            }
            _ieMatchMaking = IEMatchMaking(actionDone, actionError);
            StartCoroutine(_ieMatchMaking);
        }

        public void GetLeaderBoard(Action<DataPVPRanking> doneCallback, Action<MessageError> ErrorCallback = null)
        {
            UIManager.Instance.ShowWaitingCanvas();
            LeaderBoardPVP request = new LeaderBoardPVP();
            RocketIO.Instance.SendMessageG(request, success =>
            {
                DebugCustom.LogColorJson("GetLeaderBoardPVP", success);
                UIManager.Instance.HideWaitingCanvas();
                if (doneCallback != null)
                {
                    DataPVPRanking info = Newtonsoft.Json.JsonConvert.DeserializeObject<DataPVPRanking>(success.Body.ToString());
                    doneCallback?.Invoke(info);
                }
            }, error => {
                UIManager.Instance.HideWaitingCanvas();
                ErrorCallback?.Invoke(error);
            } );
        }

        public void InitTimeRemain(long EndTime, long CurrentTime)
        {

            DateTime endTime = DateTimeHelper.ParseUnixTimestampNormal(EndTime);
            DateTime currentUTC = DateTimeHelper.ParseUnixTimestampNormal(CurrentTime);
            TimeRemain = endTime - currentUTC;
            isActive = true;
        }
        public bool IsInRoom
        {
            get { return _room != null && _room.Room != null && Application.internetReachability != NetworkReachability.NotReachable; }
        }
        public bool InRoom()
        {
            return _room != null && _room.Room != null;
        }
        private void OnDestroy()
        {
            _stateMachine?.Dispose();
        }
        private void OnDisable()
        {
            if (Room != null)
                Room.LeaveRoom(false);
        }
    }

}
