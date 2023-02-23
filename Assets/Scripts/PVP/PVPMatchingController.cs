using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using WE.Manager;
using WE.PVP;
using WE.PVP.Manager;
using WE.UI;
using WE.UI.PVP;
using WE.Unit;

public class PVPMatchingController : MonoBehaviour
{
    [SerializeField] private UIMatchingPVP _prefabUiMatchingPVP;
    private UIMatchingPVP _uiMatchingPVP;
    public void Matching()
    {
        DebugCustom.LogColor("Matching");
        RocketIO.Instance.LoginSequence(true).Subscribe(
            success =>
            {

                if (PVPManager.Instance.Room != null)
                {
                    if (PVPManager.Instance.Room.Room != null)
                    {
                        PVPManager.Instance.Room.LeaveRoom(false, false);
                        return;
                    }
                }
                PVPManager.Instance.InitStateMachine();
                PVPManager.Instance.StateMachine.OnInitRoom = InitRoom;
                PVPManager.Instance.StateMachine.ActiveFSM();
            },
            error =>
            {
                UIManager.Instance.ShowTextNoInternet();
            });
    }
    private async void JoinRoom(int type, string roomName)
    {
        ReadyPVPMessage dataJoin = CreateDataJoin();
        bool isInitRoom = false;
        isInitRoom = await PVPManager.Instance.JoinRoom(type, roomName, dataJoin, OnErrorInitRoom);
        if (isInitRoom)
        { 
            /**
             * Kh?i t?o UI Matching PVP
             */
            Debug.Log("Init room PVP");
            //    _uiMatchingPVP = Instantiate(_prefabUiMatchingPVP);
            //   _uiMatchingPVP.gameObject.SetActive(true);
            _uiMatchingPVP = UIManager.Instance.GetUIMatchingPVP();
            _uiMatchingPVP.Show();
            _uiMatchingPVP.FillDataPlayer(dataJoin);
            PVPManager.Instance.Room.SendReadyPVP(dataJoin);
           

           // PVPManager.Instance.Room.SendStartGame();
            //PVPManager.Instance.gameObject.AddComponent<PVPMode>();
        }
    }
    private ReadyPVPMessage CreateDataJoin()
    {
        ReadyPVPMessage dataJoin = new ReadyPVPMessage();
        dataJoin.DisplayName = Context.CurrentUserPlayfabProfile.DisplayName;
        dataJoin.UserId = Context.CurrentUserPlayfabProfile.UserId;
        dataJoin.Status = 1;
        dataJoin.CountMatching = PVPManager.Instance.CountMatching;
        dataJoin.Elo = Context.PvPBattlePoint;
        dataJoin.Atk = Player.Instance.AttackDamage;
        dataJoin.TankId = Player.Instance.CurrentTank;
        dataJoin.AvatarUrl = Player.Instance.CurrentAvatar;
        PVPManager.Instance.DataPlayer = dataJoin;

        DebugCustom.LogColorJson("dataJoin", dataJoin);
        return dataJoin;
    }

    private void OnErrorInitRoom(ErrorMatching errorMatching)
    {
        DebugCustom.LogColorJson("Error matching pvp", errorMatching);
    }

    private void InitRoom()
    {
        UIManager.Instance.ShowWaitingCanvas();
        PVPManager.Instance.MatchMaking(
            success =>
            {
                UIManager.Instance.HideWaitingCanvas();
                Debug.Log("Success match making");
                PVPManager.Instance.InitClient();
                InitHandle();
                JoinRoom(success.type, success.roomName);
            },
            error =>
            {
                UIManager.Instance.HideWaitingCanvas();
                Debug.Log("Error match making");
            });
    }

    private void InitHandle()
    {
        PVPManager.Instance.Room.OnReadyPVP = OnReadyPVP;
        PVPManager.Instance.Room.OnPreparePVP = OnPreparePVP;
    }

    private void OnReadyPVP(ReadyPVPMessage data)
    {
        DebugCustom.LogColor("OnReadyPVP UI");
        DebugCustom.LogColorJson(data);
        PVPManager.Instance.DataOtherPlayer = data;
        if (_uiMatchingPVP != null)
            _uiMatchingPVP.FillDataOtherPlayer(data);
        PVPManager.Instance.StateMachine.TriggerPrepare();
    }

    private void OnPreparePVP(PreparePVPMessage data)
    {
        DebugCustom.LogColor("OnPreparePVP UI");
        DebugCustom.LogColorJson(data);
        PVPManager.Instance.DataMap = data;
        Context.PVPCurrentMap = data.Level;
        Context.PvPTimeBattle = data.Time;
        MapController.Instance.OnChangeZone(Context.PVPCurrentMap);
        PVPManager.Instance.CountMatching = 0;
        StartCoroutine(PlayBattle());
    }

    private IEnumerator PlayBattle()
    {
        yield return new WaitForSecondsRealtime(5);
        _uiMatchingPVP.Hide();
        _uiMatchingPVP = null;
        PVPManager.Instance.Room.SendStartGame();
        PVPManager.Instance.gameObject.AddComponent<PVPMode>();
    }
    private void RemoveHandle()
    {
        PVPManager.Instance.Room.OnReadyPVP = null;
        PVPManager.Instance.Room.OnPreparePVP = null;
    }
}
