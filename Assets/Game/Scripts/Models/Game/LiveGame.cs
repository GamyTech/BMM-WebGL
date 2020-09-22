using GT.Backgammon.Player;
using System.Collections.Generic;
using UnityEngine;

namespace GT.Backgammon.Logic
{
    public abstract class LiveGame : BaseGame
    {
        #region Events/Delegates
        public delegate void BetChangedHandler(BetChangedEventArgs eventArgs);
        public delegate void TimerChangedHandler(IPlayer player, PlayerData data);
        public delegate void TimerEndedHandler(IPlayer player);
        public delegate void DoubleRequestHandler(DoubleRequestEventArgs eventArgs);
        public delegate void UndoDoneHandler(IPlayer player, params Move[] moves);

        public event BetChangedHandler OnBetChanged = delegate { };
        public event TimerChangedHandler OnTimerChanged = delegate { };
        public event TimerEndedHandler OnTurnTimerEnded = delegate { };
        public event TimerEndedHandler OnBankTimerEnded = delegate { };
        public event DoubleRequestHandler OnDoubleRequest = delegate { };
        public event UndoDoneHandler OnUndoDone = delegate { };
        #endregion Events/Delegates

        protected Stake m_stake;
        protected GameTimer turnTimer;

        #region Constructor
        public LiveGame(Stake stake, params IPlayer[] players) : base(players)
        {
            m_stake = stake;
            m_stake.OnBetChanged += a => OnBetChanged(a);

            for (int i = 0; i < players.Length; i++)
            {
                IPlayer p = players[i];
                p.OnDoubleResponse += r => OnDoubleResponseEvent(r, p);
                p.OnDoubleRequested += () => OnDoubleRequestedEvent(p);
                p.OnDoubleInitiated += () => OnDoubleInitiatedEvent(p);
                if (p is IHumanPlayer)
                    (p as IHumanPlayer).OnUndoDone += m => OnUndoDoneEvent(p, m);
            }

            turnTimer = new GameTimer();
            turnTimer.OnTurnTimeChanged += OnTurnTimerChangedEvent;
            turnTimer.OnTurnTimeRanOut += OnTurnTimerRanOutEvent;
            turnTimer.OnBankTimeChanged += OnBankTimerChangedEvent;
            turnTimer.OnBankTimeRanOut += OnBankTimerRanOutEvent;
        }
        #endregion Constructor

        #region Overrides
        protected override void OnGameStartedEvent(GameStartEventArgs eventArgs)
        {
            base.OnGameStartedEvent(new LiveGameStartEventArgs(eventArgs.CurrentPlayer, m_stake));
            
            for (int i = 0; i < players.Length; i++)
                if (!players[i].playerId.Equals(eventArgs.CurrentPlayer.playerId))
                    SetPlayerDoubleState(players[i]);

            OnTimerChanged(players[0], players[0].playerData);
            OnTimerChanged(players[1], players[1].playerData);
        }

        protected override void OnGameResumedEvent(GameStartEventArgs eventArgs)
        {
            base.OnGameResumedEvent(new LiveGameStartEventArgs(eventArgs.CurrentPlayer, m_stake));

            for (int i = 0; i < players.Length; i++)
                if (CurrentTurnIndex > 0 || !players[i].playerId.Equals(eventArgs.CurrentPlayer.playerId))
                    SetPlayerDoubleState(players[i]);

            OnTimerChanged(players[0], players[0].playerData);
            OnTimerChanged(players[1], players[1].playerData);
        }

        protected override void OnStartTurnEvent(IPlayer player)
        {
            StartTimer(player);
            base.OnStartTurnEvent(player);
        }

        protected override void OnEndTurnEvent(IPlayer player)
        {
            StopTimer(player);
            base.OnEndTurnEvent(player);
        }

        protected override void StartNextTurn()
        {
            for (int i = 0; i < players.Length; i++)
                SetPlayerDoubleState(players[i]);

            base.StartNextTurn();
        }

        protected override GameStopEventArgs CreateStopGameEventArgs(IPlayer winner)
        {
            return new LiveGameStopEventArgs(winner, m_stake.Kind, m_stake.CurrentBet, m_stake.CurrentFee);
        }

        public override string ToString()
        {
            return "Stake: " + m_stake;
        }
        #endregion Overrides

        #region Virtual Methods
        /// <summary>
        /// Current turn player initiated double request
        /// </summary>
        public virtual void InitiateDoubleRequest()
        {
            NextPlayer(CurrentTurnPlayer).RequestedDouble(GameBoard);
        }

        public virtual void UpdateGame(float deltaTime)
        {
            turnTimer.UpdateTimers(deltaTime);
        }

        public abstract void Surrender();
        public abstract void Rematch(bool isRematch);
        #endregion Virtual Methods

        #region Private/Protected Methods
        protected virtual void StopTimer(IPlayer player)
        {
            turnTimer.StopTurn();
            if (player.playerData != null)
            {
                player.playerData.CurrentTurnTime = player.playerData.TotalTurnTime;
                OnTimerChanged(player, player.playerData);
            }
        }

        protected virtual void StartTimer(IPlayer player)
        {
            if (player.playerData != null)
                turnTimer.StartTurn(player.playerData.CurrentTurnTime, player.playerData.CurrentBankTime);
        }

        private void SetPlayerDoubleState(IPlayer player)
        {
            if (player is ILocalPlayer)
                (player as ILocalPlayer).SetCanDouble(m_stake.CanDouble(player.playerId));
        }
        #endregion Private/Protected Methods

        #region Virtual Events
        protected virtual void OnUndoDoneEvent(IPlayer player, params Move[] moves)
        {
            OnUndoDone(player, moves);
        }

        protected virtual void OnDoubleRequestedEvent(IPlayer player)
        {
            m_stake.RequestedDouble(CurrentTurnPlayer);

            StopTimer(CurrentTurnPlayer);
            CurrentTurnPlayer = player;
            SetPlayerDoubleState(CurrentTurnPlayer);
            StartTimer(CurrentTurnPlayer);

            if (player is IHumanPlayer)
                OnDoubleRequest(new DoubleRequestEventArgs(player, m_stake));
        }

        protected virtual void OnDoubleInitiatedEvent(IPlayer player)
        {
            InitiateDoubleRequest();
        }

        protected virtual void OnDoubleResponseEvent(DoubleResponse response, IPlayer player)
        {
            StopTimer(player);
        }

        protected virtual void OnTurnTimerChangedEvent(float time)
        {
            PlayerData data = CurrentTurnPlayer.playerData;
            if (data != null)
            {
                data.CurrentTurnTime = time;
                OnTimerChanged(CurrentTurnPlayer, data);
            }
        }

        protected virtual void OnBankTimerChangedEvent(float time)
        {
            PlayerData data = CurrentTurnPlayer.playerData;
            if (data != null)
            {
                data.CurrentBankTime = time;
                OnTimerChanged(CurrentTurnPlayer, data);
            }
        }

        protected virtual void OnTurnTimerRanOutEvent()
        {
            OnTurnTimerEnded(CurrentTurnPlayer);
        }

        protected virtual void OnBankTimerRanOutEvent()
        {
            OnBankTimerEnded(CurrentTurnPlayer);
        }
        #endregion Virtual Events

        #region Input Event Functions
        public void EndTurn()
        {
            if (CurrentTurnPlayer is IHumanPlayer && (CurrentTurnPlayer as IHumanPlayer).GetPossibleMoves().Count == 0)
                CurrentTurnPlayer.EndTurn();
        }

        public void ApplyMove(int from, int to)
        {
            if (CurrentTurnPlayer is IHumanPlayer)
                (CurrentTurnPlayer as IHumanPlayer).MakeMove(GameBoard, from, to);
        }

        public void ApplyBestMove(int from)
        {
            if (CurrentTurnPlayer is IHumanPlayer)
                (CurrentTurnPlayer as IHumanPlayer).MakeBestMove(GameBoard, from);
        }

        public void UndoMove()
        {
            if (CurrentTurnPlayer is IHumanPlayer)
                (CurrentTurnPlayer as IHumanPlayer).MakeUndo(GameBoard);
        }
        #endregion Input Event Functions
    }
}
