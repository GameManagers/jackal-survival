using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Bullet;
using UniRx;
using WE.Unit;

using Sirenix.OdinInspector;
namespace WE.Skill
{
    public class ShockZoneSupportSkill : ActiveSkill
    {
        public ShockZoneBullet childBullet;

        protected override void ExcuteSkill()
        {
            childBullet.gameObject.SetActive(true);
            InitBullet(childBullet);
            StopMiniObserve();
            miniDisposables = new CompositeDisposable();
            Observable.Interval(System.TimeSpan.FromSeconds(1.0f / BaseHitPerSec)).Subscribe(_ => InitBullet(childBullet)).AddTo(miniDisposables);
        }
        public override void Dispose()
        {
            base.Dispose();
            childBullet.gameObject.SetActive(false);
        }

    }
}

