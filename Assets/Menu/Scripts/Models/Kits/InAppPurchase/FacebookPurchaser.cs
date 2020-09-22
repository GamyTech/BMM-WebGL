using GT.Store;
using System;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;
#if !UNITY_STANDALONE
using Facebook.Unity;
#endif

namespace GT.InAppPurchase
{
    // Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
    public class FacebookPurchaser
    {
        public delegate void PurchaseFinished(InAppPurchaseResponse response);
        PurchaseFinished callback = null;
        public string productID;

        private static FacebookPurchaser m_instance;
        public static FacebookPurchaser Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new FacebookPurchaser();
                return m_instance;
            }
        }

        public void BuyProduct(string productBundleId, PurchaseFinished callback = null)
        {
            this.callback = callback;
            BuyProductID(productBundleId);
        }

        private void BuyProductID(string productId)
        {
#if !UNITY_STANDALONE
            this.productID = productId;
            FB.Canvas.PayWithProductId(productID, action: "purchaseiap", quantity: 1, callback: this.FacebookPurchaseCallBack);
#endif
        }

#if !UNITY_STANDALONE
        private void FacebookPurchaseCallBack(IPayResult result)
        {

            if (result != null)
            {
                Dictionary<string, object> messageDict = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
                object o;
                messageDict.TryGetValue("response", out o);
                Dictionary<string, object> response = o as Dictionary<string, object>;

                string failureReason = "";
                string payment_ID = "Payment ID can not be find in the facebook response";

                if (response.TryGetValue("error_message", out o))
                {
                    failureReason = o.ToString();
                    Debug.Log("failure reason : " + failureReason);
                    callback(new InAppPurchaseResponse(payment_ID, failureReason));
                }

                else if (response.TryGetValue("payment_id", out o))
                {
                    payment_ID = o.ToString();
                    Debug.Log("payment ID : " + payment_ID);

                    response.TryGetValue("purchase_token", out o);
                    string purchaseToken = o.ToString();
                    ConsumeProduct(payment_ID, purchaseToken);
                }
            }
    }
#endif


        public void ConsumeProduct(string payment_ID, string purchaseToken)
        {
#if !UNITY_STANDALONE
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("access_token", AccessToken.CurrentAccessToken.TokenString);
            string path = "/" + purchaseToken + "/consume";

            FB.API(path, HttpMethod.POST, r => { Debug.Log("consuming " + payment_ID + " : " + r.RawResult); callback(new InAppPurchaseResponse(payment_ID)); }, dic);
#endif
        }

        public static void ConsumeAllPendingProducts()
        {
#if !UNITY_STANDALONE
            FB.API("/app/purchases?access_token=" + AccessToken.CurrentAccessToken.TokenString, HttpMethod.GET, rp => {

                Debug.Log("pending list result : " + rp.RawResult);
                Dictionary<string, object> resultDic = Json.Deserialize(rp.RawResult) as Dictionary<string, object>;

                object o;
                resultDic.TryGetValue("data", out o);
                List<object> listOfProducts = (o as List<object>);

                if (listOfProducts.Capacity == 0)
                    return;

                foreach (object ProductObject in listOfProducts)
                {
                    Dictionary<string, object> Product = ProductObject as Dictionary<string, object>;

                    Product.TryGetValue("purchase_token", out o);
                    string purchaseToken = o.ToString();
                    Product.TryGetValue("product_id", out o);
                    string productId = o.ToString();

                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("access_token", AccessToken.CurrentAccessToken.TokenString);
                    FB.API(
                      "/" + purchaseToken + "/consume?access_token=" + AccessToken.CurrentAccessToken.TokenString,
                      HttpMethod.POST,
                      ri => { Debug.Log("Consuming " + productId + " result : " + ri.RawResult); }, // callback that receives a IGraphResult
                      dic
                    );
                }
            });
#endif
        }

    }

}
