using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Support;
using WE.Unit;

namespace WE.Game
{
    public class TankRange : MonoBehaviour
    {
        public int MaxRangeCheck = 15;
        public LayerMask layerEnemy;
        public Enemy GetTarget()
        {
            return Helper.GetInRangeTarget(transform.position, MaxRangeCheck, layerEnemy);
        }
    }
}

