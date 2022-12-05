using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TigerForge;
namespace WE.Unit
{
    public class EnemyImpactZone : MonoBehaviour
    {
        Enemy Owner;
        float impactDamage;
        float impactInterval;
        float tImpact;
        public void SetOwner(Enemy source, float damage, float _interval)
        {
            Owner = source;
            impactDamage = damage;
            impactInterval = _interval;
            tImpact = 0;
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (tImpact <= 0)
            {
                Player tank = collision.gameObject.GetComponent<Player>();
                if (tank != null)
                {
                    tank.TakeDamage(impactDamage, Owner);
                    tImpact = impactInterval;
                }
            }
        }
        public void OnUpdate(float t)
        {
            tImpact -= t;
        }
    }
}

