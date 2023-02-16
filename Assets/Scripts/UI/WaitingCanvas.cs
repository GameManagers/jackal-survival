using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening.Core.Easing;

public class WaitingCanvas : MonoBehaviour
{
    private bool isShow;
    private IDisposable timeOutDisposable;
    private  void OnAwake()
    {
        if (!isShow)
            Hide();
    }

    public void Show(float timeout = 15, Action timeoutAction = null)
    {
        isShow = true;
        if (timeOutDisposable != null)
        {
            timeOutDisposable.Dispose();
        }
        gameObject.SetActive(true);
        timeOutDisposable = Observable.Timer(TimeSpan.FromSeconds(timeout), Scheduler.MainThreadIgnoreTimeScale).Subscribe(i =>
        {
            Hide();
            if (timeoutAction != null)
            {
                timeoutAction();
            }
        }).AddTo(gameObject);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        isShow = false;
        if (timeOutDisposable != null)
            timeOutDisposable.Dispose();
    }
}
