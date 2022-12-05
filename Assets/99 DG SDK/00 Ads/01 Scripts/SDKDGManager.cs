using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dragon.SDK
{
    public class SDKDGManager : MonoBehaviour
    {
        public static SDKDGManager Instance => _instance;
        private static SDKDGManager _instance;

        [SerializeField] private AdsManager _adsManager;
        [SerializeField] private FirebaseManager _firebaseManager;
        [SerializeField] private AppFlyerManager _appFlyerManager;
        [SerializeField] private IAPManager _iapManager;

        public FirebaseManager FirebaseManager => _firebaseManager;
        public AdsManager AdsManager => _adsManager;
        public AppFlyerManager AppFlyerManager => _appFlyerManager;
        public IAPManager IAPManager => _iapManager;
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                InitInfo();
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        private void InitInfo()
        {
            _firebaseManager.OnInitSuccess = () =>
            {
                //TODO
            };
            _appFlyerManager.InitInfo();
            _adsManager.InitInfo();
            _firebaseManager.InitInfo();
            _iapManager.InitInfo();
        }
    }
}
