using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSizeHelper : MonoBehaviour
{
    static ScreenSizeHelper _instance;
    public static ScreenSizeHelper Instance => _instance;
    Vector2 lastScreenSize = new Vector2(Screen.width, Screen.height);
    //public event System.Action OnChangeScreenSize;
    private void Awake()
    {
        _instance = this;
    }
    //private void LateUpdate()
    //{
    //    if (Time.frameCount % 3 == 0)
    //    {
    //        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
    //        if (this.lastScreenSize != screenSize)
    //        {
    //            lastScreenSize = screenSize;
    //            OnChangeScreenSize?.Invoke();
    //        }
    //    }
    //}
    public static float ScreenLeftEdge
    {
        get
        {
            Vector2 topLeft = new Vector2(0, 1);
            Vector2 topLeftInScreen = Camera.main.ViewportToWorldPoint(topLeft);
            return topLeftInScreen.x;
        }
    }

    public static float ScreenRightEdge
    {
        get
        {
            Vector2 edge = new Vector2(1, 0);
            Vector2 edgeVector = Camera.main.ViewportToWorldPoint(edge);
            return edgeVector.x;
        }
    }

    public static float ScreenTopEdge
    {
        get
        {
            Vector2 edge = new Vector2(1, 1);
            Vector2 edgeVector = Camera.main.ViewportToWorldPoint(edge);
            return edgeVector.y;
        }
    }

    public static float ScreenBottomEdge
    {
        get
        {
            Vector2 edge = new Vector2(1, 0);
            Vector2 edgeVector = Camera.main.ViewportToWorldPoint(edge);
            return edgeVector.y;
        }
    }
}
