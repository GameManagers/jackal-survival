using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WE.Manager
{
    public class SpriteManager : MonoBehaviour
    {
        [FoldoutGroup("Sprite Avatar")]
        [SerializeField]
        private Sprite[] avatars;

        [FoldoutGroup("Sprite PVP")]
        [SerializeField]
        private Sprite[] ranksPvp;

        public static SpriteManager Instance;

        private void OnDisable()
        {
            Instance = null;
        }
        private void Awake()
        {
            Instance = this;
        }


        public Sprite GetSpriteRankingPVP(ERankPVP _rank)
        {
            return ranksPvp[(int)_rank];
        }

        public Sprite GetSpriteAvatar(TypeAvatar avatar)
        {
            return avatars[(int)avatar];
        }
    }
}

