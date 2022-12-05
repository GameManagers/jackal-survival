using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
namespace Dragon.SDK.IAP
{
    public class PurchaseItemData
    {
        private string priceDefault = "?";
        public string localizedPrice
        {
            get
            {
                if (metadata == null)
                    return priceDefault;
                else
                    return metadata.localizedPriceString;
            }
        }
        public ProductMetadata metadata { get; set; }
    }
    public class BaseIAP : IStoreListener
    {
        public Dictionary<string, PurchaseItemData> AndroidItems => _androidItems;

        private Dictionary<string, PurchaseItemData> _androidItems;
        private IStoreController _controller;
        private IAppleExtensions _appleExtensions;
        private IGooglePlayStoreExtensions _googlePlayStoreExtensions;
        private CrossPlatformValidator _validator = null;
        private ReturnPurchaseSuccess _returnPurchaseSuccess;
        public IEnumerator TryInit(List<string> productIDs)
        {
            _returnPurchaseSuccess = new ReturnPurchaseSuccess();
            yield return new WaitForSeconds(1);
            while (true)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    yield return new WaitForSeconds(1);
                }
                else
                {
                    if (IsInitialized())
                    {
                        yield break;
                    }
                    InitPurchase(productIDs);
                    yield return new WaitForSeconds(10);
                }
            }
        }
        public void Purchase(string productId)
        {
            if (IsInitialized())
            {
                Product product = _controller.products.WithID(productId);
                if (product != null && product.availableToPurchase)
                {
                    _controller.InitiatePurchase(product);
                }
                else
                {
                    Dragon.SDK.DebugCustom.LogError("Purchase product failed: product is not available for purchase");
                }
            }
            else
            {
                Dragon.SDK.DebugCustom.LogError("Init IAP Fail");
            }
        }
        public void RestorePurchase()
        {
            if (IsInitialized())
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer && _appleExtensions != null)
                {
                    _appleExtensions.RestoreTransactions(SDKDGManager.Instance.IAPManager.OnTransactionsRestored);
                }
            }
        }

        private void InitPurchase(List<string> productIDs)
        {

            var module = StandardPurchasingModule.Instance();

            var builder = ConfigurationBuilder.Instance(module);

            builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = false;
#if UNITY_IOS
    		string storeName = AppleAppStore.Name;
#elif UNITY_ANDROID
            string storeName = GooglePlay.Name;

#endif
            List<string> listID = productIDs;
            _androidItems = new Dictionary<string, PurchaseItemData>();
            for (int i = 0; i < listID.Count; i++)
            {
                _androidItems.Add(listID[i], new PurchaseItemData());
                string pID = listID[i];
                ProductType type = ProductType.Consumable;
                builder.AddProduct(pID, type, new IDs { { pID, storeName } });
            }
            UnityPurchasing.Initialize(this, builder);
        }
        private void OnDeferred(Product item)
        {
            Dragon.SDK.DebugCustom.Log("Purchase deferred: " + item.definition.id);
        }
        private bool IsInitialized()
        {
            return _controller != null;
        }
        #region Implement
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _controller = controller;
            _appleExtensions = extensions.GetExtension<IAppleExtensions>();
            _googlePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();

            // On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
            // On non-Apple platforms this will have no effect; OnDeferred will never be called.
            _appleExtensions.RegisterPurchaseDeferredListener(OnDeferred);
            Dictionary<string, string> introductory_info_dict = _appleExtensions.GetIntroductoryPriceDictionary();
            foreach (var item in controller.products.all)
            {
                if (item.availableToPurchase)
                {
                    if (_androidItems.ContainsKey(item.definition.id))
                    {
                        _androidItems[item.definition.id].metadata = item.metadata;
                    }
                }
            }
            SDKDGManager.Instance.IAPManager.ActionInitSuccess?.Invoke();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            //Debug.LogError($"In-App Purchasing initialize failed: {error}");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            //Debug.LogError($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            var product = purchaseEvent.purchasedProduct;
            var isPurchaseValid = IsPurchaseValid(product);
            if (isPurchaseValid)
            {
                _returnPurchaseSuccess.ProcessPurchaseSuccess(product);
                SDKDGManager.Instance.IAPManager.ActionPurchaseSuccess?.Invoke();
            }
            else
            {
                Dragon.SDK.DebugCustom.LogError("Invalid receipt, not unlocking content.");
            }
            return PurchaseProcessingResult.Complete;
        }
        #endregion

        #region Validator
        private void InitializeValidator()
        {
            if (IsCurrentStoreSupportedByValidator())
            {
                //#if !UNITY_EDITOR
                //_validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
                //#endif
            }
            else
            {
                var warningMsg = $"The cross-platform validator is not implemented for the currently selected store: {StandardPurchasingModule.Instance().appStore}. \n" +
                                "Build the project for Android, iOS, macOS, or tvOS and use the Google Play Store or Apple App Store. See README for more information.";
                Dragon.SDK.DebugCustom.LogError(warningMsg);
            }
        }
        private bool IsPurchaseValid(Product product)
        {
            //If we the validator doesn't support the current store, we assume the purchase is valid
            if (_validator != null)
            {
                if (IsCurrentStoreSupportedByValidator())
                {
                    try
                    {
                        var result = _validator.Validate(product.receipt);
                    }
                    //If the purchase is deemed invalid, the validator throws an IAPSecurityException.
                    catch (IAPSecurityException reason)
                    {
                        DebugCustom.Log($"Invalid receipt: {reason}");
                        return false;
                    }
                }
            }
            return true;
        }
        static bool IsCurrentStoreSupportedByValidator()
        {
            //The CrossPlatform validator only supports the GooglePlayStore and Apple's App Stores.
            return IsGooglePlayStoreSelected() || IsAppleAppStoreSelected();
        }
        static bool IsGooglePlayStoreSelected()
        {
            var currentAppStore = StandardPurchasingModule.Instance().appStore;
            return currentAppStore == AppStore.GooglePlay;
        }
        static bool IsAppleAppStoreSelected()
        {
            var currentAppStore = StandardPurchasingModule.Instance().appStore;
            return currentAppStore == AppStore.AppleAppStore ||
                   currentAppStore == AppStore.MacAppStore;
        }
        #endregion
    }
}
