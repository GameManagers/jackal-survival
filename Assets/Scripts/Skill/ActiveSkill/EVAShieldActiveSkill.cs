using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Unit;
using UniRx;
using WE.Effect;
using WE.Support;
using Sirenix.OdinInspector;
using WE.Manager;

namespace WE.Skill
{
    public class EVAShieldActiveSkill : ActiveSkill
    {
        public GameObject shieldIcon;

        public AnimationEffect fxExplose;
        public LayerMask layerCast;
        public AudioClip Sfx;
        public override void Init()
        {
            base.Init();
            Player.Instance.SetShield(shieldIcon);
        }
        protected override void ExcuteSkill()
        {
            Player.Instance.SetImortal(BaseDuration);
            StopMiniObserve();
            miniDisposables = new CompositeDisposable();
            Observable.Timer(System.TimeSpan.FromSeconds(BaseDuration)).Subscribe(_ => ShieldRunOut()).AddTo(miniDisposables);
        }
        public void ShieldRunOut()
        {
            //Player.Instance.StopImortal();
            //shieldIcon.SetActive(false);
            if (IsEvoleSkill)
            {
                SoundManager.Instance.PlaySoundFx(Sfx);
                Helper.SpawnEffect(fxExplose, transform.position, this.transform, baseAreaScale);
                Helper.CastDamage(transform.position, baseAreaScale / 2, layerCast, baseDamage, PushBackForce);
            }
        }
        public override void Dispose()
        {
            Player.Instance.StopImortal();
            Player.Instance.SetShield(null);
            base.Dispose();
        }
    }
}

