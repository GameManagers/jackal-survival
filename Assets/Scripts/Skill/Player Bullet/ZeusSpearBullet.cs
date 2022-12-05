using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using WE.Skill;
using WE.Support;
using WE.Utils;
using WE.Unit;
using WE.Effect;

namespace WE.Bullet
{
    public class ZeusSpearBullet : PlayerBullet
    {
        public GameObject FxLightning;
        public SpriteRenderer Icon;
        public Sprite flySprite, groundSprite;

        public ZeusSpearActiveSkill parentSkill;
        public Vector3 targetPos;
        public List<ZeusSpearChainLightning> listChain;
        AnimationEffect fx;

        public bool IsEvole;
        public override void ShotBullet()
        {
            FxLightning.SetActive(false);
            Icon.sprite = flySprite;
            Icon.sortingLayerName = Constant.SORTING_LAYER_Bullet;
            listChain = new List<ZeusSpearChainLightning>();
            transform.DOPath(new Vector3[] { targetPos }, BulletSpeed, PathType.CatmullRom, PathMode.TopDown2D).SetSpeedBased().SetLookAt(0).SetEase(Ease.Linear).OnComplete(() =>
            {
                FxLightning.SetActive(true);
                transform.eulerAngles = Vector3.zero;
                disposable = new CompositeDisposable();
                Observable.Timer(System.TimeSpan.FromSeconds(Duration)).Subscribe(_ => StopSpear()).AddTo(disposable);
                if (IsEvole)
                    EvoleDamageZone();
                Icon.sprite = groundSprite;
                Icon.sortingLayerName = Constant.SORTING_LAYER_OBSTACLE;
                parentSkill.activeZeusSpear.Add(this);
                parentSkill.SetChain();
            });
        }
        public void EvoleDamageZone()
        {
            if (fx == null)
            {
                fx = Helper.SpawnEffect(hitFx, transform.position, null, AreaScale);
            }
            else
            {
                fx.transform.position = transform.position;
                fx.transform.localScale = Vector3.one * areaScale;
                fx.gameObject.SetActive(true);
            }
            Observable.Interval(System.TimeSpan.FromSeconds(interval)).Subscribe(_ => CastDamage()).AddTo(disposable);
        }
        public void CastDamage()
        {
            Helper.CastDamage(transform.position, AreaScale / 2, layerCast, BulletDamage, PushBackForce, null, null, 1, true);
        }
        public void StopSpear()
        {
            if (fx != null)
            {
                fx.Despawn();
                fx = null;
            }
            listChain.Clear();
            parentSkill.StopSpear(this);
            Despawn();
        }
        public override void PlayFx(Vector3 pos)
        {

        }
    }
}

