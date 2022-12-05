using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Manager;

namespace WE.Pooling
{
    public class AudioSourcePoolingObj : PoolingObject
    {
        public AudioSource audioSource;
        public string currentClipPlaying;
        public override void Despawn()
        {
            if (SoundManager.Instance.playingSoundDic.ContainsKey(currentClipPlaying))
            {
                SoundManager.Instance.playingSoundDic[currentClipPlaying]--;
            }
            base.Despawn();
        }
    }
}

