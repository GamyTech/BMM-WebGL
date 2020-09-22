using BestHTTP.WebSocket;
using System.Collections.Generic;
using UnityEngine;

namespace GT.Websocket
{
    public class PlayerRolledService : Service
    {
        public string senderId { get; private set; }
        public Dictionary<string, string> RollingItemIds { get; private set; }

        public PlayerRolledService(ServiceId serviceId, WebSocket webSocket, ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(serviceId, webSocket, eventHandler, data, rawData)
        {
            Init(data);
        }

        private void Init(Dictionary<string, object> data)
        {
            object o;

            if (data.TryGetValue("SenderId", out o))
                senderId = o.ToString();
            else
                Debug.LogError("Missing SenderId in Dictionnary");
            
            RollingItemIds = new Dictionary<string, string>();
            if (data.TryGetValue("Data", out o) && o != null)
            {
                Dictionary<string, object> items = MiniJSON.Json.Deserialize(o.ToString()) as Dictionary<string, object>;
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        Enums.StoreType type;
                        if (item.Value != null && Utils.TryParseEnum(item.Key, out type, true))
                        {
                            RollingItemIds.Add(type.ToString(), item.Value.ToString());
                        }
                    }
                }
            }
        }
    }
}
