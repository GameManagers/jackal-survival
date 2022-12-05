using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Effect;
using WE.Manager;
using WE.Pooling;
using WE.Support;

namespace WE.Game
{
    public class ChestInGame : PoolingObject
    {
        public AnimationEffect fxChestExplose;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Hit();
        }
        public void Hit()
        {
            Helper.SpawnEffect(fxChestExplose, transform.position, null);
            GameplayManager.Instance.NormalChestDie(transform.position);
            Despawn();
        }
    }
}

