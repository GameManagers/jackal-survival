using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Unit;
using UniRx;
using WE.Effect;

namespace WE.Bullet
{
    public class FlameGunBurningBullet : PlayerBullet
    {
        public Enemy e;
        public AnimationEffect fxBurn;
        public float damageMul;
        public float intervalMul;
        public float durationMul;
        public override void ShotBullet()
        {
            gameObject.SetActive(true);
            fxBurn.OnSpawn();
            e.OnEnemyDie += OnEnemyDie;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            disposable = new CompositeDisposable();
            Observable.Interval(System.TimeSpan.FromSeconds(intervalMul * interval)).Subscribe(_ => e.TakeDamage(damageMul * BulletDamage, 0.01f, null, false, false)).AddTo(disposable);
            Observable.Timer(System.TimeSpan.FromSeconds(duration * durationMul)).Subscribe(_ => OnEnemyDie(e)).AddTo(disposable);
        }
        public virtual void OnEnemyDie(Enemy e)
        {
            e.OnEnemyDie -= OnEnemyDie;
            Despawn();
        }
    }
}

