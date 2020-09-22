using BestHTTP.WebSocket;
using GT.Exceptions;
using System.Collections.Generic;
using UnityEngine;

namespace GT.Websocket
{
    public class ChatReceivedService : Service
    {
        public string SenderId { get; private set; }
        public string Message { get; private set; }

        public ChatReceivedService(ServiceId serviceId, WebSocket webSocket, ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(serviceId, webSocket, eventHandler, data, rawData)
        {
            Debug.Log("ChatReceivedService : " + rawData);
            Init(data);
        }

        private void Init(Dictionary<string, object> data)
        {
            object o;

            if (data.TryGetValue("Message", out o))
                Message = o.ToString();
            else throw new MissingKeyException("Message");

            if (data.TryGetValue("Sender", out o))
                SenderId = o.ToString();
            else throw new MissingKeyException("Sender");

        }
    }
}
