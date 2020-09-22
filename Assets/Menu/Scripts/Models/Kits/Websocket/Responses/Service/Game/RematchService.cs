using BestHTTP.WebSocket;
using GT.Exceptions;
using System.Collections.Generic;
using UnityEngine;

namespace GT.Websocket
{
    public class RematchService : Service
    {
        private bool m_isRematch;
        public bool isRematch
        {
            get { return m_isRematch; }
        }

        private string m_senderId;
        public string senderId
        {
            get { return m_senderId; }
        }

        public RematchService(ServiceId serviceId, WebSocket webSocket, ServiceHandler eventHandler, Dictionary<string, object> data, string rawData, bool isRematch) :
            base(serviceId, webSocket, eventHandler, data, rawData)
        {
            m_isRematch = isRematch;
            Debug.Log(rawData);
            Init(data);
        }

        private void Init(Dictionary<string, object> data)
        {
            object o;
            if (data.TryGetValue("Sender", out o))
            {
                m_senderId = o.ToString();
            }
            else throw new MissingKeyException("Sender");
        }
    }
}