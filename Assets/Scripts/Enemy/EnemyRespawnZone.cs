using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Support;

namespace WE.Game
{
    public class EnemyRespawnZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            collision.transform.position = Helper.GetRandomSpawnPos();
        }
    }
}

