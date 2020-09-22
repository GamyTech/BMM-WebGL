using System.Collections.Generic;
using GT.Backgammon.Player;
using System;
using UnityEngine;

namespace GT.Backgammon
{
    public class ReplayMatchData
    {
        public DateTime StartingTime { get; private set; }
        public string Winner { get; private set; }
        public float MaxBet { get; set; }
        public float StartingBet { get; private set; }
        public string MatchId { get; private set; }
        public Enums.MatchKind Kind { get; private set; }
        public Dictionary<string,PlayerData> Players;
        public List<GameState> GameLogs { get; private set; }

        public ReplayMatchData(Dictionary<string, object> data)
        {
            GameLogs = new List<GameState>();
            Players = new Dictionary<string, PlayerData>();
            Dictionary<string, object> lastLogDict = null;

            object o;
            if ((data.TryGetValue("HistoryMoves", out o) || data.TryGetValue("Moves", out o)) && (o as List<object>) != null)
            {
                List<object> list = o as List<object>;
                for (int x = 0; x < list.Count; ++x)
                {
                    GameState prevState = x > 0 ? GameLogs[x - 1] : null;
                    GameState newState = new GameState(x, prevState, list[x] as Dictionary<string, object>);
                    if (prevState != null)
                        prevState.SetNextMoves(newState.CurrentMoves);

                    GameLogs.Add(newState);
                    newState.SetRelativeTime(GameLogs[0].TriggeredTime);
                }

                lastLogDict = list[list.Count - 1] as Dictionary<string, object>;
                GameLogs.Sort((a, b) => a.LogId.CompareTo(b.LogId));
                StartingTime = GameLogs[0].TriggeredTime;
            }

            if (data.TryGetValue("HistoryDetails", out o))
            {
                Dictionary<string, object> details = o as Dictionary<string, object>;

                if (details.TryGetValue("MatchId", out o))
                    MatchId = o.ToString();

                if (details.TryGetValue("Bet", out o))
                    StartingBet = o.ParseFloat();
                MaxBet = StartingBet * 64;

                if (details.TryGetValue("Winner", out o))
                {
                    Winner = o.ToString();
                    if(GameLogs.Count != 0 && GameLogs[GameLogs.Count - 1].LogType != GameLogType.StoppedGame)
                    {
                        lastLogDict.AddOrOverrideValue("LogId", GameLogs[GameLogs.Count - 1].LogId + 1);
                        lastLogDict.AddOrOverrideValue("Status", GameLogType.StoppedGame);
                        lastLogDict.AddOrOverrideValue("Move", "{\"Winner\":\"" + Winner + "\"}");
                        GameLogs.Add(new GameState(GameLogs.Count, GameLogs[GameLogs.Count - 1], lastLogDict));
                    }
                }

                string whitePlayerId = GameLogs.Count >= 1 ? GameLogs[0].NextPlayerId : null;

                string userId = null;
                string pictureURL = null;
                string name = null;
                PlayerColor color;
                if (details.TryGetValue("FirstUserId", out o))
                    userId = o.ToString();
                if (details.TryGetValue("FirstPicture", out o))
                    pictureURL = o.ToString();
                if (details.TryGetValue("FirstName", out o))
                    name = o.ToString();
                color = userId == whitePlayerId ? PlayerColor.White : PlayerColor.Black;
                Players.Add(userId, new PlayerData(name, new SpriteData(pictureURL), 0, 0, 0, 0, color));

                if (details.TryGetValue("SecondUserId", out o))
                    userId = o.ToString();
                if (details.TryGetValue("SecondPicture", out o))
                    pictureURL = o.ToString();
                if (details.TryGetValue("SecondName", out o))
                    name = o.ToString();
                color = Logic.Board.GetOpponentsColor(color);
                Players.Add(userId, new PlayerData(name, new SpriteData(pictureURL), 0, 0, 0, 0, color));
            }
            else
            {
                if(GameLogs.Count > 0)
                {
                    Players.Add(GameLogs[0].NextPlayerId, new PlayerData(GameLogs[0].NextPlayerId, null, 0, 0, 0, 0, PlayerColor.White));
                    if (GameLogs.Count > 1)
                        Players.Add(GameLogs[1].NextPlayerId, new PlayerData(GameLogs[1].NextPlayerId, null, 0, 0, 0, 0, PlayerColor.Black));

                    if (!string.IsNullOrEmpty(GameLogs[GameLogs.Count - 1].Winner))
                        Winner = GameLogs[GameLogs.Count - 1].Winner;
                    else
                        Winner = "Canceled";
                }
                Debug.LogError("HistoryDetails is missing from dictionnary");
            }

            // TO DO : CHECK IF THE GAME IS VIRTUAL OR CASH
            Kind = AppInformation.MATCH_KIND;
        }
    }
}
