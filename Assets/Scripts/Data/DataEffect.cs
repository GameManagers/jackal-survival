using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using WE.Unit;
using System;
using WE.Effect;

namespace WE.Manager
{
    [CreateAssetMenu(fileName = "Effect Data", menuName = "WE/Data/Data Effect")]
    public class DataEffect : SerializedScriptableObject
    {
        public List<AnimationEffect> listEffect;
        public Dictionary<string, AnimationEffect> dictEffect;

        [Button("Gen Dic")]
        public void GenDic()
        {
            dictEffect = new Dictionary<string, AnimationEffect>();
            for (int i = 0; i < listEffect.Count; i++)
            {
                dictEffect.Add(listEffect[i].poolingKey, listEffect[i]);
            }
        }
    }
}
