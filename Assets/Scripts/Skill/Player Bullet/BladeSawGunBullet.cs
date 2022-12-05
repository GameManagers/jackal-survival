using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using System;
using WE.Support;
using WE.Unit;

namespace WE.Bullet
{
    public class BladeSawGunBullet : PlayerBullet
    {
        public SpriteRenderer Icon;
        public float rotateSpeed = 360;
        public AnimationCurve goCurve;
        public AnimationCurve returnCurve;
        public override void ShotBullet()
        {
            transform.localScale = Vector3.one * AreaScale;
            Vector3 pos1 = transform.position + transform.up * MaxDistance;
            Icon.transform.DORotate(Vector3.forward * 360000, rotateSpeed, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetSpeedBased();
            GoOut(pos1);
        }
        public void GoOut(Vector3 pos1)
        {
            transform.DOMove(pos1, BulletSpeed).SetEase(goCurve).SetSpeedBased().OnComplete(() => {

                if (Duration > 0)
                {
                    disposable = new CompositeDisposable();
                    Observable.Timer(System.TimeSpan.FromSeconds(duration)).Subscribe(_ => GoBack()).AddTo(disposable);
                    Observable.Interval(System.TimeSpan.FromSeconds(Interval)).Subscribe(_ => CastDamage()).AddTo(disposable);
                }
                else
                {
                    GoBack();
                }
            });
        }

        private void CastDamage()
        {
            Helper.CastDamage(transform.position, AreaScale / 2, layerCast, BulletDamage, PushBackForce, null, hitFx, fxScale, hitSfx);
        }

        public void GoBack()
        {
            Vector3 pos2 = transform.position+(Player.Instance.transform.position - transform.position).normalized * MaxDistance * 3;
            if (disposable != null)
                disposable.Dispose();
            transform.DOMove(pos2, BulletSpeed).SetEase(returnCurve).SetSpeedBased().OnComplete(() => { Despawn(); });
        }
        public override void Despawn()
        {
            Icon.transform.DOKill();
            base.Despawn();
        }
    }
}

