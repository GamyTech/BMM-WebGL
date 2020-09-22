using BestHTTP.WebSocket;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class LoginAck : Ack
    {
        public UserResponse UserResponse { get; private set; }
        public OngoingTourneyDetails OngoingTourney { get; private set; }

        public bool IsReconnect { get; private set; }
        public ReconnectData ReconnectData { get; private set; }

        public LoginAck(RequestId requestId, WebSocket webSocket, AckHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(requestId, webSocket, eventHandler, data, rawData)
        {
            Init(rawData, data);
        }

        private void Init(string rawData, Dictionary<string, object> data)
        {
            object o;

            OngoingTourney = null;
            if (data.TryGetValue("TourneyResult", out o))
                OngoingTourney = new OngoingTourneyDetails((Dictionary<string, object>)o, false, true);
            if (data.TryGetValue("Tourney", out o))
                OngoingTourney = new OngoingTourneyDetails((Dictionary<string, object>)o);

            UserResponse = new UserResponse(rawData);

            IsReconnect = data.TryGetValue("ReconnectData", out o);
            if (!IsReconnect)
                IsReconnect = data.TryGetValue("TourneyReconnectData", out o);
            if (IsReconnect)
                ReconnectData = new ReconnectData((Dictionary<string, object>)o);

            if (data.TryGetValue(ServiceId.MessageEvents.ToString(), out o))
                PopupController.Instance.ShowPopupFromData(o);
        }
    }
}
