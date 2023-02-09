using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WE.Manager;
using UnityEngine.UI;
using Spine.Unity;
using TigerForge;
using WE.Utils;
using WE.Support;
using DG.Tweening;
using WE.PVP;

namespace WE.UI.PVP
{
    public class UIInGamePVP : UIBase
    {
        [SerializeField]
        private IconPVPInfo playerInfo, oppnentsInfo;
        [SerializeField]
        private ScoreBarPVP scoreBarPVP;
        [SerializeField]
        private GameObject textReady;
        [SerializeField]
        private TextMeshProUGUI textCount;
        [SerializeField]
        private TextMeshProUGUI textLevel;
        [SerializeField]
        private Transform expBar;
        [SerializeField]
        private Image expBarFlash;

        public SkeletonGraphic skeletonGraphic;

        [SerializeField, SpineAnimation(dataField = "skelGraphic")] string animIdle;
        [SerializeField, SpineAnimation(dataField = "skelGraphic")] string animShow;
        [SerializeField, SpineAnimation(dataField = "skelGraphic")] string animIdleShow;
        [SerializeField, SpineAnimation(dataField = "skelGraphic")] string animHide;

        float currentExp;
        float nextExpLevel;
        public override void InitUI()
        {
            playerInfo.InitInfo();

            EventManager.StartListening(Constant.GAME_TICK_EVENT, OnTick);
            EventManager.StartListening(Constant.ON_RECEVICE_EXP, OnReceiveExp);
            EventManager.StartListening(Constant.ON_ENEMY_DIE, OnEnemyKill);

            expBar.localScale = new Vector3(0, 1, 1);
            textLevel.text = "LV.1";
            textCount.text = "00:00";
            currentExp = 0;
        }

        private void OnDisable()
        {
            EventManager.StopListening(Constant.GAME_TICK_EVENT, OnTick);
            EventManager.StopListening(Constant.ON_ENEMY_DIE, OnEnemyKill);
            EventManager.StopListening(Constant.ON_RECEVICE_EXP, OnReceiveExp);
        }

        public void OnEnemyKill()
        {
            UpdateScore(GameplayManager.Instance.CurrentKillCount, 99);
        }

        public void Warning(float timer)
        {
            StartCoroutine(IEWarning(timer));
        }

        IEnumerator IEWarning(float t)
        {
            SoundManager.Instance.PlaySoundFx(SoundManager.Instance.warningSfx);
            var track = skeletonGraphic.AnimationState;
            track.ClearTracks();
            track.AddAnimation(2, animShow, false, 0);
            track.AddAnimation(3, animIdleShow, true, 0);
            track.Apply(skeletonGraphic.Skeleton);
            skeletonGraphic.gameObject.SetActive(true);
            yield return new WaitForSeconds(t);
            track.ClearTracks();
            track.AddAnimation(0, animHide, false, 0);
            track.AddAnimation(1, animIdle, true, 0);
            track.Apply(skeletonGraphic.Skeleton);
            yield return new WaitForSeconds(1);
            skeletonGraphic.gameObject.SetActive(false);
        }

        public void OnTick()
        {
            textCount.text = Helper.ConvertTimer(Context.PvPTimeBattle - GameplayManager.Instance.CurrentTimePlay);
        }
  
        public void PauseGame()
        {
            UIManager.Instance.PauseGame();
        }

        public void DisableTextReady(bool isActive)
        {
            textReady.SetActive(isActive);
        }

        bool cachedFrame = false;
        float cachedExp;
        public void OnReceiveExp()
        {
            float exp = EventManager.GetFloat(Constant.ON_RECEVICE_EXP);
            AddExp(exp);
        }

        public void UpdateScore(int currentScore, int opponetsScore)
        {
            DebugCustom.LogColor("Score", currentScore, opponetsScore);
            scoreBarPVP.UpdateScore(currentScore, opponetsScore);
        }

        private void Update()
        {
            if (cachedFrame)
            {
                if (GameplayManager.Instance.State == GameState.Gameplay)
                {
                    cachedFrame = false;
                    float val = nextExpLevel - currentExp;
                    if (cachedExp > val)
                    {
                        AddExp(val);
                        cachedExp -= val;
                    }
                    else
                    {
                        AddExp(cachedExp);
                        cachedExp = 0;
                    }
                }
            }

        }

        public void AddExp(float exp)
        {
            if (!cachedFrame)
            {
                currentExp += exp;
                expBar.transform.DOKill();
                expBarFlash.DOKill();
                if (currentExp >= nextExpLevel)
                {
                    cachedFrame = true;
                    GameplayManager.Instance.LevelUp();
                    textLevel.text = "LV." + GameplayManager.Instance.CurrentLevel.ToString();
                    currentExp -= nextExpLevel;
                    nextExpLevel = DataManager.Instance.dataGlobalUpgrade.GetNextLevelExp(nextExpLevel, GameplayManager.Instance.CurrentLevel);
                    expBar.transform.localScale = new Vector3(0, 1, 1);
                }
                expBar.transform.DOScale(new Vector3((currentExp / nextExpLevel), 1, 1), 0.2f);
                expBarFlash.color = new Color(1, 1, 1, 0.6f);
                expBarFlash.DOFade(0, 0.2f);
            }
            else
            {
                cachedExp += exp;
            }
        }

        public void UpdateHpBarCurrentPlayer(float currentHp, float maxHp)
        {
            playerInfo.UpdateValueHP(currentHp, maxHp);
        }
        public void Opponents_UpdateHpBar(float _currentHp, float _hpBar)
        {
            oppnentsInfo.UpdateValueHP(_currentHp, _hpBar);
        }

    }
}


