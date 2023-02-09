using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Stateless;
using System;
using WE.PVP.Manager;
using WE.Manager;

public enum PVPSate
{
    INIT_ROOM = 1,
    PREPARE = 2,
    START_GAME = 3,
    PLAYING = 4,
    RECONNECT = 5,
    ENDGAME = 6,
    RECONNECT_STARTGAME = 7,
}
public enum PVPTrigger
{
    Inited = 1,
    Ready = 2,
    TimeOutReconnect = 3,
    Reconnect = 4,
    Play = 5,
    EndGame = 6,
}
public class PVPStateMachine : MonoBehaviour
{
    private StateMachine<PVPSate, PVPTrigger> _fsm;
    private IDisposable _waitTimeout, _waitTimeout_StartGame, _reconnect_Disposable, _pingStartGame;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    public Action OnInitRoom;
    public Action OnPrepare;
    public Action OnStartGame;
    public Action OnPlayGame;
    public Action OnReconnect;
    private int numCheckNetWorkStartGame = 0;
    public PVPSate CurrentState()
    {
        if (_fsm != null)
            return _fsm.State;
        return PVPSate.INIT_ROOM;
    }

    public void InitStateMachine()
    {
        DebugCustom.LogColor("InitStateMachine");
        numCheckNetWorkStartGame = 0;
        _fsm = new StateMachine<PVPSate, PVPTrigger>(PVPSate.INIT_ROOM);
        _fsm.Configure(PVPSate.INIT_ROOM)
            .OnActivate(InitRoomPVP)
            .Permit(PVPTrigger.Inited, PVPSate.PREPARE);

        _fsm.Configure(PVPSate.PREPARE)
            .OnEntry(PreparePVP)
            .Permit(PVPTrigger.Ready, PVPSate.START_GAME);

        _fsm.Configure(PVPSate.START_GAME)
            .OnEntry(StartGamePVP)
            .Permit(PVPTrigger.Play, PVPSate.PLAYING)
            .Permit(PVPTrigger.EndGame, PVPSate.ENDGAME)
            .Permit(PVPTrigger.Reconnect, PVPSate.RECONNECT_STARTGAME);

        _fsm.Configure(PVPSate.PLAYING)
           .OnEntry(PlayGamePVP)
           .Permit(PVPTrigger.Reconnect, PVPSate.RECONNECT)
           .Permit(PVPTrigger.EndGame, PVPSate.ENDGAME);

        _fsm.Configure(PVPSate.RECONNECT)
          .OnEntry(ReconnectPVP)
          .Permit(PVPTrigger.Play, PVPSate.PLAYING)
          .Permit(PVPTrigger.TimeOutReconnect, PVPSate.ENDGAME)
        .Permit(PVPTrigger.EndGame, PVPSate.ENDGAME);

        _fsm.Configure(PVPSate.RECONNECT_STARTGAME)
         .OnEntry(ReconnectStartGame)
         .Permit(PVPTrigger.Play, PVPSate.PLAYING)
         .Permit(PVPTrigger.Ready, PVPSate.START_GAME)
         .Permit(PVPTrigger.TimeOutReconnect, PVPSate.ENDGAME)
         .Permit(PVPTrigger.EndGame, PVPSate.ENDGAME);

        _fsm.Configure(PVPSate.ENDGAME)
          .OnEntry(OnEndGame);

        _fsm.OnTransitioned(transition =>
        {
            //DebugCustom.LogColor("PVPStateMachine", $"{transition.Source} -- {transition.Trigger} --> {transition.Destination}");
        });
    }

    private void PlayGamePVP()
    {
        OnPlayGame?.Invoke();
        _waitTimeout?.Dispose();
        _waitTimeout_StartGame?.Dispose();
        _pingStartGame?.Dispose();
        if (_reconnect_Disposable != null)
        {
            _reconnect_Disposable?.Dispose();
        }
    }

    private void ReconnectPVP()
    {
        DebugCustom.LogColor("ReconnectPVP");
        OnReconnect?.Invoke();
        _waitTimeout?.Dispose();
        _waitTimeout = Observable.Timer(TimeSpan.FromSeconds(PVPManager.Instance.Config.Time_Out_Reconnect))
                                     .Subscribe(_ =>
                                     {
                                         _reconnect_Disposable?.Dispose();
                                         TriggerEndGameByTimeOut();
                                         PVPManager.Instance.Room.LeaveRoom(false);
                                         PVPManager.Instance.Room.OnEndGamePVPByDisconnect?.Invoke();
                                     })
                                     .AddTo(_disposables);
        ProccessReconnect(PVPSate.RECONNECT, () => { TriggerPlaying(); });
    }

    public void ActiveFSM()
    {
        _fsm.Activate();
    }
    public void DeactiveFSM()
    {
        _fsm.Deactivate();
        Dispose();
    }

    private void InitRoomPVP()
    {
        DebugCustom.LogColor("InitRoomPVP");
        OnInitRoom?.Invoke();
    }
    private void PreparePVP()
    {
        OnPrepare?.Invoke();
    }

    private void StartGamePVP()
    {
        OnStartGame?.Invoke();
        // DebugCustom.LogColor("StartGamePVP", PVPManager.Instance.IsInRoom);
        PVPManager.Instance.Room.OnLeftPVP = () =>
        {
            /**
             * Open home UI
             */
            UIManager.Instance.ReturnHome();
        };
        if (PVPManager.Instance.Room.DataEndGame != null)
        {
            PVPManager.Instance.Room.SendEndGame();
            return;
        }
        if (PVPManager.Instance.Room.OtherPlayerLeft)
        {
            PVPManager.Instance.Room.SendOtherPlayerLeftGame();
            return;
        }
        _waitTimeout_StartGame?.Dispose();
        _waitTimeout_StartGame = Observable.Timer(TimeSpan.FromSeconds(PVPManager.Instance.Config.Time_Out_Start_Game))
                                    .Subscribe(_ =>
                                    {
                                        _reconnect_Disposable?.Dispose();
                                        DebugCustom.LogError("Doi thu khong vao game");
                                        TriggerEndGame();
                                        PVPManager.Instance.Room.LeaveRoom(false);
                                        PVPManager.Instance.Room.OnEndGamePVPByDisconnect?.Invoke();
                                    })
                                    .AddTo(_disposables);
        _pingStartGame?.Dispose();
        _pingStartGame = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            if (!PVPManager.Instance.IsInRoom)
            {
                numCheckNetWorkStartGame++;
                if (numCheckNetWorkStartGame > 1)
                {
                    TriggerReconnect();
                    _pingStartGame?.Dispose();
                }
            }
        }).AddTo(_disposables);

    }
    private void ReconnectStartGame()
    {
        DebugCustom.LogColor("ReconnectPVP");
        _waitTimeout?.Dispose();
        ProccessReconnect(PVPSate.RECONNECT_STARTGAME, () => { TriggerStartGame(); });
    }

    private void OnEndGame()
    {
        DeactiveFSM();
    }

    private void ProccessReconnect(PVPSate pvpSate, Action actionSucess = null)
    {
        DebugCustom.LogColor("Reconnect");
        _reconnect_Disposable?.Dispose();
        _reconnect_Disposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            DebugCustom.LogColor("Reconnecting");
            Context.LoginServer(() =>
            {
                PVPManager.Instance.Room.Reconnect(
                   () =>
                   {
                       actionSucess?.Invoke();
                       _waitTimeout?.Dispose();
                       _reconnect_Disposable?.Dispose();
                       _waitTimeout_StartGame?.Dispose();
                       _pingStartGame?.Dispose();
                   },
                   () =>
                   {});
            }, null, false);
        });
    }

    public void TriggerPrepare()
    {
        _fsm.Fire(PVPTrigger.Inited);
    }
    public void TriggerStartGame()
    {
        if (_fsm.CanFire(PVPTrigger.Ready))
            _fsm.Fire(PVPTrigger.Ready);
    }
    public void TriggerPlaying()
    {
        if (_fsm.CanFire(PVPTrigger.Play))
            _fsm.Fire(PVPTrigger.Play);
        DebugCustom.LogColor("TriggerPlaying");
    }
    public void TriggerReconnect()
    {
        if (_fsm.CanFire(PVPTrigger.Reconnect))
            _fsm.Fire(PVPTrigger.Reconnect);
        DebugCustom.LogColor("TriggerReconnect");
    }
    public void TriggerEndGameByTimeOut()
    {
        if (_fsm.CanFire(PVPTrigger.TimeOutReconnect))
            _fsm.Fire(PVPTrigger.TimeOutReconnect);
    }
    public void TriggerEndGame()
    {
        if (_fsm.CanFire(PVPTrigger.EndGame))
            _fsm.Fire(PVPTrigger.EndGame);
    }
    public void Dispose()
    {
        _waitTimeout?.Dispose();
        _disposables?.Dispose();
        _waitTimeout_StartGame?.Dispose();
        _pingStartGame?.Dispose();
    }
}
