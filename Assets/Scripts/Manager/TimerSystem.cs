using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Unit;
using WE.Utils;
using DG.Tweening;
using WE.Pooling;
using Dragon.SDK;
using WE.Support;

namespace WE.Manager
{
    public class TimerSystem : MonoBehaviour
    {
        public static TimerSystem Instance;

        float tcount;
        public bool bannerOn;
        private void Awake()
        {
            Instance = this;
            tcount = 0;
        }
        private void Start()
        {
            Player.Instance.Init();
            GameplayManager.Instance.SetState(GameState.MainUi);

            UIManager.Instance.Init();

            SoundManager.Instance.OnStart();
        }
        private void Update()
        {
            float t = Time.deltaTime;
            tcount += t;
            if (tcount >= 1)
            {
                Tick();
                tcount -= 1;
            }
            TimerUpdate(t);
        }
        private void TimerUpdate(float t)
        {
            TigerForge.EventManager.EmitEventData(Constant.TIMER_UPDATE_EVENT, t);
            ObjectPooler.Instance.OnUpdate(t);
            CheckBanner();
            //Debug.Log("Update " + t);
        }
        private void Tick()
        {
            TigerForge.EventManager.EmitEvent(Constant.TIMER_TICK_EVENT);
        }
        public void StopTimeScale()
        {
            Time.timeScale = 0;
            GameplayManager.Instance.SetState(GameState.Pause);
        }
        public void StopTimeScale(float t, Action callBack)
        {
            int wait = 1;
            DOTween.To(() => wait, x => wait = x, 0, t).SetUpdate(true).OnComplete(()=> { callBack?.Invoke(); StopTimeScale(); });
        }
        public void ReturnTimeScale()
        {
            Time.timeScale = 1;
            if (GameplayManager.Instance.State == GameState.Pause)
            {
                GameplayManager.Instance.SetState(GameState.Gameplay);
            }
            //StartCoroutine(IEReturnTimeScale());
        }
        //IEnumerator IEReturnTimeScale()
        //{

        //    UIManager.Instance.UnScaleTime();
        //    yield return new WaitForSecondsRealtime(Constant.RETURN_UNSCALE_TIME);
        //    Time.timeScale = 1;
        //    if(GameplayManager.Instance.State == GameState.Pause)
        //    {
        //        GameplayManager.Instance.SetState(GameState.Gameplay);
        //    }
        //}
        public void WaitOneFrame(Action callBack)
        {
            StartCoroutine(IEOneFrame(callBack));
        }
        IEnumerator IEOneFrame(Action callBack)
        {
            yield return new WaitForEndOfFrame();
            callBack?.Invoke();

        }
        public void WaitToGame(Action callBack)
        {
            StartCoroutine(IEWaitToGame(callBack));
        }
        IEnumerator IEWaitToGame(Action callBack)
        {
            yield return new WaitUntil(IsNormalGame);
            callBack?.Invoke();
        }
        public bool IsNormalGame()
        {
            return GameplayManager.Instance.State == GameState.Gameplay && Time.timeScale == 1;
        }
        public void HideBanner()
        {
            bannerOn = false;
            AdsManager.Instance.HideBanner();
        }
        public void ShowBanner()
        {
            bannerOn = true;
            AdsManager.Instance.ShowBanner();
        }
        public void CheckBanner()
        {
            if (bannerOn)
            {
                if (Player.Instance.IsOnNoAds())
                {
                    HideBanner();
                }
            }
            else
            {
                if (!Player.Instance.IsOnNoAds())
                {
                    ShowBanner();
                }
            }
        }
    }
}

