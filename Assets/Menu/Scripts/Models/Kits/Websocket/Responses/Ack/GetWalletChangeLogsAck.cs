using BestHTTP.WebSocket;
using GT.Backgammon;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class GetWalletChangeLogsAck : Ack
    {
        public GetWalletChangeLogsAck(RequestId requestId, WebSocket webSocket, AckHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(requestId, webSocket, eventHandler, data, rawData)
        {
            if (UserController.Instance.gtUser != null)
                UserController.Instance.gtUser.AddTransactionHistory(data);
        }
    }
}
