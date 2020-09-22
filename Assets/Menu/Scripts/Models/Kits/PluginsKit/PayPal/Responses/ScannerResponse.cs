using UnityEngine;
using System.Collections;

namespace GT.PayPal
{
    public enum ScannerResponseType
    {
        OK,
        Cancel,
        NotSupported,
    }

    public class ScannerResponse 
    {
        public ScannerResponseType responseType { get; protected set; }
        public string cardNumber { get; protected set; }
        public string expireMonth { get; protected set; }
        public string expireYear { get; protected set; }
        public string cvv { get; protected set; }

        public ScannerResponse(ScannerResponseType response)
        {
            responseType = response;
        }

        public ScannerResponse(ScannerResponseType response, string cardNumber, string expireMonth, string expireYear, string cvv) : this(response)
        {
            this.cardNumber = cardNumber;
            this.expireMonth = expireMonth.PadLeft(2, '0');
            if (expireYear.Length == 4)
            {
                expireYear = expireYear.Substring(2);
            }
            this.expireYear = expireYear;
            this.cvv = cvv;
        }

        public override string ToString()
        {
            return "[" + responseType + "] " + (responseType != ScannerResponseType.OK ? string.Empty : " [cardNumber:" + cardNumber +
                "], [expireMonth" + expireMonth + "], [expireYear" + expireYear + "], [cvv" + cvv + "]");
        }

    }
}
