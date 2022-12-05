using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using WE.Manager;
using WE.Unit;
using WE.Support;
using WE.Effect;
using UnityEngine.Rendering;

namespace WE.Car
{
    public class CarVisualize : MonoBehaviour
    {
        public Transform gunBasePos;
        public EVehicleType vehicleType;
        public Animator gunAnim;

        public string AttackAnim = "Attack";
        public string IdleAnim = "Idle";

        public Transform firePosTransform;
        public AnimationEffect gunFx;
        public float fxScale = 1;

        public SpriteRenderer iconCar;
        public SpriteRenderer iconGun;
        public void CheckSorting()
        {
            int id = (int)(transform.position.y * 100);
            iconCar.sortingOrder = id;
            if (iconGun != null)
                iconGun.sortingOrder = id + 1;
        }
        public void ResetVisual()
        {
            gunAnim.transform.SetParent(gunBasePos);
            gunAnim.transform.localEulerAngles = Vector3.zero;
        }
        public void InitVisual()
        {
            gunAnim.transform.SetParent(null);
            SkillController.Instance.GunTransform = gunAnim.transform;
            SkillController.Instance.GunFirePos = firePosTransform;
        }
        public void HideVisual()
        {
            ResetVisual();
            gameObject.SetActive(false);
        }
        public void ShowVisual()
        {
            gameObject.SetActive(true);
            InitVisual();
        }
        public void UseWeapon(float timeAttack = 0)
        {
            if (gunAnim == null)
                return;
            gunAnim.transform.eulerAngles = new Vector3(0, 0, Helper.Get2DAngle(Player.Instance.Target - gunAnim.transform.position) - 90);
            gunAnim.Play(AttackAnim);
            if(timeAttack > 0)
                Observable.Timer(System.TimeSpan.FromSeconds(timeAttack)).Subscribe(_ => gunAnim.Play(IdleAnim)).AddTo(gameObject) ;
        }
        public void PlayFx()
        {
            if (gunFx != null)
            {
                AnimationEffect fx = Helper.SpawnEffect(gunFx, firePosTransform.position, firePosTransform);
                fx.transform.localEulerAngles = Vector3.zero;
                fx.transform.localScale = Vector3.one * fxScale;
            }
        }
        private float GetAngle(Vector2 tankPosition, Vector2 hitPosition)
        {
            Vector2 direction = (hitPosition - tankPosition).normalized;
            float getAngle_angle = 90 - Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return -getAngle_angle;
        }
        public void ReturnGun()
        {
            gunAnim.Play(IdleAnim);
            gunAnim.transform.localEulerAngles = Vector3.zero;
        }
    }
}

