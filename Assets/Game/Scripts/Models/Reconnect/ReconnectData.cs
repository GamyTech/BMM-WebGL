using System.Collections.Generic;
using GT.Backgammon.Player;
using MiniJSON;

namespace GT.Websocket
{
    public class ReconnectData
    {
        public int MatchId { get; private set; }
        public string CurrentTurnPlayer { get; private set; }
        public string CurrentDice { get; private set; }
        public string NewBoard { get; private set; }
        public int MoveId { get; private set; }
        public bool IsRolled { get; private set; }
        public string lastMove { get; private set; }

        public Dictionary<string, PlayerData> Players;

        // bet data
        public int DoublesCount { get; private set; }
        public string CantDoublePlayer { get; private set; }
        public bool IsRequestingDouble { get; private set; }
        public bool canDoubleAgain { get; private set; }
        public Enums.MatchKind Kind { get; private set; }
        public float Bet { get; private set; }
        public float Fee { get; private set; }
        public float MaxBet { get; private set; }
        public int MaxDoubleAmount { get; private set; }

        public ReconnectData(Dictionary<string, object> reconnectDataDict)
        {
            HandleGame(reconnectDataDict);
            HandlePlayers(reconnectDataDict);
            HandleBet(reconnectDataDict);
        }

        private void HandleGame(Dictionary<string, object> dataDict)
        {
            object o;
            if (dataDict.TryGetValue("NewTurn", out o))
            {
                Dictionary<string, object> newTurnDict = (Dictionary<string, object>)Json.Deserialize(o.ToString());

                if (newTurnDict.TryGetValue("Dice", out o))
                    CurrentDice = o.ToString();
                else CurrentDice = "11";

                if (newTurnDict.TryGetValue("CurrentTurn", out o))
                    CurrentTurnPlayer = o.ToString();
            }

            if (dataDict.TryGetValue("NewBoard", out o))
                NewBoard = o.ToString();

            if (dataDict.TryGetValue("MoveId", out o))
                MoveId = o.ParseInt();

            if (dataDict.TryGetValue("Move", out o))
                lastMove = o.ToString();

            if (dataDict.TryGetValue("MatchId", out o))
                MatchId = o.ParseInt();

            if (dataDict.TryGetValue("IsRoll", out o))
                IsRolled = o.ParseBool();
        }

        private void HandlePlayers(Dictionary<string, object> dataDict)
        {

            object o;
            float turnTime = 0;
            if (dataDict.TryGetValue("CurrentTurnTime", out o))
                turnTime = o.ParseFloat();

            float maxTurnTime = 0;
            if (dataDict.TryGetValue("MaxTurnTime", out o))
                maxTurnTime = o.ParseFloat();

            float maxBankTime = 0;
            if (dataDict.TryGetValue("MaxBankTime", out o))
                maxBankTime = o.ParseFloat();

            Dictionary<string, Dictionary<Enums.StoreType, string[]>> usedItems = new Dictionary<string, Dictionary<Enums.StoreType, string[]>>();
            if (dataDict.TryGetValue("RoomData", out o))
            {
                foreach (var user in o as Dictionary<string, object>)
                {
                    Dictionary<Enums.StoreType, string[]> items = new Dictionary<Enums.StoreType, string[]>();
                    foreach (var item in user.Value as Dictionary<string, object>)
                    {
                        Enums.StoreType type = (Enums.StoreType)System.Enum.Parse(typeof(Enums.StoreType), item.Key);
                        List<object> list = (List<object>)item.Value;
                        List<string> str = list.ConvertAll(x => x.ToString());
                        items.Add(type, str.ToArray());
                    }
                    usedItems.Add(user.Key, items);
                }
            }
            if (dataDict.TryGetValue("Users", out o))
            {
                List<object> users = o as List<object>;
                Players = new Dictionary<string, PlayerData>();
                for (int i = 0; i < users.Count; i++)
                {
                    GameUser user = new GameUser((Dictionary<string, object>)users[i]);                    
                    float playerTurnTime = CurrentTurnPlayer == user.UserId ? turnTime : 0;
                    PlayerData playerData = new PlayerData(user, playerTurnTime, maxTurnTime, maxBankTime);

                    Dictionary<Enums.StoreType, string[]> data;
                    if (usedItems.TryGetValue(user.UserId, out data))
                        playerData.SelectedItems = data;
                    Players.Add(user.UserId, playerData);
                }
            }

        }

        private void HandleBet(Dictionary<string, object> dataDict)
        {
            object o;
            if (dataDict.TryGetValue("AmountsOfDoubles", out o))
                DoublesCount = o.ParseInt();

            if (dataDict.TryGetValue("WhoCantDouble", out o))
                CantDoublePlayer = o.ToString();

            if (dataDict.TryGetValue("MaxPossibleBet", out o))
                MaxBet = o.ParseFloat();

            if (dataDict.TryGetValue("IsDoubleCube", out o))
                IsRequestingDouble = o.ParseBool();

            if (dataDict.TryGetValue("DoubleAgain", out o))
                canDoubleAgain = o.ParseBool();

            // TO DO : CHECK IF THE GAME IS VIRTUAL OR CASH
            Kind = AppInformation.MATCH_KIND;

            if (dataDict.TryGetValue("Bet", out o))
                Bet = o.ParseFloat();

            if (dataDict.TryGetValue("Fee", out o))
                Fee = o.ParseFloat();

            if (dataDict.TryGetValue("MaxDoubleAmount", out o))
                MaxDoubleAmount = o.ParseInt();
        }

        public override string ToString()
        {
            return "MatchId : " + MatchId + "  --  currentTurnPlayer : " + CurrentTurnPlayer + "  --  currentDice : " + CurrentDice +
                "  --  newBoard : " + NewBoard + "  -- moveId : " + MoveId + "  --  lastMove : " + lastMove +
                "  --  amountOfDoubles : " + DoublesCount + "  --  cantDoublePlayer : " + CantDoublePlayer + "  --  isRequestingDoubleCube : " + IsRequestingDouble +
                "  --  canDoubleAgain : " + canDoubleAgain + "  --  kind : " + Kind.ToString() + "  --  bet : " + Bet + "  --  fee : " + Fee + "  --  maxBet : " + MaxBet + 
                "  -- players : " + Players.Display() + "  -- is rolled already: : " + IsRolled;
        }
    }
}
