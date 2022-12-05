using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Support;
using DG.Tweening;
using UniRx;
using Sirenix.OdinInspector;
namespace WE.Pooling
{
    public class PoolingObject: MonoBehaviour
    {
        //[SerializeField, ReadOnly]
        //protected bool available = false;
        //public bool Available
        //{
        //    get => available;
        //    set
        //    {
        //        available = value;
        //    }
        //}
        public string poolingKey;
        public bool dontNeedDespawn = false;
        public CompositeDisposable disposable;
        public virtual void OnSpawn()
        {

        }
        public virtual void Despawn()
        {
            transform.DOKill();
            if (disposable != null)
                disposable.Dispose();
            Helper.Despawn(this);
        }
        protected virtual void OnDisable()
        {

        }
    }
}

