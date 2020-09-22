using UnityEngine;
using GT.Backgammon.Player;

namespace GT.Backgammon.Logic
{
    public abstract class BaseGame : IGame
    {
        #region Events
        public static event GameInitializedHandler OnGameInitialized = delegate { };

        public event GameStartedHandler OnGameStarted = delegate { };
        public event GameResumedHandler OnGameResumed = delegate { };
        public event GameStoppedHandler OnGameStopped = delegate { };
        public event GameBoardResetHandler OnGameBoardReset = delegate { };

        public event DiceHandler OnDiceSet = delegate { };
        public event DiceHandler OnDiceRolled = delegate { };
        public event StartTurnHandler OnStartTurn = delegate { };
        public event EndTurnHandler OnEndTurn = delegate { };
        public event MoveDoneHandler OnMoveDone = delegate { };
        #endregion Events

        #region Fields & Properties
        public const int START_TURN_INDEX = 0;
        public IPlayer[] players { get; protected set; }
        public Board GameBoard { get; set; }
        public IPlayer CurrentTurnPlayer { get; protected set; }
        public bool IsRolled { get; private set; }
        public int CurrentTurnIndex{ get; protected set; }
        public GameStatus CurrentGameState { get; protected set; }
        #endregion Fields & Properties

        #region Constructor
        protected BaseGame(params IPlayer[] players)
        {
            CurrentGameState = GameStatus.PreGame;
            this.players = players;

            for (int i = 0; i < this.players.Length; i++)
            {
                IPlayer player = this.players[i];

                player.OnDiceSet += d => OnDiceSetEvent(player, d);
                player.OnDiceRolled += d => OnDiceRolledEvent(player,d);
                player.OnStartTurn += () => OnStartTurnEvent(player);
                player.OnEndTurn += () => TurnEnded(player);
                player.OnMoveDone += m => OnMoveDoneEvent(player, m);
            }
        }
        #endregion Constructor

        #region Initialization
        internal void InitializationComplete()
        {
            OnGameInitialized(new GameInitializedEventArgs(this, players));
        }
        #endregion Initialization

        #region Start/Stop Handling
        public virtual void StartGame()
        {
            int spi = Random.Range(0, players.Length);
            this.StartGame(players[spi]);
        }

        public virtual void StartGame(IPlayer currentTurnPlayer)
        {
            CurrentTurnIndex = START_TURN_INDEX;
            CurrentGameState = GameStatus.Started;
            CurrentTurnPlayer = currentTurnPlayer;

            for (int i = 0; i < players.Length; i++)
                players[i].Reset();

            GameBoard = new Board();

            OnGameStartedEvent(new GameStartEventArgs(CurrentTurnPlayer));

            StartNextTurn();
        }

        public virtual void ResumeMatch(IPlayer currentTurnPlayer, int turnIndex, string currentBoard, bool isRolled)
        {
            CurrentTurnIndex = turnIndex;
            CurrentGameState = GameStatus.Started;
            CurrentTurnPlayer = currentTurnPlayer;
            IsRolled = isRolled;

            GameBoard = new Board(currentBoard);

            OnGameResumedEvent(new GameStartEventArgs(CurrentTurnPlayer));

            StartNextTurn();
        }

        public virtual void StopGame(IPlayer winner)
        {
            if (CurrentGameState == GameStatus.Stopped)
                return;

            CurrentGameState = GameStatus.Stopped;
            CurrentTurnPlayer.EndTurn();

            OnGameStoppedEvent(CreateStopGameEventArgs(winner));
            CurrentTurnPlayer = null;

            for (int i = 0; i < players.Length; i++)
            {
                IPlayer player = players[i];

                player.OnDiceSet -= d => OnDiceSetEvent(player, d);
                player.OnDiceRolled -= d => OnDiceRolledEvent(player, d);
                player.OnStartTurn -= () => OnStartTurnEvent(player);
                player.OnEndTurn -= () => TurnEnded(player);
                player.OnMoveDone -= m => OnMoveDoneEvent(player, m);

                player.EndGame();
            }
        }

        protected virtual GameStopEventArgs CreateStopGameEventArgs(IPlayer winner)
        {
            return new GameStopEventArgs(winner);
        }
        #endregion Start/Stop Handling

        #region Virtual Events
        protected virtual void OnGameStartedEvent(GameStartEventArgs eventArgs)
        {
            OnGameStarted(eventArgs);
        }

        protected virtual void OnGameResumedEvent(GameStartEventArgs eventArgs)
        {
            OnGameResumed(eventArgs);
        }

        protected virtual void OnGameStoppedEvent(GameStopEventArgs eventArgs)
        {
            OnGameStopped(eventArgs);
        }

        protected virtual void OnGameBoardResetEvent(Board newBoard)
        {
            OnGameBoardReset(newBoard);
        }

        protected virtual void OnDiceSetEvent(IPlayer player, Dice dice)
        {
            OnDiceSet(player, dice);
        }

        protected virtual void OnDiceRolledEvent(IPlayer player, Dice dice)
        {
            OnDiceRolled(player, dice);
        }

        protected virtual void OnStartTurnEvent(IPlayer player)
        {
            OnStartTurn(player);
        }

        protected virtual void OnEndTurnEvent(IPlayer player)
        {
            OnEndTurn(player, NextPlayer(player));
        }

        protected virtual void OnMoveDoneEvent(IPlayer player, params Move[] moves)
        {
            OnMoveDone(player, moves);
        }
        #endregion Virtual Events

        #region Turn Handling
        protected void TurnEnded(IPlayer player)
        {
            OnEndTurnEvent(player);

            if (CurrentGameState == GameStatus.Stopped)
                return;

            CurrentTurnIndex++;

            if (CheckIfNextTurnPossible(player))
            {
                CurrentTurnPlayer = NextPlayer(CurrentTurnPlayer);
                NextTurnReady();
            }
            else 
                EndGame(player);
        }

        protected virtual void EndGame(IPlayer winner)
        {
            StopGame(winner);
        }

        protected virtual bool CheckIfNextTurnPossible(IPlayer player)
        {
            return (GameBoard.GetBearoffSlot(player.playerColor).Quantity != Board.MAX_IN_SLOT);
        }

        protected virtual void StartNextTurn()
        {
            CurrentTurnPlayer.StartTurn(GameBoard);
        }

        protected virtual void NextTurnReady()
        {
            StartNextTurn();
        }
        #endregion Turn Handling

        #region Protected Methods
        public virtual IPlayer NextPlayer(IPlayer player)
        {
            int index = 0;
            for (int i = 0; i < players.Length; i++)
                if (player.Equals(players[i]))
                    index = i;

            if (index + 1 < players.Length)
                index++;
            else
                index = 0;

            return players[index];
        }

        protected IPlayer PreviousPlayer(IPlayer player)
        {
            int index = 0;
            for (int i = 0; i < players.Length; i++)
                if (player.Equals(players[i]))
                    index = i;

            if (index - 1 >= 0)
                index--;
            else
                index = players.Length - 1;

            return players[index];
        }
        #endregion Protected Methods

        #region Public Methods
        public IPlayer GetPlayerFromId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            for (int i = 0; i < players.Length; i++)
                if(players[i].playerId.Equals(id))
                    return players[i];

            return null;
        }

        public IPlayer GetPlayerFromColor(PlayerColor color)
        {
            for (int i = 0; i < players.Length; i++)
                if (players[i].playerColor == color)
                    return players[i];

            return null;
        }
        #endregion Public Methods
    }
}
