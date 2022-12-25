using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class UIResourceItem : MonoBehaviour
{
    [SerializeField]
    private Text txtCount;

    public void SetValue(int value)
    {
        txtCount.text = "x" + value.ToString();
    }
}
