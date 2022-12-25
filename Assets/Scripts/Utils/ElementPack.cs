using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementPack : MonoBehaviour
{
    public UIResourceItem item;
    public GameObject gChecker;

    public void SetStatus(bool isClaimed)
    {
        gChecker.SetActive(isClaimed);
    }
}
