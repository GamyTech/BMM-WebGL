using BestHTTP.WebSocket;
using GT.Backgammon;
using System.Collections.Generic;

namespace GT.Websocket
{
    public class GetMatchHistoryMovesAck : Ack
    {
        public ReplayMatchData MatchData { get; private set; }

        public GetMatchHistoryMovesAck(RequestId requestId, WebSocket webSocket, AckHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(requestId, webSocket, eventHandler, data, rawData)
        {
            MatchData = new ReplayMatchData(data);
        }
    }
}
