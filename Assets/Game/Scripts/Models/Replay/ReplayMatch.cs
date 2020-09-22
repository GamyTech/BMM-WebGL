using GT.Backgammon.Player;
using System.Collections.Generic;
using System;

namespace GT.Backgammon
{
    public class ReplayMatch
    {
        public Dictionary<string, PlayerData> Players { get; private set; }
        public string MatchId { get; private set; }
        public float StartingBet { get; private set; }
        public float MaxBet { get; private set; }
        public Enums.MatchKind Kind { get; private set; }
        public List<GameState> GameStates { get; private set; }
        public int CurrentStateIndex { get; private set; }
        public DateTime StartingTime { get; private set; }

        #region Events/Delegates
        public delegate void GoToNextStatedHandler(GameState eventArgs);
        public delegate void GoToPreviousStateHandler(GameState eventArgs);
        public delegate void JumpToStatedHandler(GameState eventArgs);

        public event GoToNextStatedHandler OnGoToNextState = delegate { };
        public event GoToPreviousStateHandler OnGoToPreviousState = delegate { };
        public event JumpToStatedHandler OnJumpToState = delegate { };
        #endregion Events/Delegates

        public ReplayMatch(ReplayMatchData data)
        {
            Kind = data.Kind;
            MatchId = data.MatchId;
            GameStates = data.GameLogs;
            StartingTime = data.StartingTime;
            Players = data.Players;
            StartingBet = data.StartingBet;
            MaxBet = data.MaxBet;
        }

        public void NextState()
        {
            SelectNewSate(CurrentStateIndex + 1);
        }

        public void PreviousState()
        {
            SelectNewSate(CurrentStateIndex -1);
        }

        public void JumpToState(int logIndex)
        {
            SelectNewSate(logIndex);
        }

        private void SelectNewSate(int newIndex)
        {
            if (GameStates == null || GameStates.Count == 0 || newIndex < 0 || newIndex >= GameStates.Count)
            {
                UnityEngine.Debug.LogError(newIndex + " is out of gameLogs range");
                return;
            }

            int previousIndex = CurrentStateIndex;
            CurrentStateIndex = newIndex;

            GameStates[previousIndex].Selected = false;
            GameStates[CurrentStateIndex].Selected = true;

            if (Math.Abs(newIndex - previousIndex) == 1)
            {
                if (CurrentStateIndex > previousIndex)
                    OnGoToNextState(GameStates[CurrentStateIndex]);
                else
                    OnGoToPreviousState(GameStates[CurrentStateIndex]);
            }
            else
                OnJumpToState(GameStates[CurrentStateIndex]);
        }
    }
}
