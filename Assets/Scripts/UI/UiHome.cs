using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TigerForge;
using WE.Utils;
using DG.Tweening;
using WE.Unit;
using WE.Game;
using WE.Manager;
using TMPro;
using Dragon.SDK;
using UniRx;
namespace WE.UI
{
    public class UiHome : UIBase
    {
        public UiGlobalUpgradeItem[] upgradeItems;

        public GameObject hackCoin;
        public GameObject hackLevel;
        public GameObject hackSliverKey;
        public GameObject cheatServer;

        [Header("==========Assign Text=======")]
        public TextMeshProUGUI textCoins;
        public TextMeshProUGUI textZone;
        public TextMeshProUGUI textNoadsCountdown;

        public TextMeshProUGUI textDisplayName;
        public Image avatarHome;
        [Header("==========Assign Button=======")]
        public Button SettingButton;
        public Button PlayButton;
        public Button PVPButton;
        public Button SelectCarButton;
        public Button NextZoneButton;
        public Button PreviousZoneButton;
        public Button NoAdsButton;
        public Button AvatarButton;

        public GameObject buttonMoreGame;
        public Image imgNoti;

        Tween tween;

        public override void ActionAfterShow()
        {
            base.ActionAfterShow();
            if (GameplayManager.Instance.CurrentGameplayType == GameType.Tutorial)
            {
                Observable.Timer(System.TimeSpan.FromSeconds(2)).Subscribe(_ => EventManager.EmitEvent(Constant.TUT_ON_UI_INIT_DONE)).AddTo(gameObject);
            }
        }
        public override void InitUI()
        {
            OnChangeZone();
            for (int i = 0; i < upgradeItems.Length; i++)
            {
                upgradeItems[i].InitUI();
            }

            textCoins.text = Player.Instance.currentCoin.ToString();
            EventManager.StartListening(Constant.ON_CHANGE_AVATAR, OnChangeAvatar);
            EventManager.StartListening(Constant.ON_CHANGE_NAME, OnChangeName);
            EventManager.StartListening(Constant.ON_COINS_CHANGE, OnChangeCoin);
            EventManager.StartListening(Constant.ON_CHANGE_ZONE, OnChangeZone);
            EventManager.StartListening(Constant.TIMER_TICK_EVENT, OnTick);
            OnTick();

            SettingButton.onClick.AddListener(OpenSetting);
            PlayButton.onClick.AddListener(StartCamapaign);
            PVPButton.onClick.AddListener(OpenPVPPopup);
            SelectCarButton.onClick.AddListener(OpenSlectCarPopup);
            NextZoneButton.onClick.AddListener(NextZone);
            PreviousZoneButton.onClick.AddListener(PreviousZone);
            AvatarButton.onClick.AddListener(OpenUIAvatar);
            hackCoin.SetActive(Constant.IS_TESTER_JACKAL);
            hackLevel.SetActive(Constant.IS_TESTER_JACKAL);
            hackSliverKey.SetActive(Constant.IS_TESTER_JACKAL);
            cheatServer.SetActive(Constant.IS_TESTER_JACKAL);
            buttonMoreGame.SetActive(FireBaseRemoteConfig.GetBoolConfig("ActiveMoreGames", false));

            MailController.Instance.actionNoti += CheckNotiMail;

            ResolutionManager.Instance.ZoomInUiPanel();
            if (GameplayManager.Instance.CurrentTimePlay > 180 && !Player.Instance.IsOnNoAds() &&  GameplayManager.Instance.CurrentGameplayType != GameType.Tutorial)
            {
                OpenNoAdsPopup();
            }
        }
        public void OnChangeZone()
        {
            int currentZone = Player.Instance.CurrentMap;
            MapController.Instance.LoadMap();
            //textZone.text = currentZone.ToString() + "." + " " + MapController.Instance.currentMapConfig.nameZone.ToUpper();
            textZone.text = "Zone " + currentZone.ToString();
            NextZoneButton.gameObject.SetActive(Player.Instance.CanNextLevel());
            PreviousZoneButton.gameObject.SetActive(Player.Instance.CanPreviousLevel());
        }
        public void OnChangeCoin()
        {
            if (tween != null)
                tween.Kill();
            EventManager.GetInt(Constant.ON_COINS_CHANGE);
            int lastCoin = int.Parse(textCoins.text);
            int currentCoin = Player.Instance.currentCoin;
            tween = DOTween.To(() => lastCoin, x => lastCoin = x, currentCoin, 1).OnUpdate(() => { textCoins.text = lastCoin.ToString(); }).OnComplete(() => { textCoins.text = currentCoin.ToString(); });
            for (int i = 0; i < upgradeItems.Length; i++)
            {
                upgradeItems[i].OnUpgradeChange();
            }
        }

        public void OnChangeName()
        {
            textDisplayName.text = Context.CurrentUserPlayfabProfile.DisplayName;
        }

        public void OnChangeAvatar()
        {
            avatarHome.sprite = SpriteManager.Instance.GetSpriteAvatar(Player.Instance.CurrentAvatar);
        }

        public void CheckNotiMail(bool show)
        {
            imgNoti.gameObject.SetActive(show);
        }

        public override void AfterHideAction()
        {
            base.AfterHideAction();
            EventManager.StopListening(Constant.ON_CHANGE_AVATAR, OnChangeAvatar);
            EventManager.StopListening(Constant.ON_CHANGE_NAME, OnChangeName);
            EventManager.StopListening(Constant.ON_COINS_CHANGE, OnChangeCoin);
            EventManager.StopListening(Constant.ON_CHANGE_ZONE, OnChangeZone);
            EventManager.StopListening(Constant.TIMER_TICK_EVENT, OnTick);


            SettingButton.onClick.RemoveListener(OpenSetting);
            PlayButton.onClick.RemoveListener(StartCamapaign);
            PVPButton.onClick.RemoveListener(OpenPVPPopup);
            SelectCarButton.onClick.RemoveListener(OpenSlectCarPopup);
            NextZoneButton.onClick.RemoveListener(NextZone);
            PreviousZoneButton.onClick.RemoveListener(PreviousZone);
            AvatarButton.onClick.RemoveListener(OpenUIAvatar);

            MailController.Instance.actionNoti -= CheckNotiMail;
        }
        public void StartCamapaign()
        {
            GameplayManager.Instance.StartGame(GameType.Campaign);
        }

        public void OpenSetting()
        {
            UIManager.Instance.OpenHomeSetting();
        }
        public void OpenSlectCarPopup()
        {
            UIManager.Instance.OpenSelectCarPopup();
        }
        public void NextZone()
        {
            Player.Instance.NextLevel();
        }
        public void PreviousZone()
        {
            Player.Instance.PreviousLevel();
        }
        public void HackCoin()
        {
            Player.Instance.AddCoin(1000000);
        }
        public void HackLevel()
        {
            Player.Instance.HackLevel();
        }
        public void HackSliverKey()
        {
            Player.Instance.AddEndlessKey(10);
        }
        public void OpenPopupEndless()
        {
            UIManager.Instance.OpenPopupEndless();
        }
        public void OpenPopupMoreGame()
        {
            UIManager.Instance.OpenPopupMoreGames();
        }
        public void OpenCheatServerPopup()
        {
            UIManager.Instance.OpenCheatServer();
        }

        public void OpenPopupMail()
        {
            MailController.Instance.GetMail(null, null);
            UIManager.Instance.OpenPopupMail();
        }

        public void OpenUIAvatar()
        {
            UIManager.Instance.OpenUIAvatar();
        }

        public void OnTick()
        {
            NoAdsButton.enabled = !Player.Instance.IsOnNoAds();
            textNoadsCountdown.text = Player.Instance.ConvetNoAdsTime();
        }
        public void OpenNoAdsPopup()
        {
            UIManager.Instance.OpenPopupNoAds();
        }

        public void OpenPVPPopup()
        {
            UIManager.Instance.ShowPopupPVP();
        }
    }
}

