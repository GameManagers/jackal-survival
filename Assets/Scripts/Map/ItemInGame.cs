using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WE.Unit;
using WE.Pooling;
using WE.Manager;
using WE.Support;
using UniRx;
using WE.Effect;

namespace WE.Game
{
    public class ItemInGame : PoolingObject
    {
        public EItemInGame type;
        public int value;
        public SpriteRenderer icon;
        public Collider2D _collider;
        bool reviced = false;
        public System.Action<ItemInGame> OnDespawn;

        AnimationEffect fxItem;
        Tween tweenRevice;
        Tween idleTween;

        float reviceSpeed;
        bool revicedFrame;
        public override void OnSpawn()
        {
            base.OnSpawn();
            reviced = false;
            revicedFrame = false;
            //_collider.enabled = true;
            reviceSpeed = ObjectPooler.Instance.itemFlySpeed;
        }
        public void CheckType()
        {
            if (type == EItemInGame.Bomb || type == EItemInGame.Magnetic || type == EItemInGame.Heal || type == EItemInGame.Boss_Chest || type == EItemInGame.EndlessKey)
            {
                float offset = 0.15f;
                if (type == EItemInGame.Boss_Chest)
                    offset = 0.25f;
                idleTween = icon.transform.DOMove(transform.position + transform.up * offset, 0.3f).SetEase(Ease.InOutQuad).SetLoops(1000, LoopType.Yoyo);
                fxItem = Helper.SpawnEffect(ObjectPooler.Instance.fxItem, transform.position, null);
            }
            //tweenRevice = transform.DOMove(transform.position + (transform.position - Player.Instance.transform.position).normalized * ObjectPooler.Instance.flyBackDistance, ObjectPooler.Instance.itemFlyTime)
            //            .OnComplete(() =>
            //            {
            //                transform.SetParent(Player.Instance.transform);
            //                if (Helper.IsInScreen(transform.position))
            //                {
            //                    transform.DOLocalMove(Vector3.zero, ObjectPooler.Instance.itemFlySpeed * Random.Range(0.8f, 1.2f)).SetSpeedBased().OnComplete(() =>
            //                    {
            //                        GameplayManager.Instance.ReviceItem(type, value);
            //                        Despawn();
            //                    });
            //                }
            //                else
            //                {
            //                    transform.DOLocalMove(Vector3.zero, ObjectPooler.Instance.itemFlySpeed * 3 * Random.Range(0.8f, 1.2f)).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() =>
            //                    {
            //                        GameplayManager.Instance.ReviceItem(type, value);
            //                        Despawn();
            //                    });
            //                }
            //                //Despawn();
            //            });
            //tweenRevice.Pause();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Recevied();
        }
        public void SetSprite(Sprite _icon, int _sortingOder)
        {
            icon.sprite = _icon;
            icon.sortingOrder = _sortingOder;
        }
        public void Recevied(bool fly = true)
        {
            if (!reviced)
            {
                if (idleTween != null)
                    idleTween.Kill();
                reviced = true;
                if (fxItem != null)
                    fxItem.Despawn();
                //_collider.enabled = false;
                //if (fly)
                //{
                //    if (idleTween != null)
                //    {
                //        idleTween.Kill();
                //    }
                //    tweenRevice.Play();
                //}
                //else
                //{
                //    float timer = 0.2f;
                //    if (!Helper.IsInScreen(transform.position))
                //    {
                //        timer = 0.5f * Random.Range(0.8f, 1.2f);
                //    }
                //    disposable = new CompositeDisposable();
                //    Observable.Timer(System.TimeSpan.FromSeconds(timer)).Subscribe(_ =>
                //    {
                //        GameplayManager.Instance.ReviceItem(type, value);
                //        Despawn();
                //        ;
                //    }).AddTo(disposable);
                //}
            }
        }
        public override void Despawn()
        {
            if (idleTween != null)
                idleTween.Kill();
            if (type == EItemInGame.Exp)
                ObjectPooler.Instance.listExp.Remove(this);
            tweenRevice.Kill();
            OnDespawn?.Invoke(this);
            base.Despawn();
        }
        public void OnUpdate(float t)
        {
            icon.enabled = Helper.IsInScreen(transform.position);
            Vector3 dir = Player.Instance.transform.position - transform.position;
            if (!reviced)
            {
                if (!Helper.IsInScreen(transform.position))
                    return;
                float dis2 = Vector3.SqrMagnitude(dir);
                if (dis2 <= Player.Instance.AbsorbRange * Player.Instance.AbsorbRange)
                    Recevied();
            }
            else
            {
                if (!revicedFrame)
                {

                    float dis2 = Vector3.SqrMagnitude(dir);
                    float step = reviceSpeed * Time.deltaTime;
                    if (step > 0 && step * step >= dis2)
                    {
                        transform.position = Player.Instance.transform.position;
                        revicedFrame = true;
                    }
                    else
                    {
                        transform.position += dir.normalized * step;
                    }
                    reviceSpeed += ObjectPooler.Instance.itemFlySpeedIncrease * Time.deltaTime;
                }
                else if(Player.Instance.IsAlive)
                {
                    GameplayManager.Instance.ReviceItem(type, value);
                    Despawn();
                }
            }
        }
    }
}

