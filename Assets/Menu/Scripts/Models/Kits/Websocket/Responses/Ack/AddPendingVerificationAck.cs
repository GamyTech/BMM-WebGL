using GT.Database;
using BestHTTP.WebSocket;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class AddPendingVerificationAck : Ack
    {
        private string m_error;
        public string Error
        {
            get { return m_error; }
        }

        public AddPendingVerificationAck(RequestId requestId, WebSocket webSocket, AckHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(requestId, webSocket, eventHandler, data, rawData)
        {
            Init(rawData, data);
        }

        private void Init(string rawData, Dictionary<string, object> data)
        {
            m_error = rawData;
            UnityEngine.Debug.Log("No way!");
        }
    }
}
