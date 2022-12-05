using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Effect;
using DG.Tweening;
using WE.Support;
using UniRx;
using WE.Unit;
using WE.Manager;

namespace WE.Bullet
{
    public class MolotovBullet : PlayerBullet
    {
        public SpriteRenderer icon;
        public float rotateAngle;
        public AnimationEffect exploseFx;
        public AnimationEffect fadeFx;
        public AnimationEffect groundFx;
        public Vector3 tarGetPos;
        AnimationEffect currentFx;
        public override void ShotBullet()
        {
            float flyTime = maxDistance / bulletSpeed;
            icon.enabled = true;
            transform.DOMove(tarGetPos , flyTime ).SetEase(Ease.Linear).OnComplete(() => { StartBurn(); });
            transform.DORotate(new Vector3(0, 0, rotateAngle), flyTime, RotateMode.WorldAxisAdd).SetEase(Ease.Linear);
        }
        public void StartBurn()
        {
            icon.enabled = false;
            Helper.SpawnEffect(exploseFx, transform.position, null, AreaScale);
            if (hitSfx != null)
            SoundManager.Instance.PlaySoundFx(hitSfx);
            currentFx = Helper.SpawnEffect(hitFx, transform.position, null, AreaScale);
            disposable = new CompositeDisposable();
            Observable.Interval(System.TimeSpan.FromSeconds(Interval)).Subscribe(_ => CastDamage()).AddTo(disposable);
            Observable.Timer(System.TimeSpan.FromSeconds(Duration)).Subscribe(_ => TimeOut()).AddTo(disposable);
        }
        public void TimeOut()
        {
            currentFx.Despawn();
            Helper.SpawnEffect(fadeFx, currentFx.transform.position, null, AreaScale); 
            Helper.SpawnEffect(groundFx, currentFx.transform.position, null, AreaScale); 
            Despawn();
        }
        public void CastDamage()
        {
            Helper.CastDamage(transform.position, AreaScale/2, layerCast, BulletDamage, PushBackForce);
        }
    }
}

