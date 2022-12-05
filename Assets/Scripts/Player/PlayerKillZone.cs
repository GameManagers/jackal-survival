using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Unit;

namespace WE.Game
{
    public class PlayerKillZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Enemy e = collision.GetComponent<Enemy>();
            if (e != null)
                e.Die();
        }
    }
}

