using BestHTTP.WebSocket;
using GT.Exceptions;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class FriendInviteService : Service
    {
        public string senderId { get; private set; }
        public List<float> bets { get; private set; }
        public string name { get; private set; }
        public string picUr { get; private set; }

        public FriendInviteService(ServiceId serviceId, WebSocket webSocket, ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(serviceId, webSocket, eventHandler, data, rawData)
        {
            Init(data);
        }

        private void Init(Dictionary<string, object> data)
        {
            object o;
            if (data.TryGetValue("Sender", out o))
            {
                senderId = o.ToString();
            }
            else throw new MissingKeyException("Sender");

            if (data.TryGetValue("Bet", out o))
            {
                bets = new List<float>();
                string[] betStrings = o.ToString().Split(',');
                for (int i = 0; i < betStrings.Length; i++)
                {
                    bets.Add(betStrings[i].ParseFloat());
                }
            }
            else throw new MissingKeyException("Bet");

            if(data.TryGetValue("FacebookFriend", out o))
            {
                Dictionary<string, object> dict = (Dictionary<string, object>)o;
                if (dict.TryGetValue("UserName", out o))
                {
                    name = o.ToString();
                }

                if (dict.TryGetValue("PictureUrl", out o))
                {
                    picUr = o.ToString();
                }
            }
        }
    }
}
