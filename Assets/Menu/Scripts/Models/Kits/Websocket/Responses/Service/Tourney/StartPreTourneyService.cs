using BestHTTP.WebSocket;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class StartPreTourneyService : Service
    {
        public OngoingTourneyDetails PreTourney { get; private set; }

        public StartPreTourneyService(ServiceId serviceId, WebSocket webSocket,
            ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
                base(serviceId, webSocket, eventHandler, data, rawData)
        {
            Init(data);
        }

        private void Init(Dictionary<string, object> data)
        {
            PreTourney = new OngoingTourneyDetails(data, true);
        }
    }
}
