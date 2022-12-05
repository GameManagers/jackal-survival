using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WE.Support;
using UniRx;
using Sirenix.OdinInspector;
using System;

public class TestScripts : MonoBehaviour
{
    //UI Counter;
    int count;
    private void Start()
    {
        count = 0;
        InstatntiateOBJ.Instance.OnKillCount += CountPlus;
    }

    public void CountPlus()
    {
        count++;
    }
}
