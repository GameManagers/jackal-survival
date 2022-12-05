using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using WE.Support;
using DG.Tweening;
using WE.Unit;
using WE.Manager;

namespace WE.Bullet
{
    public class GravityTower : PlayerBullet
    {
        public Transform Icon1;
        public Transform Icon2;
        public float rotateSpeed1 = 360;
        public float rotateSpeed2 = 360;
        public float lastWaveMultiple = 1;

        public AnimationCurve lastWaveScaleCurve;
        public float lastWaveScaleTime = 1;
        public override void ShotBullet()
        {
            if (hitFx != null)
                hitFx.OnSpawn();
            transform.localScale = Vector3.one * AreaScale;
            disposable = new CompositeDisposable();
            Observable.Timer(System.TimeSpan.FromSeconds(duration)).Subscribe(_ => Explose()).AddTo(disposable);
            Observable.Interval(System.TimeSpan.FromSeconds(interval)).Subscribe(_ => CastDamage()).AddTo(disposable);
            if (Icon1 != null)
                Icon1.DORotate(Vector3.forward * 36000, rotateSpeed1, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetSpeedBased();
            float timerotate = 36000 / rotateSpeed2;
            if (Icon2 != null)
                Icon2.DORotate(-Vector3.forward * 36000, timerotate, RotateMode.WorldAxisAdd).SetEase(Ease.Linear);
        }
        public void CastDamage()
        {
            Helper.CastDamage(transform.position, AreaScale / 2, layerCast, BulletDamage, -PushBackForce, this.transform, null, 1, false, hitSfx);
        }
        public void Explose()
        {
            transform.DOScale(Vector3.one * 0.1f, lastWaveScaleTime).SetEase(lastWaveScaleCurve).OnComplete(() =>
            {
                Helper.CastDamage(transform.position, AreaScale / 2, layerCast, BulletDamage * lastWaveMultiple, -PushBackForce * lastWaveMultiple, this.transform);
                Despawn();
            });
        }
        public override void Hit(Enemy e)
        {
            e.TakeDamage(bulletDamage, -pushBackForce, this.transform);
        }
        public override void Despawn()
        {
            if (Icon1 != null)
                Icon1.DOKill();
            if (Icon2 != null)
                Icon2.DOKill();
            base.Despawn();
        }
    }
}

