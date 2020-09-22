using BestHTTP.WebSocket;
using System.Collections.Generic;
using MiniJSON;

namespace GT.Websocket
{
    public class SyncAck : Ack
    {
        public IServerResponse response { get; private set; }
        public bool NeedToUpdate { get; private set; }
        public int MoveId { get; private set; }
        public string CurrentTurnPlayerID { get; private set; }
        public float CurrentTurnTime { get; private set; }
        public Backgammon.Player.PlayerColor CurrentPlayerColor { get; private set; }
        public string CurrentDice { get; private set; }
        public string WhoCantDouble { get; private set; }
        public bool HaveBankTimeStarted { get; private set; }
        public int AmountsOfDoubles { get; private set; }
        public string Move { get; private set; }
        public string NewBoard { get; private set; }
        public bool IsDoubleCube { get; private set; }


        public SyncAck(RequestId requestId, WebSocket webSocket, AckHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(requestId, webSocket, eventHandler, data, rawData)
        {
            Init(rawData, data);
        }

        private void Init(string rawData, Dictionary<string, object> data)
        {
            //if(TestingController.Instance.TestBox)
            //{
            //    rawData = TestingController.Instance.testString;
            //    data = Json.Deserialize(TestingController.Instance.testString) as Dictionary<string, object>;
            //}

            response = new ResponseBase(rawData);

            object o;
            if (data.TryGetValue("NeedToUpdate", out o))
                NeedToUpdate = o.ParseBool();

            if (data.TryGetValue("MoveId", out o))
                MoveId = o.ParseInt();

            if (data.TryGetValue("NewTurn", out o))
            {
                Dictionary<string, object> newTurnDict = (Dictionary<string, object>)Json.Deserialize(o.ToString());

                if (newTurnDict.TryGetValue("Dice", out o))
                    CurrentDice = o.ToString();

                if (newTurnDict.TryGetValue("CurrentTurn", out o))
                    CurrentTurnPlayerID = o.ToString();

                Backgammon.Player.PlayerColor currentTurnPlayerColor;
                if (newTurnDict.TryGetValue("Color", out o) && Utils.TryParseEnum(o, out currentTurnPlayerColor))
                    CurrentPlayerColor = currentTurnPlayerColor;
            }

            if (data.TryGetValue("CurrentTurnTime", out o))
                CurrentTurnTime = o.ParseFloat();

            if (data.TryGetValue("WhoCantDouble", out o))
                WhoCantDouble = o.ToString();

            if (data.TryGetValue("HaveBankTimeStarted", out o))
                HaveBankTimeStarted = o.ParseBool();

            if (data.TryGetValue("AmountsOfDoubles", out o))
                AmountsOfDoubles = o.ParseInt();

            if (data.TryGetValue("Move", out o))
                Move = o.ToString();

            if (data.TryGetValue("NewBoard", out o))
                NewBoard = o.ToString();

            if (data.TryGetValue("IsDoubleCube", out o))
                IsDoubleCube = o.ParseBool();
        }
    }
}

