using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using WE.Manager;
using WE.Game;
using Cinemachine;
using DG.Tweening;
namespace WE.Manager
{
    public class ResolutionManager : MonoBehaviour
    {
        #region Field        

        public Camera mainCam;
        [SerializeField]
        private Transform canvasScreenSize;
        public MapLooper mapLooper;
        [SerializeField]
        private float perUnitSize = 100;
        [SerializeField]
        private Vector2 renderCanvasSize, tempRenderCanvasSize;
        [SerializeField, ReadOnly
                       ]
        private float screenWidth = 0f;
        [SerializeField, ReadOnly
            ]
        private float screenHeight = 0f;
        [SerializeField, ReadOnly
            ]
        private float canvasWidth = 0f;
        [SerializeField, ReadOnly
            ]
        private float canvasHeight = 0f;
        private float zoomAccuracy = 0.01f;
        private static ResolutionManager instance;
        #endregion
        #region Property
        public static ResolutionManager Instance { get { return instance; } }

        [SerializeField]
        private float screenLeftEdge = 0f;
        [SerializeField]
        private float screenRightEdge = 0f;
        [SerializeField]
        private float screenTopEdge = 0f;
        [SerializeField]
        private float screenBottomEdge = 0f;
        public Vector3 ScreenTopRight => screenTopRight;
        public Vector3 ScreenBottomLeft => screenBottomLeft;
        public float ScreenLeftEdge => screenLeftEdge;
        public float ScreenRightEdge => screenRightEdge;
        public float ScreenBottomEdge => screenBottomEdge;
        public float ScreenTopEdge => screenTopEdge;
        private Vector3 currentPosition, previousePosition;
        private Vector3 screenTopRight, screenBottomLeft;
        //Vector2 lastScreenSize = new Vector2(Screen.width, Screen.height);
        private float defaultCameraSize = 0f;
        public float ScreenHigh => screenTopEdge - screenBottomEdge;
        public float ScreenWidth => screenRightEdge - screenLeftEdge;

        [FoldoutGroup("Cam Zoom")]
        public CinemachineVirtualCamera virtualCamera;
        [FoldoutGroup("Cam Zoom")]
        public float highPanelZoom, uiPanelZoom;


        [FoldoutGroup("Cam Zoom")]
        public float zoomTime;


        public float lowPannelZoom => defaultCameraSize;
        public bool Zooming => zooming;
        bool zooming;
        Tween zoomTween;
        //[FoldoutGroup("Respawn Zone")]
        //public float offsetPos;
        //[FoldoutGroup("Respawn Zone")]
        //public BoxCollider2D leftBox, rightBox, topBox, bottomBox;
        public void GetEdge()
        {

            screenBottomLeft = mainCam.ViewportToWorldPoint(Vector2.zero);
            screenLeftEdge = screenBottomLeft.x;
            screenBottomEdge = screenBottomLeft.y;

            screenTopRight = mainCam.ViewportToWorldPoint(new Vector2(1, 1));
            screenRightEdge = screenTopRight.x;
            screenTopEdge = screenTopRight.y;

            //leftBox.transform.position = mainCam.ViewportToWorldPoint(new Vector2(0, 0.5f)) + new Vector3(-offsetPos, 0,0);
            //leftBox.size = new Vector2(1, ScreenHigh + offsetPos * 2 + 1);
            //rightBox.transform.position = mainCam.ViewportToWorldPoint(new Vector2(1, 0.5f)) + new Vector3(offsetPos, 0,0);
            //rightBox.size = new Vector2(1, ScreenHigh + offsetPos * 2 + 1);
            //bottomBox.transform.position = mainCam.ViewportToWorldPoint(new Vector2(0.5f, 0)) + new Vector3(0, -offsetPos,0);
            //bottomBox.size = new Vector2(ScreenWidth + offsetPos * 2, 1 + 1);
            //topBox.transform.position = mainCam.ViewportToWorldPoint(new Vector2(0.5f, 1)) + new Vector3(0,offsetPos,0);
            //topBox.size = new Vector2(ScreenWidth + offsetPos * 2, 1 + 1);
        }

        private void Awake()
        {
            //if (ScreenSizeHelper.Instance != null)
            //    ScreenSizeHelper.Instance.OnChangeScreenSize += Instance_OnChangeScreenSize;
            defaultCameraSize = mainCam.orthographicSize;
            //instance = this;
            currentPosition = previousePosition = transform.position;
            tempRenderCanvasSize = renderCanvasSize;
            RefreshResolution();
            zooming = false;
            //Debug.Log(render.bounds.size.x + " | " + render.bounds.size.y);
        }

        public void OnChangeScreenSize()
        {
            //renderCanvasSize = tempRenderCanvasSize;
            //mainCam.orthographicSize = defaultCameraSize;
            //lastScreenSize = screenSize;
            RefreshResolution();
            mapLooper.CheckEdge();
        }

        public void RefreshResolution()
        {
            if (instance == null)
                instance = this;
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            renderCanvasSize = new Vector2(renderCanvasSize.x / perUnitSize, renderCanvasSize.y / perUnitSize);
            canvasWidth = renderCanvasSize.x;
            canvasHeight = renderCanvasSize.y;
            //ZoomCameraToCanvasSize();
            GetEdge();
        }

        private void LateUpdate()
        {
            if (Time.frameCount % 5 == 0)
            {
                currentPosition = transform.position;
                if (currentPosition != previousePosition)
                {
                    previousePosition = currentPosition;
                    GetEdge();
                }


                //if (this.lastScreenSize != screenSize)
                //{
                //    renderCanvasSize = tempRenderCanvasSize;
                //    camera.orthographicSize = defaultCameraSize;
                //    lastScreenSize = screenSize;
                //    RefreshResolution();
                //}
            }
        }

        private void ZoomCameraToCanvasSize()
        {
            Vector2 canvasTopLeft = new Vector2(canvasScreenSize.position.x - renderCanvasSize.x / 2, canvasScreenSize.position.y - renderCanvasSize.y / 2);
            if (PointIsVisibleToCamera(canvasTopLeft, mainCam))
            {
                ZoomIn(canvasTopLeft, mainCam);
            }
            else
            {
                ZoomOut(canvasTopLeft, mainCam);
            }

        }
        void ZoomIn(Vector2 point, Camera cam)
        {
            while (PointIsVisibleToCamera(point, cam))
                cam.orthographicSize -= zoomAccuracy;
        }

        void ZoomOut(Vector2 point, Camera cam)
        {
            while (!PointIsVisibleToCamera(point, cam))
                cam.orthographicSize += zoomAccuracy;
        }
        bool PointIsVisibleToCamera(Vector2 point, Camera cam)
        {
            if (cam.WorldToViewportPoint(point).x < 0 || cam.WorldToViewportPoint(point).x > 1 || cam.WorldToViewportPoint(point).y > 1 || cam.WorldToViewportPoint(point).y < 0)
                return false;

            return true;
        }

        public bool IsPointInScreen(Vector2 point, float offsetX = 0, float offsetY = 0)
        {
            if (point.x > screenRightEdge + offsetX || point.x < screenLeftEdge - offsetX || point.y > screenTopEdge + offsetY || point.y < screenBottomEdge - offsetY)
                return false;

            return true;
        }

        public Vector2 ScreenCentre
        {
            // returns the x-coordinate that is the centre of the screen on the x axis regardless of where the camera is
            get
            {
                Vector2 zeroZero = new Vector2(0.5f, 0.5f);

                Vector2 zeroZeroToWorld = mainCam.ViewportToWorldPoint(zeroZero);


                return zeroZeroToWorld;
            }

        }
        #endregion

        public void ZoomOutHighPannel()
        {

            ZoomVirtualCam(highPanelZoom);
        }
        public void ZoomInGamePannel()
        {
            ZoomVirtualCam(defaultCameraSize);
        }
        public void ZoomInUiPanel()
        {
            ZoomVirtualCam(uiPanelZoom);
        }
        public void ZoomVirtualCam(float value)
        {
            if (zoomTween != null)
                zoomTween.Kill();
            zoomTween = DOTween.To(() => virtualCamera.m_Lens.OrthographicSize, x => virtualCamera.m_Lens.OrthographicSize = x, value, zoomTime).OnUpdate(() => {
                OnChangeScreenSize();
            }).OnComplete(() => {
                virtualCamera.m_Lens.OrthographicSize = value;
                OnChangeScreenSize();
                zooming = false;
            });
        }
    }
}