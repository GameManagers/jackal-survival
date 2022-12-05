using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using WE.Manager;
using System;
using WE.Unit;
using WE.Support;
using WE.Utils;

public class EnemySpriteController : MonoBehaviour
{
    Enemy Owner;
    public SpriteRenderer spr;
    public SpriteRenderer hitSpr;
    float animLoopInterval => Owner.animFrameRate;
    float flashTime => Owner.flashTime;
    Sprite[] animSprite => Owner.spriteSet;
    Sprite flashSprite => Owner.spriteFlash;
    CompositeDisposable disposables;
    CompositeDisposable hitdisposables;
    int idSp;
    public void EnableSprite(bool end)
    {
        spr.enabled = end;
        hitSpr.enabled = end;
    }
    public void FlipX(bool flip)
    {
        spr.flipX = flip;
        hitSpr.flipX = flip;
    }
    public void Init(Enemy _owner)
    {
        Owner = _owner;
        Owner.OnTakeDamage += TakeDamage;
        idSp = 0;
        RunCustomAnimator();
        hitSpr.gameObject.SetActive(false);
    }
    public void RunCustomAnimator()
    {
        if (disposables != null)
            disposables.Dispose();
        if (animLoopInterval <= 0)
        {
            DebugCustom.LogError(name + "Anim Loop Interval Can't be lower than 0");
            return;
        }
        disposables = new CompositeDisposable();
        Observable.Interval(TimeSpan.FromSeconds(animLoopInterval)).Subscribe(_ => ChangeSprite()).AddTo(disposables);
    }
    public void ChangeSprite()
    {
        //Debug.Log("RunCoroutine");
        int id = (int)(-transform.position.y * 100);
        spr.sortingOrder = id;
        hitSpr.sortingOrder = id + 1;
        if (Owner == null || !Owner.isActiveAndEnabled)
            return;
        spr.sprite = animSprite[idSp];
        idSp++;
        if (idSp >= animSprite.Length)
            idSp -= animSprite.Length;
    }
    private void OnDisable()
    {
        if (Owner != null)
        Owner.OnTakeDamage -= TakeDamage;
        if (disposables != null)
            disposables.Dispose();
        if (hitdisposables != null)
            hitdisposables.Dispose();
    }
    private void TakeDamage(bool shocked, bool showHit = true)
    {
        //if (!Owner.IsAlive || !showHit)
        //    return;
        //if (disposables != null)
        //    disposables.Dispose();
        //if (shocked)
        //    spr.sprite = Helper.GetElectricShockSprite();
        //else
        //    spr.sprite = flashSprite;
        //disposables = new CompositeDisposable();
        float ft = flashTime;
        if (Owner.IsBoss)
            ft = 0.01f;
        //Observable.Interval(TimeSpan.FromSeconds(ft)).Subscribe(_ => RunCustomAnimator()).AddTo(disposables);
        if (shocked)
        {
            hitSpr.sprite = Helper.GetElectricShockSprite();
        }
        else
        {
            hitSpr.sprite = flashSprite;
        }
        hitSpr.gameObject.SetActive(true);
        if (hitdisposables != null)
        {
            hitdisposables.Dispose();
        }
        hitdisposables = new CompositeDisposable();
        Observable.Interval(TimeSpan.FromSeconds(ft)).Subscribe(_ => hitSpr.gameObject.SetActive(false)).AddTo(hitdisposables);
    }
}
