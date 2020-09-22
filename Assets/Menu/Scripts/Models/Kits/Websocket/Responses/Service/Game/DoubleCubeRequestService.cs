using BestHTTP.WebSocket;
using GT.Exceptions;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class DoubleCubeRequestService : Service
    {
        public string SenderId { get; private set; }
        public string NextPlayerId { get; private set; }
        public float CurrentBet { get; private set; }
        public float CurrentFee { get; private set; }
        public int CurrentDouble { get; private set; }

        public DoubleCubeRequestService(ServiceId serviceId, WebSocket webSocket, ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(serviceId, webSocket, eventHandler, data, rawData)
        {
            Init(data);
        }

        private void Init(Dictionary<string, object> data)
        {
            object o;
            if (data.TryGetValue("Sender", out o))
                SenderId = o.ToString();
            else throw new MissingKeyException("Sender");

            if (data.TryGetValue("NextUser", out o))
                NextPlayerId = o.ToString();
            else throw new MissingKeyException("NextUser");

            if (data.TryGetValue("CurrentBet", out o))
                CurrentBet = o.ParseFloat();

            if (data.TryGetValue("CurrentFee", out o))
                CurrentFee = o.ParseFloat();

            CurrentDouble = 0;
            if (data.TryGetValue("CurrentDouble", out o))
                CurrentDouble = o.ParseInt();
        }
    }
}
