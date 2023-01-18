using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using WE.UI;
using WE.UI.PVP;
using UnityEngine.UI;
using DG.Tweening;
using WE.Utils;
using WE.Support;
using WE.Unit;
using TMPro;
using static MailController;

namespace WE.Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        [FoldoutGroup("Assign UI")]
        public UiHome uiHome;
        [FoldoutGroup("Assign UI")]
        public UIHomeSetting uIHomeSetting;
        [FoldoutGroup("Assign UI")]
        public UISelectVehicle uISelectVehicle;
        [FoldoutGroup("Assign UI")]
        public UIPopupEndless popupEndless;
        [FoldoutGroup("Assign UI")]
        public UIInGame uIInGame;
        [FoldoutGroup("Assign UI")]
        public UIPauseInGame uIPauseInGame;
        [FoldoutGroup("Assign UI")]
        public UIPopupWin uIPopupWin;
        [FoldoutGroup("Assign UI")]
        public UIPopupLose uIPopupLose;
        [FoldoutGroup("Assign UI")]
        public UISelectSkill uISelectSkill;
        [FoldoutGroup("Assign UI")]
        public UIPopupOpenChest uIPopupOpenChest;
        [FoldoutGroup("Assign UI")]
        public UIDie uIDie;
        [FoldoutGroup("Assign UI")]
        public Image flashImage;
        [FoldoutGroup("Assign UI")]
        public UIPopupMoreGames popupMoreGames;
        [FoldoutGroup("Assign UI")]
        public PopUpNoAds popUpNoAds;
        [FoldoutGroup("Assign UI")]
        public UIMailPopup uIMailPopup;
        [FoldoutGroup("Assign UI")]
        public UIMailDetail uIMailDetailPopup;

        [FoldoutGroup("Assign UI")]
        public UIAvatar uIAvatar;
        [FoldoutGroup("Assign UI")]
        public UIChangeName uIChangeName;

        [FoldoutGroup("Assign UI")]
        public UIInGamePVP uiInGamePVP;
        [FoldoutGroup("Assign UI")]
        public UIPopupPVP uIPopupPVP;

        [FoldoutGroup("Assign Text")] 
        public UITextPopup uITextPopup;


        public bool tester;
        public bool playTut;
        public UIBase currentGameUI;
        UIBase pendingUI;

        private void InitServer()
        {
            RocketIO.Instance.Init();
        }

        public void StartGame()
        {
            uiHome.Hide();
            uIInGame.Show();
        }

        public void StartGamePVP()
        {
            uiHome.Hide();
            uiInGamePVP.Show();
        }

        public void ReturnHome()
        {
            if (currentGameUI != null)
            {
                currentGameUI.Hide();
            }
            uIPauseInGame.Hide();

            uIInGame.Hide();
            uiHome.Show();
        }
        public void PendingUI(UIBase ui)
        {
            pendingUI = ui;
        }
        private void Update()
        {
            if (pendingUI != null && TimerSystem.Instance.IsNormalGame())
            {
                if (currentGameUI == null)
                {
                    var cachedUI = pendingUI;
                    pendingUI = null;
                    TimerSystem.Instance.WaitOneFrame(() =>
                    {
                        cachedUI.Show();
                    });
                }
            }

            if (Input.GetKey(KeyCode.Escape))
            {
                if (GameplayManager.Instance.State == GameState.Gameplay && currentGameUI == null && GameplayManager.Instance.CurrentGameplayType != GameType.Tutorial)
                {
                    PauseGame();
                }
                if (currentGameUI != null)
                {
                    if (currentGameUI.IsActiveBackButton && currentGameUI.CanBack)
                    {
                        currentGameUI.Hide();
                    }
                }
            }

#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.K))
            //{
            //    Player.Instance.TakeDamage(Player.Instance.CurrentHp + 1, null);
            //}
            if (Input.GetKeyDown(KeyCode.E) && GameplayManager.Instance.State == GameState.Gameplay)
            {
                GameplayManager.Instance.AddExp(10);
                //GameplayManager.Instance.DropExp(Helper.GetRandomPosInScreen(), 10);
            }
            if (Input.GetKeyDown(KeyCode.U) && GameplayManager.Instance.State == GameState.Gameplay)
            {
                GameplayManager.Instance.DropItem(Helper.GetRandomPosInScreen(), EItemInGame.Boss_Chest);
            }
            //if (Input.GetKeyDown(KeyCode.J))
            //{
            //    ResolutionManager.Instance.ZoomOutHighPannel();
            //}
            if (Input.GetKeyDown(KeyCode.L) && GameplayManager.Instance.State == GameState.Gameplay)
            {
                uIInGame.LockExp();
            }
            //if (Input.GetKeyDown(KeyCode.W))
            //{
            //    Warning(5);
            //}
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (Player.Instance.IsImortal)
                {
                    Player.Instance.StopImortal();
                }
                else
                {
                    Player.Instance.SetImortal();
                }
            }
            //if (Input.GetKeyDown(KeyCode.W))
            //{
            //    ShowPopupWin();
            //}
            if (Input.GetKeyDown(KeyCode.F))
            {
                Time.timeScale += 1;
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                float scale = Time.timeScale;
                scale--;
                if (scale < 0) scale = 0;
                Time.timeScale = scale;
            }
            //if (Input.GetKeyDown(KeyCode.B))
            //{
            //    GameplayManager.Instance.DropItem(Helper.GetRandomPosInScreen(), EItemInGame.Bomb);
            //}
            if (Input.GetKeyDown(KeyCode.S))
            {
                GameplayManager.Instance.ReviceItem(EItemInGame.Magnetic);
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                SpawnExp();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                Helper.SpawnChestIngame();
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                GameplayManager.Instance.ReviceItem(EItemInGame.Heal);
            }
#endif
        }
        public void SpawnExp()
        {
            for (int i = 0; i < 1000; i++)
            {
                Helper.SpawnExp(Helper.GetRandomSpawnPos(), 1);
            }
        }
        public void Warning(float t)
        {
            uIInGame.Warning(t);
        }
        public void Init()
        {
            flashImage.color = new Color(1, 1, 1, 0);
            if (!Player.Instance.IsPlayTut() && playTut)
            {
                GameplayManager.Instance.StartGame(GameType.Tutorial);
            }
            else
            {
                InitServer();
                uiHome.Show();
            }
        }
        public void ShowPopupDie()
        {
            uIDie.Show();
        }
        public void OpenHomeSetting()
        {
            uIHomeSetting.Show();
        }
        public void OpenSelectCarPopup()
        {
            uISelectVehicle.Show();
        }
        public void PauseGame()
        {
            uIPauseInGame.Show();
        }
        public void ShowUIIngame()
        {
            uIInGame.Show();
        }
        public void OpenPopupEndless()
        {
            popupEndless.Show();
        }
        public void ShowPopupLose()
        {

            //if (currentGameUI != null)
            //{
            //    currentGameUI.Hide();
            //    currentGameUI = null;
            //}
            //currentGameUI = null;
            uIPopupLose.Show();
        }
        public void ShowPopupWin()
        {
            //if (currentGameUI != null)
            //{
            //    currentGameUI.Hide();
            //    currentGameUI = null;
            //}
            //currentGameUI = null;
            uIPopupWin.Show();
        }
        public void ShowPopupSelectSkill()
        {
            uISelectSkill.Show();
        }
        public void ShowPopupOpenChestBoss()
        {
            uIPopupOpenChest.Show();
        }
        public void UnScaleTime()
        {
            //flashImage.color = new Color(1, 1, 1, 0.5f);
            //flashImage.DOFade(0, Constant.RETURN_UNSCALE_TIME).SetUpdate(true);
        }
        public void FlashScreen()
        {
            StartCoroutine(IEFlash());
        }
        IEnumerator IEFlash()
        {
            flashImage.DOFade(1, 0.1f);
            yield return new WaitForSecondsRealtime(0.1f);
            flashImage.DOFade(0, 0.3f);

        }
        public void ShowTextNotEnoughCoin()
        {
            uITextPopup.Show("Not Enought Coin");  
        }
        public void ShowTextNoAds()
        {
            uITextPopup.Show("No Ads Avaiable");
        }
        public void ShowTextNoInternet()
        {
            uITextPopup.Show("No Internet Connection");
        }
        public void ShowTextAds(int id)
        {
            if(id == 0)
            {
                ShowTextNoInternet();
            }else if(id == 1)
            {
                ShowTextNoAds();
            }
        }
        public void OpenPopupMoreGames()
        {
            popupMoreGames.Show();
        }
        public void OpenPopupNoAds()
        {
            popUpNoAds.Show();
        }

        public void OpenPopupMail()
        {
            uIMailPopup.InitMail();
            uIMailPopup.Show();
        }

        public void OpenPopupMailDetail(string idMail, int type, DataMailDetail data)
        {
            uIMailDetailPopup.LoadInfo(idMail, type, data);
            uIMailDetailPopup.Show();
        }

        public void OpenUIAvatar()
        {
            uIAvatar.Show();
        }
        public void ShowPopupRename()
        {
            Debug.Log("Show popup rename");
            uIChangeName.Show();
        }

        public void ShowPopupPVP()
        {
            uIPopupPVP.Show();
        }
    }
}

