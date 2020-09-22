using BestHTTP.WebSocket;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class Ack : WebSocketMessage
    {
        private AckHandler ackEventHandler;
        public RequestId RequestId { get; private set; }

        public Ack(RequestId requestId, WebSocket webSocket, AckHandler eventHandler, Dictionary<string, object> data, string rawData) : base(webSocket, data, rawData)
        {
            RequestId = requestId;
            ackEventHandler = eventHandler;
        }

        public override void TriggerEvent()
        {
            if (ackEventHandler != null)
                ackEventHandler(this);
        }
    }
}
