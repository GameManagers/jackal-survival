using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Unit;

public class MoveControl
{
    //private bool bMoveing;
    public bool bTouchMove;
    public JoyData m_JoyData;
    public float Angle => m_JoyData.angle %= 360f;
    public Vector3 Direction => m_JoyData._moveDirection;

    public System.Action StartMove;
    public System.Action Moving;
    public System.Action EndMove;
    public void RegisterJoyEvent()
    {
        JoyStick.On_JoyTouchStart += this.OnMoveStart;
        JoyStick.On_JoyTouching += this.OnMoving;
        JoyStick.On_JoyTouchEnd += this.OnMoveEnd;
    }

    public void RemoveJoyEvent()
    {
        JoyStick.On_JoyTouchStart -= this.OnMoveStart;
        JoyStick.On_JoyTouching -= this.OnMoving;
        JoyStick.On_JoyTouchEnd -= this.OnMoveEnd;
        StartMove = null;
        Moving = null;
        EndMove = null;
    }
    public void OnMoveStart(JoyData data)
    {
        //this.bMoveing = true;
        this.bTouchMove = true;
        StartMove?.Invoke();
    }
    public void OnMoving(JoyData data)
    {
        if (this.bTouchMove)
        {
            m_JoyData = data;
            this.m_JoyData._moveDirection = new Vector3(m_JoyData.direction.x, m_JoyData.direction.z);
            Moving?.Invoke();
        }
    }
    private void OnMoveEnd(JoyData data)
    {
        this.bTouchMove = false;
        this.m_JoyData.direction = Vector3.zero;
        this.m_JoyData._moveDirection = Vector3.zero;
        EndMove?.Invoke();
    }
}
