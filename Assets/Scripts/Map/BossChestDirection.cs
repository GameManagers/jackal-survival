using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Pooling;
using WE.Manager;
using WE.Support;
using WE.Unit;
using UnityEngine.UI;

namespace WE.Game
{
    public class BossChestDirection : PoolingObject
    {
        public float offsetX, offsetY;
        float offsetPlayer;
        public Transform direction;
        public Transform icon;
        public RectTransform directionTransform;
        ItemInGame bossChest;
        public void InitItem(ItemInGame _bossChest)
        {
            bossChest = _bossChest;
            bossChest.OnDespawn += OnBossChestDespawn;
            offsetPlayer = ResolutionManager.Instance.ScreenWidth / 2 - 1;
        }
        private void LateUpdate()
        {
            if (IsChestInScreen())
            {
                icon.gameObject.SetActive(false);
            }
            else
            {
                SetTransform();
            }
        }
        public bool IsChestInScreen()
        {
            //ResolutionManager.Instance.GetEdge();
            Vector3 pos = bossChest.transform.position;
            return pos.x > ResolutionManager.Instance.ScreenLeftEdge -offsetX&& pos.x < ResolutionManager.Instance.ScreenRightEdge + offsetX
                && pos.y < ResolutionManager.Instance.ScreenTopEdge - offsetY && pos.y > ResolutionManager.Instance.ScreenBottomEdge + offsetY;
        }
        public void SetTransform()
        {
            icon.gameObject.SetActive(true);
            gameObject.transform.SetParent(Player.Instance.transform);
            //Vector3 pos = bossChest.transform.position;
            ////pos.x = Mathf.Clamp(pos.x, ResolutionManager.Instance.ScreenLeftEdge + offsetLeft, ResolutionManager.Instance.ScreenRightEdge - offsetRight);
            ////pos.y = Mathf.Clamp(pos.y, ResolutionManager.Instance.ScreenBottomEdge + offsetBottom, ResolutionManager.Instance.ScreenTopEdge - offsetTop);
            ////transform.position = pos;
            ////direction.transform.localEulerAngles = new Vector3(0, 0, Helper.Get2DAngle( bossChest.transform.position - pos));
            ////float screenLeft = ResolutionManager.Instance.ScreenLeftEdge;
            ////float screenRight = ResolutionManager.Instance.ScreenRightEdge;
            ////float screenTop = ResolutionManager.Instance.ScreenTopEdge;
            ////float screenBottom = ResolutionManager.Instance.ScreenBottomEdge;
            ////float screenWidth = screenRight - screenLeft;
            ////float screenHeight = screenTop - screenBottom;
            //float anglex = Mathf.Atan(screenHeight / screenWidth) * Mathf.Rad2Deg;
            //float sinx = Helper.Sin(anglex);
            //float cosx = Helper.Cos(anglex);
            Vector3 pos = Vector3.zero;
            Vector3 chestPos = bossChest.transform.position;
            Vector3 pPos = Player.Instance.transform.position;
            Vector3 dir = chestPos - pPos;
            float angle = Helper.Get2DAngle(dir);
            pos = Vector3.zero;   
            float screenx = ResolutionManager.Instance.ScreenWidth / 2 - offsetX;
            float screenY = ResolutionManager.Instance.ScreenHigh / 2 - offsetY;
            pos.x = screenx * Helper.Cos(angle);
            pos.y = screenY * Helper.Sin(angle);
            transform.localPosition = pos;
            direction.eulerAngles = new Vector3(0,0, angle);
            ////if (Helper.Cos(angle) >= cosx)
            ////{
            ////    pos.x = screenRight;
            ////    pos.y = (1 + Helper.Sin(angle)) / 2 * screenHeight + screenBottom;
            ////}
            ////else if (Helper.Cos(angle) <= -cosx)
            ////{
            ////    pos.x = screenLeft;
            ////    pos.y = (1 + Helper.Sin(angle)) / 2 * screenHeight + screenBottom;

            ////}
            ////else if (Helper.Sin(angle) >= sinx)
            ////{
            ////    pos.y = screenTop;
            ////    pos.x = (1 +Helper.Cos(angle)) / 2 * screenWidth + screenLeft;
            ////}
            ////else
            ////{
            ////    pos.y = screenBottom;
            ////    pos.x = (1 + Helper.Cos(angle)) / 2 * screenWidth + screenLeft;
            ////}
            //transform.position = pos;
            //direction.localEulerAngles = new Vector3(0, 0, angle);
        }
        public void OnBossChestDespawn(ItemInGame e)
        {
            Despawn();
        }
        public override void Despawn()
        {
            bossChest.OnDespawn -= OnBossChestDespawn;
            base.Despawn();
        }
    }
}

