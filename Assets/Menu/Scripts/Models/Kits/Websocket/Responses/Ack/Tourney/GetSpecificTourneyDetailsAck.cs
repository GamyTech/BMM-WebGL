using BestHTTP.WebSocket;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class GetSpecificTourneyDetailsAck : Ack
    {
        public GetSpecificTourneyDetailsAck(RequestId requestId, WebSocket webSocket, AckHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(requestId, webSocket, eventHandler, data, rawData)
        {
        }
    }
}
