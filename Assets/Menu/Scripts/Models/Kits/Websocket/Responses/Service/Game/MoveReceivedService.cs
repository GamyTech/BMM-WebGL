using BestHTTP.WebSocket;
using System.Collections.Generic;
using UnityEngine;

namespace GT.Websocket
{
    public class MoveReceivedService : Service
    {
        public int TurnIndex { get; private set; }
        public string SerializedMove { get; private set; }
        public string NextDice { get; private set; }
        public bool IsRolled { get; private set; }
        public string CantDoubleId { get; private set; }
        public bool DoubleResponse { get; private set; }
        public string NextTurnPlayer { get; private set; }
        public float CurrentBet { get; private set; }
        public float CurrentFee { get; private set; }
        public int TotalTime { get; private set; }
        public int CurrentDoubleAmount { get; private set; }

        public MoveReceivedService(ServiceId serviceId, WebSocket webSocket, ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(serviceId, webSocket, eventHandler, data, rawData)
        {
            Init(data);
        }

        private void Init(Dictionary<string, object> data)
        {
            object o;

            if (data.TryGetValue("MoveId", out o))
                TurnIndex = o.ParseInt();
            else
                Debug.LogError("Missing MoveId in Dictionnary");

            if (data.TryGetValue("MoveData", out o))
                SerializedMove = o.ToString();
            else
                Debug.LogError("Missing MoveData in Dictionnary");

            if (data.TryGetValue("Dices", out o))
                NextDice = o.ToString();
            else
                Debug.LogError("Missing Dices in Dictionnary");

            if (data.TryGetValue("IsRoll", out o))
                IsRolled = o.ParseBool();

            if (data.TryGetValue("WhoCantDouble", out o))
                CantDoubleId = o.ToString();
            else
                Debug.LogError("Missing WhoCantDouble in Dictionnary");

            if (data.TryGetValue("NextUser", out o))
                NextTurnPlayer = o.ToString();
            else
                Debug.LogError("Missing NextUser in Dictionnary");

            if (data.TryGetValue("CurrentBet", out o))
            {
                DoubleResponse = true;
                CurrentBet = o.ParseFloat();
            }

            if (data.TryGetValue("CurrentFee", out o))
                CurrentFee = o.ParseFloat();

            if (data.TryGetValue("CurrentDoubleAmount", out o))
            {
                DoubleResponse = true;
                CurrentDoubleAmount = o.ParseInt();
            }

            if (data.TryGetValue("TotalTime", out o))
                TotalTime = o.ParseInt();
        }
    }
}
