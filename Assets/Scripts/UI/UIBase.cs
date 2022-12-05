using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WE.Manager;
using WE.Utils;

namespace WE.UI
{
    public abstract class UIBase : MonoBehaviour
    {
        public Button CloseButton;
        public Animator UiAnim;
        public GameObject blockPanel;
        public GameObject hackObj;
        public static string openAnim = "Open";
        public static string normalAnim = "Normal";
        public static string closeAnim = "Close";

        protected bool IsInited = false;
        public bool IsControllTimeScale = false;
        public bool IsActiveBackButton = false;
        public bool IsGameplayUI = false;
        public virtual bool CanBack
        {
            get => !isClosing;
        }
        protected bool isClosing;
        public virtual void Show()
        {
            if (IsGameplayUI)
            {
                if (UIManager.Instance.currentGameUI != null)
                {
                    UIManager.Instance.PendingUI(this);
                    return;
                }
                else
                {
                    UIManager.Instance.currentGameUI = this;
                }

                //UIManager.Instance.currentGameUI = this;
            }
            if (IsControllTimeScale)
            {
                TimerSystem.Instance.StopTimeScale();
                GameplayManager.Instance.SetState(GameState.Pause);
            }
            if (CloseButton != null && !IsInited)
                CloseButton.onClick.AddListener(Hide);
            if(hackObj != null)
            hackObj.SetActive(Constant.IS_TESTER_JACKAL);
            isClosing = true;
            gameObject.SetActive(true);
            blockPanel.SetActive(true);
            InitUI();
            IsInited = true;
            StartCoroutine(IEShow());
        }
        public virtual void Hide()
        {
            if(!gameObject.activeInHierarchy)
                return;
            if (CloseButton != null)
                CloseButton.onClick.RemoveListener(Hide);
            isClosing = true;
            IsInited = false;
            StartCoroutine(IEHide());
        }
        public virtual IEnumerator IEShow()
        {
            if (UiAnim != null)
            {
                UiAnim.Play(openAnim);
                yield return new WaitForSecondsRealtime(0.2f);
                //UiAnim.Play(normalAnim);
            }
            blockPanel.SetActive(false);
            isClosing = false;
            ActionAfterShow();
        }
        public virtual IEnumerator IEHide()
        {
            blockPanel.SetActive(true);
            if (UiAnim != null)
            {
                UiAnim.Play(closeAnim);
                yield return new WaitForSecondsRealtime(0.25f);
            }
            if (UIManager.Instance.currentGameUI != null)
            {
                if (UIManager.Instance.currentGameUI == this)
                {
                    UIManager.Instance.currentGameUI = null;
                }
            }
            if (IsControllTimeScale)
            {
                TimerSystem.Instance.ReturnTimeScale();
            }
            AfterHideAction();
            gameObject.SetActive(false);
        }
        public virtual void AfterHideAction()
        {

        }
        public virtual void ActionAfterShow()
        {

        }
        public abstract void InitUI();
    }
}

