using System;
using UnityEngine;
using WE.Support;
using WE.Utils;

public struct JoyData
{
    public Vector3 direction;

    public Vector3 _moveDirection;

    public float angle;

    public float length;


    public void Revert()
    {
        this.direction *= -1f;
        this.angle = (this.angle + 180f) % 360f;
    }

    public void UpdateDirectionByAngle(float angle)
    {
        this.angle = angle;
        this.direction.x = Helper.Sin(angle);
        this.direction.z = Helper.Cos(angle);
    }

}
