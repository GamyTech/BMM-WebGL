using BestHTTP.WebSocket;
using System.Collections.Generic;
using UnityEngine;

namespace GT.Websocket
{
    public class GameStoppedService : Service
    {
        public string WinnerId { get; private set; }
        public string RoomId { get; private set; }
        public int Loyalty { get; private set; }
        public bool IsCanceled { get; private set; }
        public int MatchId { get; private set; }

        public string FirstUserId { get; private set; }
        public int FirstPoints { get; private set; }
        public string SecondUserId { get; private set; }
        public int SecondPoints { get; private set; }

        public GameStoppedService(ServiceId serviceId, WebSocket webSocket, ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(serviceId, webSocket, eventHandler, data, rawData)
        {
            Init(data);
        }

        private void Init(Dictionary<string, object> data)
        {
            object o;
            if (data.TryGetValue("Winner", out o))
                WinnerId = o.ToString();
            else Debug.LogError("Winner");

            if (data.TryGetValue("RoomId", out o))
                RoomId = o.ToString();
            else Debug.LogError("RoomId");

            if (data.TryGetValue("MatchLoyalty", out o))
                Loyalty = o.ParseInt();

            if (data.TryGetValue("MatchId", out o))
                MatchId = o.ParseInt();

            if (data.TryGetValue("IsCanceled", out o))
                IsCanceled = o.ParseBool();

            if (data.TryGetValue("FirstUserId", out o))
                FirstUserId = o.ToString();
            if (data.TryGetValue("FirstPoints", out o))
                FirstPoints = o.ParseInt();

            if (data.TryGetValue("SecondUserId", out o))
                SecondUserId = o.ToString();
            if (data.TryGetValue("SecondPoints", out o))
                SecondPoints = o.ParseInt();

            if (!string.IsNullOrEmpty(FirstUserId) && !string.IsNullOrEmpty(SecondUserId) && 
                WinnerId != FirstUserId && WinnerId != SecondUserId)
                IsCanceled = true;
        }
    }
}
