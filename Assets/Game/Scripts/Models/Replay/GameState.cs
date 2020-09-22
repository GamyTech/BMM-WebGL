using System.Collections.Generic;
using GT.Backgammon.Player;
using GT.Backgammon.Logic;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace GT.Backgammon
{

    public enum GameLogType
    {
        Unknown,
        StartMatch,
        SendMove,
        DoubleCubeYes,
        SendDoubleCubeLogic,
        StoppedGame,
    }

    public class GameState : DynamicElement
    {
        public bool Selected;

        public int Index { get; private set; }
        public TimeSpan RelativeTime { get; private set; }

        public int LogId { get; protected set; }
        public DateTime TriggeredTime { get; protected set; }
        public string NextPlayerId { get; protected set; }
        public string CurrentPlayerId { get; protected set; }
        public PlayerColor CurrentPlayerColor { get; protected set; }
        public PlayerColor NextPlayerColor { get; protected set; }
        public GameLogType LogType { get; protected set; }

        public string Winner { get; protected set; }

        public int[] CurrentDice { get; protected set; }
        public int[] NextDice { get; protected set; }
        public Move[] CurrentMoves { get; protected set; }

        public string CurrentSerializedBoard { get; protected set; }
        public Board CurrentBoard { get; protected set; }

        public Move[] NextMoves { get; protected set; }

        public PlayerColor CanDoublePlayer { get; protected set; }
        public int AmountOfDoubles { get; protected set; }

        internal GameState(int index, GameState previousState, Dictionary<string, object> logData)
        {
            Index = index;

            object o;

            if (logData.TryGetValue("LogId", out o))
                LogId = o.ParseInt();
            else
                Debug.Log("LogId is missing in dictionary");

            DateTime time;
            if (logData.TryGetValue("Date", out o) && DateTime.TryParse(o.ToString(), out time))
                TriggeredTime = time;
            else
                Debug.Log("Date is missing in dictionary");

            GameLogType type = GameLogType.Unknown;
            if (logData.TryGetValue("Status", out o))
                Utils.TryParseEnum(o.ToString().Replace(" ",""), out type, true);
            else
                Debug.Log("Status is missing in dictionary");

            LogType = type;

            if(previousState != null)
            {
                if(LogType == GameLogType.DoubleCubeYes)
                {
                    AmountOfDoubles = previousState.AmountOfDoubles + 1;
                    CanDoublePlayer = AmountOfDoubles == 1 ? CurrentPlayerColor : Board.GetOpponentsColor(previousState.CanDoublePlayer);
                }
                else
                {
                    AmountOfDoubles = previousState.AmountOfDoubles;
                    CanDoublePlayer = previousState.CanDoublePlayer;
                }
            }

            ChangeBoard(previousState, logData);
        }

        private void ChangeBoard(GameState previousState, Dictionary<string, object> data)
        {
            object o;

            PlayerColor color;
            if (data.TryGetValue("NewTurn", out o) && o != null && !string.IsNullOrEmpty(o.ToString()))
            {
                Dictionary<string, object> newTurn = MiniJSON.Json.Deserialize(o.ToString()) as Dictionary<string, object>;
                if (newTurn.TryGetValue("Color", out o) && Utils.TryParseEnum(o, out color))
                    NextPlayerColor = color;
                else if (previousState != null)
                    CurrentPlayerColor = previousState.CurrentPlayerColor;
                else
                    CurrentPlayerColor = PlayerColor.White;

                if (newTurn.TryGetValue("CurrentTurn", out o))
                    NextPlayerId = o.ToString();

                if (newTurn.TryGetValue("Dice", out o))
                {
                    string serial = o.ToString();
                    NextDice = new int[2];
                    NextDice[0] = serial[0].ParseInt();
                    NextDice[1] = serial[1].ParseInt();
                }

            }

            if (previousState != null)
            {
                CurrentPlayerColor = previousState.NextPlayerColor;
                CurrentPlayerId = previousState.NextPlayerId;
                if (LogType == GameLogType.SendMove)
                    CurrentDice = previousState.NextDice;
            }

            if (data.TryGetValue("Move", out o))
            {
                if(LogType == GameLogType.SendMove && o.ToString() != "No Moves")
                    CurrentMoves = Move.DeserializeMoves(o.ToString());
                else if(LogType == GameLogType.StoppedGame)
                {
                    Dictionary<string, object> stoppeddict = MiniJSON.Json.Deserialize(o.ToString()) as Dictionary<string, object>;
                    if (stoppeddict != null && stoppeddict.TryGetValue("Winner", out o))
                    {
                        Winner = o.ToString();
                        CurrentPlayerColor = previousState.CurrentPlayerColor;
                        CurrentPlayerId = previousState.CurrentPlayerId;
                    }
                    else
                        Debug.LogError("Winner is missing in dictionary");
                }
            }

            if (data.TryGetValue("NewBoard", out o) && !string.IsNullOrEmpty(o.ToString()) && o.ToString() != "Stopped Game")
            {
                CurrentSerializedBoard = o.ToString();
                CurrentBoard = new Board(CurrentSerializedBoard);
            }

           if (CurrentBoard == null && previousState != null)
            {
                CurrentSerializedBoard = previousState.CurrentSerializedBoard;
                CurrentBoard = previousState.CurrentBoard;
            }
        }

        public void SetNextMoves(Move[] nextMoves)
        {
            NextMoves = nextMoves;
        }

        public void SetRelativeTime(DateTime firstLogTime)
        {
            RelativeTime = TriggeredTime.Subtract(firstLogTime);
        }

        protected override void Populate(RectTransform activeObject)
        {
            activeObject.GetComponent<GameLogView>().Populate(this);
        }
    }
}
