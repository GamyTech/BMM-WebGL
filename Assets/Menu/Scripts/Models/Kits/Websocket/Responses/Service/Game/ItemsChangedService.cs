using BestHTTP.WebSocket;
using System.Collections.Generic;
using UnityEngine;

namespace GT.Websocket
{
    public class ItemsChangedService : Service
    {
        private Dictionary<string, object> changesByUser;

        public ItemsChangedService(ServiceId serviceId, WebSocket webSocket, ServiceHandler eventHandler, 
            Dictionary<string, object> data, string rawData) :
            base(serviceId, webSocket, eventHandler, data, rawData)
        {
            Init(data);
        }

        private void Init(Dictionary<string, object> data)
        {
            object o;
            if (data.TryGetValue("RoomItems", out o))
            {                
                changesByUser = o as Dictionary<string, object>;
            }
        }

        public Dictionary<string, object> GetChangesByUser()
        {
            return changesByUser;
        }
    }
}