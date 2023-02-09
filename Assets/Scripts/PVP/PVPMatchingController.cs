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
            PVPManager.Instance.Room.SendReadyPVP(dataJoin);
            /**
             * Kh?i t?o UI Matching PVP
             */

            PVPManager.Instance.Room.SendStartGame();
            PVPManager.Instance.gameObject.AddComponent<PVPMode>();
            Debug.Log("Init room PVP");
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
        PVPManager.Instance.MatchMaking(
            success =>
            {
                Debug.Log("Success match making");
                PVPManager.Instance.InitClient();
                InitHandle();
                JoinRoom(success.type, success.roomName);
            },
            error =>
            {
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
        yield return new WaitForSecondsRealtime(2);

    }
    private void RemoveHandle()
    {
        PVPManager.Instance.Room.OnReadyPVP = null;
        PVPManager.Instance.Room.OnPreparePVP = null;
    }
}
