using BestHTTP.WebSocket;
using GT.Exceptions;
using System.Collections.Generic;
using GT.Backgammon.Player;
using UnityEngine;

namespace GT.Websocket
{
    public class GameStartedService : Service
    {
        public string Dice { get; private set; }
        public Enums.MatchKind Kind { get; private set; }
        public float MaxBet { get; set; }
        public float CurrentBet { get; private set; }
        public float CurrentFee { get; private set; }
        public float TotalBankTime { get; private set; }
        public float TotalTurnTime { get; private set; }
        public float CurrentTurnTime { get; private set; }
        public string CurrentTurnPlayer { get; private set; }
        public int MatchId { get; private set; }
        public List<GameUser> Players;
        public string WhoCantDouble;
        public int MaxDoubleAmount;

        public GameStartedService(ServiceId serviceId, WebSocket webSocket, ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(serviceId, webSocket, eventHandler, data, rawData)
        {
            Init(data);
        }

        private void Init(Dictionary<string, object> data)
        {
            object o;

            if (data.TryGetValue("Dices", out o))
            {
                Dice = o.ToString();
            }
            else throw new MissingKeyException("Dices");

            if (data.TryGetValue("MaxBankTime", out o))
            {
                TotalBankTime = o.ParseFloat();
            }
            else throw new MissingKeyException("MaxBankTime");

            if (data.TryGetValue("MaxTurnTime", out o))
            {
                TotalTurnTime = o.ParseFloat();
            }
            else throw new MissingKeyException("MaxTurnTime");

            if (data.TryGetValue("CurrentTurnTime", out o))
            {
                CurrentTurnTime = o.ParseFloat();
            }
            else throw new MissingKeyException("CurrentTurnTime");

            if (data.TryGetValue("CurrentTurn", out o))
            {
                CurrentTurnPlayer = o.ToString();
            }
            else throw new MissingKeyException("CurrentTurn");

            if (data.TryGetValue("Bet", out o))
                CurrentBet = o.ParseFloat();

            if (data.TryGetValue("MaxPossibleBet", out o))
                MaxBet = o.ParseFloat();

            // TO DO : CHECK IF THE GAME IS VIRTUAL OR CASH
            Kind = AppInformation.MATCH_KIND;

            if (data.TryGetValue("Fee", out o))
                CurrentFee = o.ParseFloat();

            if (data.TryGetValue("MatchId", out o))
            {
                MatchId = o.ParseInt();
            }
            else throw new MissingKeyException("MatchId");

            WhoCantDouble = "";
            if (data.TryGetValue("WhoCantDouble", out o))
                WhoCantDouble = o.ToString();
            if (data.TryGetValue("MaxDoubleAmount", out o))
                MaxDoubleAmount = o.ParseInt();


            Players = new List<GameUser>();
            if (data.TryGetValue("Users", out o))
            {
                List<object> users = o as List<object>;
                for (int i = 0; i < users.Count; i++)
                {
                    GameUser user = new GameUser((Dictionary<string, object>)users[i]);
                    Players.Add(user);
                }
            }
            else throw new MissingKeyException("Users");
        }

        public GameUser GetPlayerById(string playerId)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].UserId == playerId)
                    return Players[i];
            }
            return null;
        }
    }
}
