using BestHTTP.WebSocket;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class CloseTourneyService : Service
    {
        public Tourney Tourney { get; private set; }
        public OngoingTourneyDetails TourneyDetails { get; private set; }

        public CloseTourneyService(ServiceId serviceId, WebSocket webSocket,
            ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
                base(serviceId, webSocket, eventHandler, data, rawData)
        {
        }
    }
}
