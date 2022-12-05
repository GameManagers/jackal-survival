using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Manager;
using WE.Tank;
using WE.Unit;

namespace WE.GameAction.Tank
{
    public class TankMovement : MonoBehaviour
    {
        private MoveControl moveControl;
        public bool IsTouching
        {
            get
            {
                if (moveControl != null)
                {
                    return moveControl.bTouchMove;
                }
                return false;
            }

        }

        public void Init()
        {
            IsInitialize = true;
            moveControl = new MoveControl();
            moveControl.RegisterJoyEvent();
        }
        public void Stop()
        {
            IsInitialize = false;
            moveControl = null;
        }
        public float Angle => -moveControl.Angle;
        public Vector2 Direction => moveControl.Direction;
        public float Length => moveControl.m_JoyData.length;

        [Header("Game Object that visualize this Unit")]
        public GameObject Icon;
        [SerializeField]
        private Transform vetXeJeep;

        bool IsInitialize;
        bool isCustomSpeed;
        float MoveSpeed => Player.Instance.MoveSpeed;
        float movespeedCustom;
        private void FixedUpdate()
        {
#if UNITY_EDITOR
            if (GameplayManager.Instance != null)
            {
#endif
                if (IsInitialize && GameplayManager.Instance.State == GameState.Gameplay)
                {
                    if (IsTouching)
                    {
                        //if (Length > 30)
                        //{
                        //    isCustomSpeed = false;
                        //transform.Translate(Direction.normalized * Time.deltaTime * MoveSpeed);

                        //}
                        //else
                        //{
                        //    if (!isCustomSpeed)
                        //    {
                        //        isCustomSpeed = true;
                        //        movespeedCustom = MoveSpeed / 2;
                        //    }

                        //    if (movespeedCustom < MoveSpeed)
                        //    {
                        //        movespeedCustom += 0.02f * MoveSpeed;
                        //        transform.Translate(Direction.normalized * Time.deltaTime * movespeedCustom);
                        //    }
                        //    else
                        //    {
                        //        transform.Translate(Direction.normalized * Time.deltaTime * MoveSpeed);
                        //    }
                        //}
                        transform.position += (Vector3)Direction * Time.fixedDeltaTime * MoveSpeed;
                        Icon.transform.eulerAngles = new Vector3(0, 0, Angle);
                        vetXeJeep.eulerAngles = Icon.transform.eulerAngles;
                        CarController.Instance.UpdateGunPos();
                    }
                }
#if UNITY_EDITOR
            }
#endif
        }
    }
}