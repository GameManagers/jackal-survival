using Dragon.SDK.IAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;


public class IAPManager : MonoBehaviour
{
    public Action ActionInitSuccess;
    public Action ActionPurchaseSuccess;
    public Action<bool> OnTransactionsRestored;
    [SerializeField] private List<string> _productIDs;
    private Dictionary<string, PurchaseItemData> _androidItems;
    private BaseIAP _baseIAP;

    public void InitInfo()
    {
        _baseIAP = new BaseIAP();
        StartCoroutine(_baseIAP.TryInit(_productIDs));
    }
    public void Purchase(string productId, Action actionPurchaseSuccess=null)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return;
        }
        ActionPurchaseSuccess = actionPurchaseSuccess;
        _baseIAP.Purchase(productId);
    }
    public void RestorePurchase()
    {
        _baseIAP.RestorePurchase();
    }
    public string GetPrice(string productId)
    {
        if (_baseIAP.AndroidItems != null && _baseIAP.AndroidItems.ContainsKey(productId))
        {
            if (_androidItems[productId].metadata != null)
                return _androidItems[productId].localizedPrice;
        }
        return "?";
    }

}
