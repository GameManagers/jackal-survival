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

namespace WE.UI.PVP
{
    public class UIInGamePVP : UIBase
    {
        public TextMeshProUGUI textCount;
        public TextMeshProUGUI textKill;
        public TextMeshProUGUI textLevel;
        public Transform expBar;
        public Image expBarFlash;
        public CurrentSkillBar skillBar;

        public SkeletonGraphic skeletonGraphic;

        [SerializeField, SpineAnimation(dataField = "skelGraphic")] string animIdle;
        [SerializeField, SpineAnimation(dataField = "skelGraphic")] string animShow;
        [SerializeField, SpineAnimation(dataField = "skelGraphic")] string animIdleShow;
        [SerializeField, SpineAnimation(dataField = "skelGraphic")] string animHide;

        float currentExp;
        float nextExpLevel;
        public override void InitUI()
        {
            EventManager.StartListening(Constant.GAME_TICK_EVENT, OnTick);

        }

        private void OnDisable()
        {
            EventManager.StopListening(Constant.GAME_TICK_EVENT, OnTick);
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
            textCount.text = Helper.ConvertTimer(GameplayManager.Instance.CurrentTimePlay);
        }
        public void PauseGame()
        {
            UIManager.Instance.PauseGame();
        }
    }
}


