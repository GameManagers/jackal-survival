using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WE.Manager;
using WE.Support;
using WE.Utils;

public class JoyStick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
    public RectTransform parentCanvas;
    [SerializeField]
    private float speedDeltaDrag = 1.23f;
    //private int DesignWidth = 720;
    //private int DesignHeight = 1280;
    public Camera m_camera = null;
    public float DoubleClick = 0.3f;
    public float JoyRadius = 100f;
    public float JoyScaleBG = 100f;
    public float JoyScaleTouch = 100f;
    protected Vector2 Origin;
    private float mRadius;
    protected float mRadiusSmall;
    public CanvasGroup canvasGroup;
    public Transform child;
    public Transform bgParent;
    public Transform bgParengbgbg;
    public Transform touch;
    public Transform direction;
    private Vector3 StartPos;
    private bool bShowDirection = true;
    protected JoyData m_Data = default(JoyData);
    private bool disable;
    private bool touchdown;
    private Vector3 touchdownpos;
    private static bool TouchIn = true;
    private int mTouchID = -1;
    private float ClickDelayTime = 0.2f;
    //private bool bDrag;
    private Vector3 pos_v;
    private float pos_w;
    private Vector3 returnPos;
    private Vector3 pos_2 = new Vector3(0.5f, 0.5f, 0f);
    private Vector2 DealDrag_touchpos;
    private Vector2 DealDrag_touchpos1;
    public delegate void JoyTouchStart(JoyData data);
    public delegate void JoyTouching(JoyData data);
    public delegate void JoyTouchEnd(JoyData data);
    public static event JoyTouchStart On_JoyTouchStart;
    public static event JoyTouching On_JoyTouching;
    public static event JoyTouchEnd On_JoyTouchEnd;
    public static Action OnDoubleClick;
    private const float alphaDefault = 1f;
    private void OnEnable()
    {
        this.disable = false;
        this.touchdown = false;
        //_isNewJoystick = GameConfigManager.Instance.NewJoyStick;
        mTouchID = -1;
    }

    private void OnDisable()
    {
        this.OnPointerUp(null);
        this.disable = true;
        this.touchdown = false;
    }
    private void OnDestroy()
    {
        //TigerForge.EventManager.StopListening(WE.EventConstant.EVENT_SETUP_JOYSTICK, SetSacleJoystick);
    }
    private void Awake()
    {
        SetSacleJoystick();
        SetAlphaJoystick(alphaDefault);
        this.ClickDelayTime = DoubleClick;
        this.StartPos = this.child.localPosition;
        this.touch.localScale = Vector3.one * JoyScaleTouch / 100f;
        this.mRadius = (this.child as RectTransform).sizeDelta.x * 0.5f * JoyRadius / 100f;
        this.mRadiusSmall = this.mRadius;
        if (TouchIn)
        {
            this.mRadiusSmall = this.mRadius - (this.touch as RectTransform).sizeDelta.x * 0.5f * JoyRadius / 100f * this.touch.localScale.x;
        }
        this.mRadius *= JoyScaleBG / 100f;
        this.mRadiusSmall *= JoyScaleBG / 100f;
        this.bgParengbgbg.localScale = Vector3.one * JoyScaleBG / 100f;
        this.direction.gameObject.SetActive(this.bShowDirection);
        this.m_Data.direction = default(Vector3);
        mTouchID = -1;
        //TigerForge.EventManager.StartListening(WE.EventConstant.EVENT_SETUP_JOYSTICK, SetSacleJoystick);
    }
    private void SetSacleJoystick()
    {
        //float scaleJoystick = (Player.Instance.ScaleJoystick / GameConfigManager.Instance.Default_Scale_Joystick);
        //child.localScale = Vector3.one * scaleJoystick;
    }
    private void SetAlphaJoystick(float alpha)
    {
        canvasGroup.alpha = alpha;
    }
    private void OnValueChange()
    {
        this.mRadius = (this.child as RectTransform).sizeDelta.x * 0.5f * JoyRadius / 100f;
        this.mRadiusSmall = this.mRadius;
        if (TouchIn)
        {
            this.mRadiusSmall = this.mRadius - (this.touch as RectTransform).sizeDelta.x * 0.5f * JoyRadius / 100f * this.touch.localScale.x;
        }
        this.mRadius *= JoyScaleBG / 100f;
        this.mRadiusSmall *= JoyScaleBG / 100f;
        this.bgParengbgbg.localScale = Vector3.one * JoyScaleBG / 100f;
    }

    private Vector3 GetPos(Vector3 pos)
    {
        //this.pos_v = m_camera.ScreenToViewportPoint(pos) - this.pos_2;
        ////this.pos_v = m_camera.ScreenToViewportPoint(pos);
        //this.pos_w = (float)DesignHeight / (float)Screen.height * (float)Screen.width;
        //returnPos = new Vector3(this.pos_w * this.pos_v.x, (float)DesignHeight * this.pos_v.y, 0f);
        //return returnPos;
        ////return m_camera.ScreenToViewportPoint(pos);
        ///
        Vector2 _pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas, pos, m_camera, out _pos);
        return _pos;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.mTouchID == -1 && GameplayManager.Instance.State == GameState.Gameplay)
        {
            this.mTouchID = eventData.pointerId;
            this.OnPointerDown(this.GetPos(eventData.position));
        }
    }

    private void OnPointerDown(Vector3 pos)
    {
        if (this.disable || GameplayManager.Instance.State != GameState.Gameplay)
        {
            return;
        }
        //this.bDrag = false;
        if (OnDoubleClick != null)
        {
            OnDoubleClick();
        }
        else
        {
            this.touchdownpos = pos;
            this.touchdown = true;
            this.child.localPosition = pos;
            this.child.gameObject.SetActive(true);
            this.Origin = pos;
            this.DealDrag(this.Origin, true);
            if (On_JoyTouchStart != null)
            {
                On_JoyTouchStart(this.m_Data);
            }
        }
        //SetAlphaJoystick(Player.Instance.AlphaJoystick);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (this.disable || GameplayManager.Instance.State != GameState.Gameplay)
        {
            return;
        }
        if (this.mTouchID != eventData.pointerId)
        {
            return;
        }
        if (!this.touchdown)
        {
            this.child.gameObject.SetActive(true);
            Vector3 pos = this.GetPos(eventData.position);
            this.touchdownpos = pos;
            this.touchdown = true;
            this.child.localPosition = pos;
            this.Origin = pos;

            this.DealDrag(this.Origin, true);
            if (On_JoyTouchStart != null)
            {
                On_JoyTouchStart(this.m_Data);
            }

        }
        //this.bDrag = true;
        this.DealDrag(this.GetPos(eventData.position), true);
        if (On_JoyTouching != null)
        {
            On_JoyTouching(this.m_Data);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData == null)
        {
            return;
        }
        if (this.disable)
        {
            return;
        }
        if (this.mTouchID != eventData.pointerId)
        {
            return;
        }
        this.mTouchID = -1;
        this.touchdown = false;
        if (On_JoyTouchEnd != null)
        {
            this.touch.localPosition = Vector2.zero;
            On_JoyTouchEnd(this.m_Data);
        }
        if (this.child)
        {
            this.child.localPosition = this.StartPos;
            this.direction.localRotation = Quaternion.identity;
        }
        this.touch.localPosition = Vector2.zero;
        SetAlphaJoystick(alphaDefault);
    }

    private void DealDrag(Vector2 pos, bool updateui = true)
    {
        this.DealDrag_touchpos = pos - this.Origin;
        //float distance = Vector2.Distance(pos, this.Origin);

        //{
        //    if (distance > this.mRadius)
        //    {
        //        this.Origin += this.DealDrag_touchpos.normalized * (distance - this.mRadius);
        //        this.touchdownpos = this.Origin;
        //        this.child.localPosition = this.Origin;
        //    }

        //}
        //DebugCustom.LogColor("DealDrag_touchpos", DealDrag_touchpos);
        if (this.DealDrag_touchpos.magnitude > this.mRadius)
        {
            //this.Origin += this.DealDrag_touchpos.normalized * (this.DealDrag_touchpos.magnitude - this.mRadius);
            this.Origin = pos - DealDrag_touchpos.normalized * mRadius;
            this.child.localPosition = this.Origin;
            this.DealDrag_touchpos = this.DealDrag_touchpos.normalized * this.mRadius;
        }
        this.DealDrag_touchpos1 = this.DealDrag_touchpos;
        if (this.DealDrag_touchpos1.magnitude > this.mRadiusSmall)
        {
            this.DealDrag_touchpos1 = this.DealDrag_touchpos1.normalized * this.mRadiusSmall;
        }
        this.m_Data.length = this.DealDrag_touchpos.magnitude;
        this.m_Data.direction.x = this.DealDrag_touchpos.normalized.x;
        this.m_Data.direction.z = this.DealDrag_touchpos.normalized.y * speedDeltaDrag;/*2f;//1.23f;*/
        this.m_Data.angle = Helper.getAngle(this.m_Data.direction);
        //DebugCustom.Log(" Update Joytick 1", this.m_Data.direction);
        if (updateui)
        {
            this.touch.localPosition = this.DealDrag_touchpos1;
            this.direction.localRotation = Quaternion.Euler(0f, 0f, -this.m_Data.angle);
        }
    }
//#if UNITY_EDITOR
//    private int start = 0;
//    private void Update()
//    {
//        //float moveSpeed = 100;
//        float horizontalInput = Input.GetAxis("Horizontal");
//        float verticalInput = Input.GetAxis("Vertical");

//        if (Mathf.Abs(horizontalInput) >= 0.02f || Mathf.Abs(verticalInput) >= 0.02f)
//        {
//            if (start == 0)
//            {
//                start = 1;
//                if (On_JoyTouchStart != null)
//                {
//                    On_JoyTouchStart(this.m_Data);
//                }
//            }
//            Vector2 pos = new Vector2(horizontalInput, verticalInput);
//            pos = pos.normalized;

//            this.m_Data.length = 1;
//            this.m_Data.direction.x = pos.x;
//            this.m_Data.direction.z = pos.y * speedDeltaDrag;/*2f;//1.23f;*/
//            this.m_Data.angle = Helper.getAngle(this.m_Data.direction);
//            //DebugCustom.LogColorJson(" Update Joytick", m_Data.direction);
//            if (On_JoyTouching != null)
//            {
//                On_JoyTouching(this.m_Data);
//            }
//        }
//        else
//        {
//            if (start != 0)
//            {
//                start = 0;
//                if (On_JoyTouchEnd != null)
//                {
//                    On_JoyTouchEnd(this.m_Data);
//                }
//            }
//        }
//    }
//#else
//
//#endif
}
