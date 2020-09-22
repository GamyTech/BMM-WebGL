using BestHTTP.WebSocket;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class UpdateTourneyDetailsService : Service
    {
        public string TourneyId { get; private set; }
        public Dictionary<string, object> TourneyData { get; private set; }

        public UpdateTourneyDetailsService(ServiceId serviceId, WebSocket webSocket,
            ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
                base(serviceId, webSocket, eventHandler, data, rawData)
        {
            TourneyData = data;

            object o;
            if (data.TryGetValue("TourneyId", out o))
                TourneyId = o.ToString();
        }
    }
}

