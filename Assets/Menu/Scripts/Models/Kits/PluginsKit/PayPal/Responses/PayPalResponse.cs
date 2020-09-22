using UnityEngine;
using System.Collections;

namespace GT.PayPal
{
    public enum PayPalResponseType
    {
        OK,
        Cancel,
        NotSupported,
    }

    public class PayPalResponse
    {
        public PayPalResponseType responseType { get; protected set; }
        public string email { get; protected set; }
        public string price { get; protected set; }
        public string paymentID { get; protected set; }
        public string date { get; protected set; }
        public string state { get; protected set; }
        public string adress { get; protected set; }


        public PayPalResponse(PayPalResponseType response)
        {
            responseType = response;
        }

        public PayPalResponse(PayPalResponseType response, string adress)
        {
            responseType = response;
            this.adress = adress;
        }

        public PayPalResponse(PayPalResponseType response, string email, string price, string paymentID, string date, string state) : this(response)
        {
            responseType = PayPalResponseType.OK;
            this.email = email;
            this.price = price;
            this.paymentID = paymentID;
            this.date = date;
            this.state = state;
        }

        public override string ToString()
        {
            return "[" + responseType + "] " + (responseType != PayPalResponseType.OK ? string.Empty : " [email:" + email +
                "], [price" + price + "], [paymentID" + paymentID + "], [date" + date + "], [state" + state + "]");
        }
    }
}
