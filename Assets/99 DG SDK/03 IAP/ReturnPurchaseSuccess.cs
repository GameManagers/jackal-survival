
using UnityEngine.Purchasing;
public class ReturnPurchaseSuccess 
{
   public void ProcessPurchaseSuccess(Product product)
    {
        if (product.definition.id == "com.test")
        {
            Dragon.SDK.DebugCustom.LogColor("purchase test success");
        }
    }
}
