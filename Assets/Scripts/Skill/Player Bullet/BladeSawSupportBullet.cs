using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using WE.Unit;

namespace WE.Bullet
{
    public class BladeSawSupportBullet : PlayerBullet
    {
        public float RotateSpeed = 360;
        public Transform Icon;
        public override void Despawn()
        {
            Icon.transform.DOKill();
            base.Despawn();
        }
        public override void ShotBullet()
        {
            transform.localScale = Vector3.one * 0.1f;
            transform.DOScale(Vector3.one * AreaScale, 0.5f).SetEase(Ease.Linear);
            transform.DOLocalMove(transform.right * maxDistance * AreaScale, 1).SetEase(Ease.Linear).OnComplete(() => {
                disposable = new CompositeDisposable();
                Observable.Timer(System.TimeSpan.FromSeconds(Duration)).Subscribe(_ => BulletCallBack()).AddTo(disposable);
            });
            Icon.transform.DORotate(Vector3.forward * 360000, RotateSpeed, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetSpeedBased();

        }
        public void BulletCallBack()
        {

            transform.DOScale(Vector3.one * 0.1f, 0.5f).SetEase(Ease.Linear);
            transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.Linear).OnComplete(() => {
                Despawn();
            });
        }
    }
}

