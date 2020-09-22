using BestHTTP.WebSocket;
using System.Collections.Generic;
using UnityEngine;

namespace GT.Websocket
{
    public class RegisterTourneyAck : Ack
    {
        public bool IsSuccess { get; private set; }
        public string TourneyId { get; private set; }
        public Dictionary<string, object> Wallet { get; private set; }

        public RegisterTourneyAck(RequestId requestId, WebSocket webSocket, AckHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(requestId, webSocket, eventHandler, data, rawData)
        {
            Init(rawData, data);
        }

        private void Init(string rawData, Dictionary<string, object> data)
        {
            object o;
            IsSuccess = false;
            if (data.TryGetValue("IsSuccess", out o))
                IsSuccess = o.ParseBool();
            if (data.TryGetValue("Id", out o))
                TourneyId = o.ToString();
            if (data.TryGetValue("Wallet", out o))
                Wallet = o as Dictionary<string, object>;
        }
    }
}
