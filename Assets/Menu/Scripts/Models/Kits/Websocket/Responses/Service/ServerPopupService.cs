using BestHTTP.WebSocket;
using System.Collections.Generic;
using UnityEngine;


namespace GT.Websocket
{
    public class ServerPopupService : Service
    {
        public ServerPopupService(ServiceId serviceId, WebSocket webSocket,
            ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
                base(serviceId, webSocket, eventHandler, data, rawData)
        {
            Init(data);
        }

        private void Init(Dictionary<string, object> data)
        {
            if(PopupController.Instance != null)
                PopupController.Instance.ShowPopup(data);
            Debug.LogError("PopupController not instanciated");
        }
    }
}

