using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Colyseus;
using WE.PVP.Schemas;
using WE.PVP.Manager;
using DG.Tweening.Core.Easing;
using WE.PVP;

public class PVPRoomController : MonoBehaviour
{
    public Action OnLeftPVP;
    public Action<ReadyPVPMessage> OnReadyPVP;
    public Action<PreparePVPMessage> OnPreparePVP;
    public Action<StartGamePVPMessage> OnStartGamePVP;
    public Action<EndGameMessage> OnEndGamePVP;
    public Action OnEndGamePVPByDisconnect;
    public Action<int> OnAttackHide;    
    public Action<int, int> OnScoreChange;
   
    public Action<int> OnChangeTimePlay;
    public Action<float, float> OnHpChange;
    public Action<int> OnReconnect;
    public EndGameMessage DataEndGame => _dataEndgame;
    public bool OtherPlayerLeft => _otherPlayerLeft;

    private ColyseusClient _client;
    private ColyseusRoom<BaseRoomState> _room;
    private Action<GetScorePVPMessage> _onUpdateScore;
    private Action<AttackSkillMessage> _onAttackSkill;

    private string _roomId;
    private string _sessionId;
    private bool _isHaveReconnect = false;
    private EndGameMessage _dataEndgame;
    private bool _otherPlayerLeft = false;
    public ColyseusRoom<BaseRoomState> Room => _room;
    public PVPRoomController(string url)
    {
        _client = new ColyseusClient(url);
        _isHaveReconnect = false;
        _dataEndgame = null;
        _otherPlayerLeft = false;
        RemoveHandle();
    }
    public async Task<bool> JoinRoom(int type, string roomName, ReadyPVPMessage dataPlayer, Action<ErrorMatching> actionError)
    {
        try
        {
            _dataEndgame = null;
            _otherPlayerLeft = false;
            if (_room != null)
            {
                await _room.Leave(true);
                _room = null;
            }
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("Atk", dataPlayer.Atk);
            options.Add("Elo", dataPlayer.Elo);

            switch (type)
            {
                case 0:
                    {
                        _room = await _client.JoinOrCreate<BaseRoomState>(roomName, options);
                        break;
                    }
                case 1:
                    {
                        _room = await _client.JoinById<BaseRoomState>(roomName, options);
                        break;
                    }
                case 2:
                    {
                        _room = await _client.Create<BaseRoomState>(roomName, options);
                        break;
                    }
                default:
                    {

                        _room = await _client.JoinOrCreate<BaseRoomState>(roomName, options);
                        break;
                    }
            }

            _room.OnLeave += OnLeave;
            _roomId = _room.Id;
            _sessionId = _room.SessionId;
            InitHandle();
            DebugCustom.LogColor("joined successfully");
            return true;
        }
        catch (Exception ex)
        {
            Debug.Log("join error");
            ErrorMatching errorMatching = new ErrorMatching();
            errorMatching.ErrorCode = 1000;
            errorMatching.Message = ex.Message;
            actionError?.Invoke(errorMatching);
            return false;
        }
    }
    public void SendStartGame()
    {
        if (_room != null)
        {
            _ = _room.Send("GAME_START", new StartGamePVPMessage());

            DebugCustom.LogColor("SEND_GAME_START");
        }
    }

    public void SendScore(SendScorePVPMessage data)
    {
        if (_room != null)
        {
            _ = _room.Send("SEND_GAME_SCORE", data);
        }
    }

    public void SendPlayerDie()
    {
        if (_room != null)
        {
            _ = _room.Send("PLAYER_DIE", new PlayerDieMessage());
            DebugCustom.LogColor("SEND_PLAYER_DIE");
        }
        else
            DebugCustom.LogColor("Room Null Can not SEND_PLAYER_DIE");
    }

    public void SendAttackSkill(int type)
    {
        if (_room != null)
        {
            AttackSkillMessage atkMsg = new AttackSkillMessage();
            atkMsg.Type = type;
            _ = _room.Send("ATTACK_SKILL", atkMsg);
            DebugCustom.LogColor("ATTACK_SKILL");
        }
        else
            DebugCustom.LogColor("Room Null Can not SEND_PLAYER_DIE");
    }

    public async void Reconnect(Action actionSuccess, Action actionError)
    {
        try
        {
            _room = await _client.Reconnect<BaseRoomState>(_roomId, _sessionId);
            _roomId = _room.Id;
            _sessionId = _room.SessionId;
            _room.OnLeave += OnLeave;
            InitHandle();
            _isHaveReconnect = true;
            actionSuccess?.Invoke();
        }
        catch (Exception ex)
        {
            actionError?.Invoke();
        }
    }

    public async void LeaveRoom(bool isHaveReconnect, bool consented = true)
    {
        _isHaveReconnect = isHaveReconnect;
        if (_room == null)
            return;
        try
        {
#if UNITY_IOS
            OnLeave(1006);
#endif
            await _room.Leave(consented);         
        }
        catch (Exception e)
        {
            _room = null;
        }
        DebugCustom.LogColor(" Da LeaveRoom");
    }

    public void SendEndGame()
    {
        if (_dataEndgame != null)
        {
            PVPManager.Instance.StateMachine.TriggerEndGame();
            OnEndGamePVP?.Invoke(_dataEndgame);
            if (OnEndGamePVP != null)
                _dataEndgame = null;
        }
    }

    public void SendOtherPlayerLeftGame()
    {
        if (_otherPlayerLeft)
        {
            if (OnLeftPVP != null)
            {
                OnLeftPVP.Invoke();
                PVPManager.Instance.StateMachine.TriggerEndGame();
                _otherPlayerLeft = false;
            }
        }
    }

    private void InitHandle()
    {
        _onUpdateScore = OnUpdateScore;
        _onAttackSkill = OnAttackSkill;
        _room.OnMessage<PlayerLeftMessage>("PLAYER_LEFT", (msg) =>
        {
            if (msg.GameState == 0 || msg.GameState == 1)
            {
                _otherPlayerLeft = true;
                SendOtherPlayerLeftGame();
            }
        });

        _room.OnMessage<ReadyPVPMessage>("ENERMY_READY_PVP", (msg) =>
        {
            DebugCustom.LogColorJson("ENERMY_READY_PVP", msg);
            OnReadyPVP?.Invoke(msg);
        });
        _room.OnMessage<PreparePVPMessage>("PREPARE_PVP", (msg) =>
        {
            OnPreparePVP?.Invoke(msg);
            _isHaveReconnect = true;
        });
        _room.OnMessage<AttackSkillMessage>("ATTACK_SKILL", (msg) =>
        {

            _onAttackSkill.Invoke(msg);
            _isHaveReconnect = true;
        });
        _room.OnMessage<StartGamePVPMessage>("GAME_START", (msg) =>
        {
            DebugCustom.LogColorJson("GAME_START revice msg", msg);
            OnStartGamePVP?.Invoke(msg);
        });
        _room.OnMessage<NeedGameStartPVPMessage>("NEED_GAME_START", (msg) =>
        {
            SendStartGame();
        });

        _room.OnMessage<GetScorePVPMessage>("GAME_SCORE_UPDATE", (msg) =>
        {
            _onUpdateScore?.Invoke(msg);
            //DebugCustom.LogColorJson("GAME_SCORE_UPDATE", msg);
        });

        _room.OnMessage<EndGameMessage>("END_GAME", (msg) =>
        {
            DebugCustom.LogColorJson("END_GAME", msg);
            _dataEndgame = msg;
            _isHaveReconnect = false;
            SendEndGame();
        });
    }
    public void SendReadyPVP(ReadyPVPMessage dataPlayer)
    {
        _ = _room.Send("READY_PVP", dataPlayer);
         DebugCustom.LogColor("SEND_READY_PVP");
    }
    private void OnAttackSkill(AttackSkillMessage data)
    {
        OnAttackHide?.Invoke(data.Type);
    }
    private void OnUpdateScore(GetScorePVPMessage data)
    {
        string idOrtherPlayer = PVPManager.Instance.DataOtherPlayer.SessionId;
        DebugCustom.LogColorJson("id other player", idOrtherPlayer);

        if (data.GameScores.ContainsKey(idOrtherPlayer))
        {
            GameScoreData otherPlayerData = data.GameScores[idOrtherPlayer];
            DebugCustom.LogColorJson("data other player", otherPlayerData);

            GameScoreData playerData = data.GameScores[PVPManager.Instance.DataPlayer.SessionId];
            DebugCustom.LogColorJson("data current player", playerData);

            if (otherPlayerData != null)
            {
                OnScoreChange?.Invoke(otherPlayerData.Score, playerData.Score);
                OnHpChange?.Invoke(otherPlayerData.CurHP, otherPlayerData.MaxHP);
            }
        }
        if(data.Time >=0)
        {
            OnChangeTimePlay?.Invoke(data.Time);
        }
    }

    public void RemoveHandle()
    {
        OnReadyPVP = null;
        OnPreparePVP = null;
        OnStartGamePVP = null;
        OnScoreChange = null;
        OnChangeTimePlay = null;
        OnHpChange = null;
        OnEndGamePVP = null;
        OnLeftPVP = null;
        OnEndGamePVPByDisconnect = null;
        OnReconnect = null;
    }

    private void OnLeave(int code)
    {
        _room = null;
         DebugCustom.LogColorJson("OnLeave", _isHaveReconnect, code, _room);
        if (_isHaveReconnect && !_otherPlayerLeft)
            OnReconnect?.Invoke(code);
        else
            PVPManager.Instance.StateMachine.Dispose();
    }
}
