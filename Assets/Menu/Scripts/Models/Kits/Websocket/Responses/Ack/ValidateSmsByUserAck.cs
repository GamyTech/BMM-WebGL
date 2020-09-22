using BestHTTP.WebSocket;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class ValidateSmsByUserAck : Ack
    {
        public IServerResponse response { get; private set; }
        public bool Result { get; private set; }

        public ValidateSmsByUserAck(RequestId requestId, WebSocket webSocket, AckHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(requestId, webSocket, eventHandler, data, rawData)
        {
            Init(rawData, data);
        }

        private void Init(string rawData, Dictionary<string, object> data)
        {
            response = new ResponseBase(rawData);

            object o;
            
            if (data.TryGetValue("Result", out o))
                Result = o.ParseBool();
        }
    }
}
