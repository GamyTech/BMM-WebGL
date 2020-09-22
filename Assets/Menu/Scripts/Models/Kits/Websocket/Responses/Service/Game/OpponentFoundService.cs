using BestHTTP.WebSocket;
using UnityEngine;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class OpponentFoundService : Service
    {
        public int matchId { get; private set; }
        public float bet { get; private set; }
        public Enums.MatchKind MatchKind { get; private set; }


        public List<Dictionary<string, object>> users { get; private set; }

        public OpponentFoundService(ServiceId serviceId, WebSocket webSocket, ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(serviceId, webSocket, eventHandler, data, rawData)
        {
            Init(data);
        }

        private void Init(Dictionary<string, object> data)
        {
            object o;
            if (data.TryGetValue("TempMatchId", out o))
                matchId = o.ParseInt();
            else
                Debug.LogError("Missing TempMatchId in Dictionnary");

            if (data.TryGetValue("BetAmount", out o))
                bet = o.ParseFloat();

            Enums.MatchKind kind;
            if (data.TryGetValue("MatchKind", out o) && Utils.TryParseEnum(o, out kind))
                MatchKind = kind;
            else
                MatchKind = AppInformation.MATCH_KIND;

            users = new List<Dictionary<string, object>>();
            if (data.TryGetValue("Users", out o))
            {
                List<object> list = (List<object>)o;
                for (int i = 0; i < list.Count; i++)
                    users.Add((Dictionary<string, object>)list[i]);
            }
            else
                Debug.LogError("Missing Users in Dictionnary");

            UserController.Instance.SetPlayerFoundData(users);
        }
    }
}