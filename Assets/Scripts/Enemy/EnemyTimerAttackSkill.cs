using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Unit;
using Sirenix.OdinInspector;
using WE.Support;
using UniRx;
namespace WE.SkillEnemy
{
    public class EnemyTimerAttackSkill : EnemyAttackSkill
    {
        public bool randomInterval;

        [HideIf("randomInterval", true)]
        public float interval = 3;
        [ShowIf("randomInterval", true)]
        public float minInterval = 1, maxInterval = 5;

        protected bool newInterval;
        protected override void OnInit()
        {
            base.OnInit();
            StartTimer();
        }
        protected virtual void StartTimer()
        {
            newInterval = false;
            disposables = new CompositeDisposable();
            if (randomInterval)
            {
                interval = Random.Range(minInterval, maxInterval);
                Observable.Timer(System.TimeSpan.FromSeconds(interval)).Subscribe(_ => OnInterval()).AddTo(disposables);
            }
            else
            {
                Observable.Interval(System.TimeSpan.FromSeconds(interval)).Subscribe(_ => OnInterval()).AddTo(disposables);
            }
        }
        protected virtual void OnInterval()
        {
            ExcuteSkill();
            if (randomInterval)
            {
                disposables.Dispose();
                Observable.Timer(System.TimeSpan.FromSeconds(interval)).Subscribe(_ => OnInterval()).AddTo(disposables);
            }
        }
        public override void ExcuteSkill()
        {
            
        }
    }
}

