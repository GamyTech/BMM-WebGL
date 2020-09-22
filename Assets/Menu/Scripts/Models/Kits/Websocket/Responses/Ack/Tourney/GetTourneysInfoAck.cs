using BestHTTP.WebSocket;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class GetTourneysInfoAck : Ack
    {
        public List<Tourney> Tourneys { get; private set; }

        public GetTourneysInfoAck(RequestId requestId, WebSocket webSocket, AckHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(requestId, webSocket, eventHandler, data, rawData)
        {
            Init(rawData, data);
        }

        private void Init(string rawData, Dictionary<string, object> data)
        {
            object o;

            Tourneys = new List<Tourney>();
            if (data.TryGetValue("TourneysDetails", out o))
            {
                List<object> tourneys = o as List<object>;
                for (int i = 0; i < tourneys.Count; i++)
                {
                    Tourney tourney = new Tourney();
                    tourney.Update(tourneys[i] as Dictionary<string, object>);
                    Tourneys.Add(tourney);
                }
            }
        }
    }
}
