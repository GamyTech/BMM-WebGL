using BestHTTP.WebSocket;
using System.Collections.Generic;
using UnityEngine;


namespace GT.Websocket
{
    public class NotifyCashInSuccessService : Service
    {
        public float Cash { get; private set; }
        public float BonusCash { get; private set; }
        public float TotalCash { get; private set; }
        public bool IsFirstCashIn { get; private set; }
        public float NewTotalCash { get; private set; }

        public NotifyCashInSuccessService(ServiceId serviceId, WebSocket webSocket,
            ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
                base(serviceId, webSocket, eventHandler, data, rawData)
        {
            Init(data);
        }

        private void Init(Dictionary<string, object> data)
        {
            object o;
            if (data.TryGetValue("Cash", out o))
                Cash = o.ParseFloat();
            else
                Debug.Log("Cash is missing in dictionnary");

            if (data.TryGetValue("BonusCash", out o))
                BonusCash = o.ParseFloat();
            else
                Debug.Log("BonusCash is missing in dictionnary");

            TotalCash = Cash + BonusCash;

            if (data.TryGetValue("GetCashInCount", out o))
                IsFirstCashIn = o.ParseInt() == 1;
            else
                Debug.Log("GetCashInCount is missing in dictionnary");

            if (data.TryGetValue("Wallet", out o))
            {
                Dictionary<string, object> wallet = o as Dictionary<string, object>;
                if (wallet.TryGetValue("Cash", out o))
                    NewTotalCash += o.ParseFloat();
                else
                    Debug.Log("Cash is missing in Wallet dictionnary");

                if (wallet.TryGetValue("BonusCash", out o))
                    NewTotalCash += o.ParseFloat();
                else
                    Debug.Log("BonusCash is missing in Wallet dictionnary");
            }
            else
                Debug.Log("Wallet is missing in dictionnary");

        }
    }
}

