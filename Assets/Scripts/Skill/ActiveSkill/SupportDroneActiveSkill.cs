using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Bullet;
using WE.Support;
using WE.Unit;
using UniRx;
using Sirenix.OdinInspector;
using WE.Manager;

namespace WE.Skill
{
    public enum DroneType
    {
        A,
        B,
    }
    public class SupportDroneActiveSkill : ActiveSkill
    {
        public DroneType droneType;
        public float targetRotateSpeed = 60;
        public Transform targetRotation;
        public Transform target;         
        public Transform targetRotation2;
        public Transform target2;        
        public float TargetDistance = 2;
        public float targetRadius = 1;

        public float iconRotateSpeed = 90;
        public Transform iconRotation;
        public Transform iconFollow;
        public Transform Icon;
        public float baseIconDistance = 1;
        int rotateDir;
        public AudioClip Sfx;
        public override void Init()
        {
            targetRotation.gameObject.SetActive(true);
            targetRotation.SetParent(null);   
            iconRotation.gameObject.SetActive(true);
            Icon.SetParent(null);
            Icon.eulerAngles = Vector3.zero;
            target.localPosition = new Vector3(TargetDistance, 0, 0);
            if (IsEvoleSkill)
            {
                targetRotation2.gameObject.SetActive(true);
                targetRotation2.SetParent(null);

                target2.localPosition = new Vector3(-TargetDistance, 0, 0);
            }
            switch (droneType)
            {
                case DroneType.A:
                    rotateDir = 1;
                    break;
                case DroneType.B:
                    rotateDir = -1;
                    break;
                default:
                    break;
            }

            iconFollow.localPosition = new Vector3(baseIconDistance * rotateDir, 0, 0);

            base.Init();
        }
        public override void Dispose()
        {
            Icon.SetParent(iconRotation);
            targetRotation.SetParent(this.transform);
            targetRotation.gameObject.SetActive(false);
            if (targetRotation2 != null)
            {
                targetRotation2.SetParent(this.transform);
                targetRotation2.gameObject.SetActive(false);
            }
            iconRotation.gameObject.SetActive(false);
            base.Dispose();
        }
        private void LateUpdate()
        {
            targetRotation.position = this.transform.position;
            targetRotation.localEulerAngles = new Vector3(0, 0, targetRotation.localEulerAngles.z + Time.deltaTime*rotateDir * targetRotateSpeed * (1 + Player.Instance.BulletSpeedIncrease));

            if (targetRotation2 != null)
            {
                targetRotation2.position = this.transform.position;
                targetRotation2.localEulerAngles = new Vector3(0, 0, targetRotation2.localEulerAngles.z - Time.deltaTime * rotateDir * targetRotateSpeed * (1 + Player.Instance.BulletSpeedIncrease));
            }
            iconRotation.localEulerAngles = new Vector3(0, 0, iconRotation.localEulerAngles.z + Time.deltaTime * rotateDir * iconRotateSpeed);
            Icon.position = iconFollow.position;
        }
        protected override void ExcuteSkill()
        {
            StopMiniObserve();
            //target.gameObject.SetActive(true);
            target.transform.localScale = Vector3.one * targetRadius * BaseAreaScale;
            target.localPosition = new Vector3(TargetDistance * BaseAreaScale, 0, 0);
            if (IsEvoleSkill)
            {
                //target2.gameObject.SetActive(true);
                target2.transform.localScale = Vector3.one* targetRadius * BaseAreaScale;
                target2.localPosition = new Vector3(TargetDistance * BaseAreaScale, 0, 0);
            }
            miniDisposables = new CompositeDisposable();
            Observable.Timer(System.TimeSpan.FromSeconds(BaseDuration)).Subscribe(_ => StopMiniObserve()).AddTo(miniDisposables);
            Observable.Interval(System.TimeSpan.FromSeconds((float)1f / (BaseHitPerSec + BaseBulletNumb))).
                Subscribe(_ => ShotBullet()).AddTo(miniDisposables);
        }
        public void ShotBullet()
        {
            SoundManager.Instance.PlaySoundFx(Sfx);
            Vector3 pos = target.position + (Vector3)Random.insideUnitCircle * targetRadius * BaseAreaScale;
            DroneBullet bullet = Helper.SpawnBullet<DroneBullet>(bulletPrefabs, Icon.position, Quaternion.identity, null);
            bullet.tarPos = pos;
            InitBullet(bullet);
            if (IsEvoleSkill)
            {
                Vector3 _pos = target2.position + (Vector3)Random.insideUnitCircle * targetRadius * BaseAreaScale;
                DroneBullet _bullet = Helper.SpawnBullet<DroneBullet>(bulletPrefabs, Icon.position, Quaternion.identity, null);
                _bullet.tarPos = _pos;
                InitBullet(_bullet);
            }
        }
        protected override void StopMiniObserve()
        {
            target.gameObject.SetActive(false);
            if (IsEvoleSkill)
            {
                target2.gameObject.SetActive(false);

            }
            base.StopMiniObserve();
        }
    }
}

