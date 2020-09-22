using BestHTTP.WebSocket;
using GT.Exceptions;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class BankTimeService : Service
    {
        private float m_bankTime;
        public float bankTime
        {
            get { return m_bankTime; }
        }


        public BankTimeService(ServiceId serviceId, WebSocket webSocket, ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(serviceId, webSocket, eventHandler, data, rawData)
        {
            Init(data);
        }

        private void Init(Dictionary<string, object> data)
        {
            object o;
            if (data.TryGetValue("BankTime", out o))
            {
                m_bankTime = o.ParseFloat();
            }
            else throw new MissingKeyException("BankTime");
        }
    }
}
