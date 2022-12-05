using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.UI;
using WE.Manager;
using WE.Utils;
using WE.Unit;
using UnityEngine.UI;
using WE.Support;
using UniRx;
using TigerForge;

namespace WE.Game
{
    public class TutorialController : MonoBehaviour
    {
        public static TutorialController Instance;
        private void Awake()
        {
            Instance = this;
        }
        public List<TutorialStep> tutorialSteps;
        public Enemy enemyToSpawn;
        public PopupTut popupTut;
        public GameObject popupTaptoPlay;

        int currentId;
        TutorialStep currentStep;
        CompositeDisposable disposables;
        bool firstEnemy = false;
        Vector3 pos;
        public void OnFirstTap()
        {
            popupTaptoPlay.gameObject.SetActive(false);
            UIManager.Instance.StartGame();
            firstEnemy = true;
            popupTut.OnClicked += OnStep;
            disposables = new CompositeDisposable();
            Observable.Timer(System.TimeSpan.FromSeconds(2)).Subscribe(_ =>
            {
                OnStep();
            }).AddTo(disposables);
            Observable.Timer(System.TimeSpan.FromSeconds(2)).Subscribe(_ =>
            {
                Observable.Interval(System.TimeSpan.FromSeconds(0.5f)).Subscribe(_ => { SpawnEnemy(); }).AddTo(disposables);
            }).AddTo(disposables);
            Observable.Interval(System.TimeSpan.FromSeconds(30)).Subscribe(_ => {
                Observable.Interval(System.TimeSpan.FromSeconds(0.3f)).Subscribe(_ => { SpawnEnemy(); }).AddTo(disposables);
            }).AddTo(disposables);
            EventManager.StartListening(Constant.TUT_ON_FIRST_ENEMY_IN_SCENE, OnFirstEnemyInScene);
            EventManager.StartListening(Constant.TUT_ON_QUIT_TUT, OnOutTut);
        }
        public void StartTutorial()
        {
            popupTaptoPlay.gameObject.SetActive(true);
            
        }
        public void OnOutTut()
        {
            TutorialDone();
        }
        public void OnFirstEnemyInScene()
        {
            EventManager.StopListening(Constant.TUT_ON_FIRST_ENEMY_IN_SCENE, OnFirstEnemyInScene);
            SetAnchor(pos);
            ShowTut();
            EventManager.StartListening(Constant.TUT_ON_FIRST_ENEMY_DIE, OnFirstEnemyDie);
        }
        public void OnFirstEnemyDie()
        {
            EventManager.StopListening(Constant.TUT_ON_FIRST_ENEMY_DIE, OnFirstEnemyDie);
            SetAnchor(pos);
            Observable.Timer(System.TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                ShowTut();
            }).AddTo(disposables);
            EventManager.StartListening(Constant.TUT_ON_FIRST_LEVEL_REACH, OnFirstLevelReach);
        }
        public void OnFirstLevelReach()
        {
            EventManager.StopListening(Constant.TUT_ON_FIRST_LEVEL_REACH, OnFirstLevelReach);
            ShowTut();
            EventManager.StartListening(Constant.TUT_ON_SECONDS_LEVEL_REACH, OnSecondLevelReach);
        }
        public void OnSecondLevelReach()
        {
            EventManager.StopListening(Constant.TUT_ON_SECONDS_LEVEL_REACH, OnSecondLevelReach);
            ShowTut();
            EventManager.StartListening(Constant.TUT_ON_THIRD_LEVEL_REACH, OnThirdLevelReach);
            Observable.Interval(System.TimeSpan.FromSeconds(0.3f)).Subscribe(_ => { SpawnEnemy(); }).AddTo(disposables);
        }
        public void OnThirdLevelReach()
        {
            EventManager.StopListening(Constant.TUT_ON_THIRD_LEVEL_REACH, OnThirdLevelReach);
            ShowTut();
            EventManager.StartListening(Constant.TUT_ON_FIRST_SKILL_MAX, OnFirstSkillMax);
            Observable.Interval(System.TimeSpan.FromSeconds(0.3f)).Subscribe(_ => { SpawnEnemy(); }).AddTo(disposables);
        }
        public void OnFirstSkillMax()
        {
            EventManager.StopListening(Constant.TUT_ON_FIRST_SKILL_MAX, OnFirstSkillMax);
            ShowTut();
            disposables.Dispose();
            disposables = new CompositeDisposable();
            SpawnBoss();
            EventManager.StartListening(Constant.TUT_ON_LEVEL_TUT_END, OnLevelTutEnd);
        }
        public void OnLevelTutEnd()
        {
            EventManager.StopListening(Constant.TUT_ON_LEVEL_TUT_END, OnLevelTutEnd);
            Observable.Timer(System.TimeSpan.FromSeconds(2)).Subscribe(_ =>
            {
                Debug.Log("AAAAAAAAAAAAAAAAAAAA");
                ShowTut();
                Observable.Timer(System.TimeSpan.FromSeconds(2)).Subscribe(_ =>
                {
                    GameplayManager.Instance.ShowPopupEndGame(true);
                }).AddTo(disposables);
            }).AddTo(disposables);
            EventManager.StartListening(Constant.TUT_ON_UI_INIT_DONE, OnHomeInitDone);
        }
        public void OnHomeInitDone()
        {
            EventManager.StopListening(Constant.TUT_ON_UI_INIT_DONE, OnHomeInitDone);
            ShowTut();
        }
        public void SetAnchor(Vector3 pos)
        {
            popupTut.childRect.transform.position = new Vector3(pos.x, pos.y, popupTut.childRect.transform.position.z);
            currentStep.anchor = popupTut.childRect.anchoredPosition;
        }
        public void OnStep()
        {
            if (currentId >= tutorialSteps.Count)
            {
                TutorialDone();
            }
            else
            {
                currentStep = tutorialSteps[currentId];
                if (!currentStep.WaitCondition)
                {
                    ShowTut();
                }
                if (currentId == 1)
                {
                    SkillController.Instance.GameInit();
                }
            }

        }
        public void ShowTut()
        {
            popupTut.Show();
            popupTut.InitTut(Helper.GetTranslation(currentStep.I2TextKey),currentStep.showBlackBG, currentStep.overideAnchor,currentStep.targetAnchor, currentStep.anchor, currentStep.interactButton, currentStep.activeHand, currentStep.hideOnClick);
            currentId++;
        }
        public void TutorialDone()
        {
            if (disposables != null)
            {
                disposables.Dispose();
            }
            Player.Instance.SetTutDone();
            popupTut.Hide();
            EventManager.StopListening(Constant.TUT_ON_QUIT_TUT, OnOutTut);
            EventManager.StopListening(Constant.TUT_ON_FIRST_ENEMY_IN_SCENE, OnFirstEnemyInScene);
            EventManager.StopListening(Constant.TUT_ON_FIRST_ENEMY_DIE, OnFirstEnemyDie);
            EventManager.StopListening(Constant.TUT_ON_FIRST_LEVEL_REACH, OnFirstLevelReach);
            EventManager.StopListening(Constant.TUT_ON_SECONDS_LEVEL_REACH, OnSecondLevelReach);
            EventManager.StopListening(Constant.TUT_ON_THIRD_LEVEL_REACH, OnThirdLevelReach);
            EventManager.StopListening(Constant.TUT_ON_FIRST_SKILL_MAX, OnFirstSkillMax);
            EventManager.StopListening(Constant.TUT_ON_LEVEL_TUT_END, OnLevelTutEnd);
            EventManager.StopListening(Constant.TUT_ON_UI_INIT_DONE, OnHomeInitDone);
        }
        public void SpawnEnemy()
        {
            if (firstEnemy)
            {
                pos = Helper.GetRandomPosInScreen();
                Helper.SpawnEnemy(enemyToSpawn, pos);
                EventManager.EmitEventData(Constant.TUT_ON_FIRST_ENEMY_IN_SCENE, pos);
                firstEnemy = false;
            }
            Helper.SpawnEnemy(enemyToSpawn);
        }
        public void SpawnBoss()
        {
            Enemy e =Helper.SpawnEmptyEnemy(enemyToSpawn);
            e.Init(true, 100, 1, 0, 10, 2);
        }
    }
    [System.Serializable]
    public class TutorialStep
    {
        public bool showBlackBG;
        public bool WaitCondition;
        public bool overideAnchor = true;
        public Vector2 anchor;
        public Transform targetAnchor;
        public Button interactButton;
        public string I2TextKey;
        public bool activeHand;
        public bool hideOnClick;
    }
}

