using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using WE.Pooling;
using UniRx;
namespace WE.Game
{
    public class TextDamage : PoolingObject
    {
        public TextMeshPro textDamage;
        ObjectPooler pooler => ObjectPooler.Instance;
        public void InitText(float value, bool isCrit, bool isHeal = false)
        {
            textDamage.text = ((int)value).ToString();
            textDamage.color = new Color(1, 1, 1, 0.6f);
            textDamage.fontSize = pooler.normalTextSize/2;
            disposable = new CompositeDisposable();
            transform.DOMove(transform.position + (Vector3)Random.insideUnitCircle * pooler.textRandomOffset, pooler.fadeTime);
            if (isCrit)
            {
                textDamage.color = pooler.critHitColor;
                textDamage.DOFontSize(pooler.critTextSize, pooler.fadeTime);
                textDamage.DOFade(1, pooler.fadeTime).OnComplete(() => {
                    FadeOut();
                    //Observable.Timer(System.TimeSpan.FromSeconds(pooler.critTextTime)).Subscribe(_ => FadeOut()).AddTo(disposable);
                });
               
            }
            else if(!isHeal)
            {
                textDamage.color = pooler.normalHitColor;
                textDamage.DOFontSize(pooler.normalTextSize, pooler.fadeTime);
                textDamage.DOFade(1, pooler.fadeTime).OnComplete(() => {
                    //Observable.Timer(System.TimeSpan.FromSeconds(pooler.normalTextTime)).Subscribe(_ => FadeOut()).AddTo(disposable);
                    FadeOut();
                });
            }
            else
            {
                textDamage.text = "+" + ((int)value).ToString();
                textDamage.color = pooler.healTextColor;
                textDamage.DOFontSize(pooler.healTextSize, pooler.fadeTime);
                textDamage.DOFade(1, pooler.fadeTime).OnComplete(() => {
                    //Observable.Timer(System.TimeSpan.FromSeconds(pooler.normalTextTime)).Subscribe(_ => FadeOut()).AddTo(disposable);
                    FadeOut();
                });
            }
        }
        public void FadeOut()
        {
            textDamage.DOFade(0.6f, pooler.fadeTime).OnComplete(()=> { Despawn(); });
        }
        public override void Despawn()
        {
            textDamage.DOKill();
            base.Despawn();
        }
    }
}

