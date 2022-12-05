using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Pooling;
using Sirenix.OdinInspector;
using DG.Tweening;
using UniRx;

namespace WE.Effect
{
    public class AnimationEffect : PoolingObject
    {

        [SerializeField]
        protected bool useAnimator;
        [SerializeField, ShowIf("useAnimator", true)]
        protected bool loopAnim;
        [SerializeField, ShowIf("useAnimator", true)]
        protected bool fadeDie;
        [SerializeField, ShowIf("fadeDie", true)]
        protected float timeStay = 1, timefade = 1;
        [SerializeField, ShowIf("useAnimator", true)]
        protected SpriteRenderer spr;
        [SerializeField, ShowIf("useAnimator", true)]
        protected Sprite[] spriteSet;
        [SerializeField, ShowIf("useAnimator", true)]
        protected float playTime;
        [SerializeField, HideIf("useAnimator", true)]
        protected ParticleSystem particles;

        public float PlayTime
        {
            get => playTime;
            set
            {
                playTime = value;
                Stop();
                PlayFx();
            }
        }

        protected int spId = 0;
        public void SetFlipX(bool flip)
        {
            spr.flipX = flip;
        }
        protected void Awake()
        {
            if (particles != null)
            {
                var main = particles.main;
                main.playOnAwake = false;
            }
        }
        public override void OnSpawn()
        {
            base.OnSpawn();
            if (fadeDie)
                spr.DOFade(1, 0);
            PlayFx();
        }
        public void Stop()
        {
            if (disposable != null)
                disposable.Dispose();
            if (particles != null)
                particles.Stop();
            if (useAnimator)
                spr.sprite = spriteSet[0];
            if (fadeDie)
                spr.DOKill();
        }
        protected void PlayFx()
        {
            if (disposable != null)
                disposable.Dispose();
            if (useAnimator)
            {
                int id = (int)(-transform.position.y * 100);
                spr.sortingOrder = id;
                if (playTime <= 0)
                {
                    DebugCustom.Log(name + " frameRate Cannot lower than 0");
                    return;
                }
                spId = -1;
                disposable = new CompositeDisposable();
                Observable.Interval(System.TimeSpan.FromSeconds(playTime / spriteSet.Length)).Subscribe(_ => PlayAnim()).AddTo(disposable);
                if (loopAnim && fadeDie) FadeDie();
            }
            else
            {
                if (particles == null)
                    return;
                var main = particles.main;
                main.stopAction = ParticleSystemStopAction.Callback;
                particles.Play();
            }
        }
        protected void PlayAnim()
        {
            spId++;
            if (spId >= spriteSet.Length)
            {
                if (loopAnim)
                {
                    spId = 0;
                    spr.sprite = spriteSet[spId];
                }
                else
                {
                    if (fadeDie)
                    {
                        FadeDie();
                    }
                    Despawn();
                }
            }
            else
            {
                spr.sprite = spriteSet[spId];
            }
        }
        protected void OnParticleSystemStopped()
        {
            Despawn();
        }
        public virtual void FadeDie()
        {
            if (timeStay > 0)
                Observable.Timer(System.TimeSpan.FromSeconds(timeStay)).Subscribe(_ => spr.DOFade(0.2f, timefade).OnComplete(() => { Despawn(); })).AddTo(disposable);
            else
                spr.DOFade(0.2f, timefade).OnComplete(() => { Despawn(); });

        }
        public override void Despawn()
        {
            Stop();
            base.Despawn();
        }
        protected override void OnDisable()
        {
            Stop();
            base.OnDisable();
        }
    }
}

