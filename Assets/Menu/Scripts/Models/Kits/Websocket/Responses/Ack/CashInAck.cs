using GT.Database;
using BestHTTP.WebSocket;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class CashInAck : Ack
    {
        public IServerResponse response { get; private set; }
        public string VerifyUrl { get; private set; }


        public CashInAck(RequestId requestId, WebSocket webSocket, AckHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(requestId, webSocket, eventHandler, data, rawData)
        {
            Init(rawData, data);
        }

        private void Init(string rawData, Dictionary<string, object> data)
        {
            response = new ResponseBase(rawData);

            object o;
            if(data.TryGetValue("VerifyURL", out o))
                VerifyUrl = o.ToString();
        }
    }
}

