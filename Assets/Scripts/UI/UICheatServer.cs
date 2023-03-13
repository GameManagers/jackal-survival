using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WE.Utils;

namespace WE.UI
{
    public class UICheatServer : UIBase
    {


        [SerializeField] private InputField _ipServer;

        public override void InitUI()
        {
            if (!RocketIO.Instance.IsPublicServer) _ipServer.text = PlayerPrefs.GetString(Constant.IPV4ServerTest);
        }

        public void OnClickChangeServer()
        {
            var ipServer = _ipServer.text;

            RocketIO.Instance.GetIpServerTest = "http://" + ipServer + ":8000/socket.io/";
            RocketIO.Instance.GetPVPRoomUrl = "ws://" + ipServer + ":3978";
            RocketIO.Instance.GetPVPMatchMarker = "http://" + ipServer + ":8000/api/match";
            PlayerPrefs.SetInt(Constant.ServerPublic, 0);
            PlayerPrefs.SetString(Constant.IPV4ServerTest, ipServer);
            ExitGame();
        }

        public void OnChangeServerPublic()
        {
            if (!RocketIO.Instance.IsPublicServer)
            {
                PlayerPrefs.SetInt(Constant.ServerPublic, 1);
                ExitGame();
            }
        }

        private void ExitGame()
        {
            Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        #endif
        }
   
    } 
}
