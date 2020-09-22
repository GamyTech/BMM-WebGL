using BestHTTP.WebSocket;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class StartTourneyService : Service
    {
        public StartTourneyService(ServiceId serviceId, WebSocket webSocket,
            ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
                base(serviceId, webSocket, eventHandler, data, rawData)
        {
        }
    }
}
