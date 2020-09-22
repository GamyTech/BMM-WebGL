
using UnityEngine.Purchasing;

namespace GT.InAppPurchase
{
    public class InAppPurchaseResponse
    {
        public ResponseType responseType;
        public string failureReason;
        public Product product = null;
        public string transactionID;

        public InAppPurchaseResponse(Product product, ResponseType type, string failureReason)
        {
            responseType = type;
            this.failureReason = failureReason;
        }

        public InAppPurchaseResponse(Product product)
        {
            ResponseType type = ResponseType.OK;
            responseType = type;
            this.product = product;
        }

        public InAppPurchaseResponse(string productID, string failureReason = "")
        {

            if (string.IsNullOrEmpty(failureReason))
            {
                responseType = ResponseType.OK;
                this.transactionID = productID;
            }
            else
            {
                responseType = ResponseType.Error;
                this.failureReason = failureReason;
            }
        }

        public override string ToString()
        {
            string transaction = product != null ? product.transactionID : transactionID;
            return string.IsNullOrEmpty(failureReason) ? "Purchase successfull : " + transaction : "Purchase error : " + failureReason;
        }
    }

}
