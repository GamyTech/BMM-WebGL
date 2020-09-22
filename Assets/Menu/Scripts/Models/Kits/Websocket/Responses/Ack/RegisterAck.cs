using BestHTTP.WebSocket;
using System.Collections.Generic;
using GT.Database;

namespace GT.Websocket
{
    public class RegisterAck : Ack
    {
        private UserResponse m_userResponse;
        public UserResponse userResponse
        {
            get { return m_userResponse; }
        }


        public RegisterAck(RequestId requestId, WebSocket webSocket, AckHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(requestId, webSocket, eventHandler, data, rawData)
        {
            Init(rawData, data);
        }

        private void Init(string rawData, Dictionary<string, object> data)
        {
            m_userResponse = new UserResponse(rawData);
        }
    }
}
