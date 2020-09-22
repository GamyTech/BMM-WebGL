using BestHTTP.WebSocket;
using System.Collections.Generic;
using UnityEngine;


namespace GT.Websocket
{
    public class NotifyCashInFailedService : Service
    {
        public string Reason { get; private set; }
        public bool Canceled { get; private set; }

        public NotifyCashInFailedService(ServiceId serviceId, WebSocket webSocket, ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
                base(serviceId, webSocket, eventHandler, data, rawData)
        {
            Init(data);
        }

        private void Init(Dictionary<string, object> data)
        {
            object o;
            if (data.TryGetValue("Reason", out o))
                Reason = o.ToString();

            Canceled = Reason.Equals("Canceled");
        }
    }
}

