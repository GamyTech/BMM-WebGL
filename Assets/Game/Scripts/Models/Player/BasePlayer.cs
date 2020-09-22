using GT.Backgammon.Logic;

namespace GT.Backgammon.Player
{
    public abstract class BasePlayer : IPlayer
    {
        public event TurnHandler OnStartTurn;
        public event TurnHandler OnEndTurn;
        public event MoveDoneHandler OnMoveDone;
        public event DiceSetHandler OnDiceSet;
        public event DiceRolledHandler OnDiceRolled;
        
        public event DoubleRequestedHandler OnDoubleRequested;
        public event DoubleResponseHandler OnDoubleResponse;
        public event DoubleInitiatedHandler OnDoubleInitiated;

        protected string m_playerId;
        protected PlayerColor m_color;
        protected Dice m_dice;
        protected PlayerData m_playerData;


        public string playerId
        {
            get { return m_playerId; }
        }

        public PlayerColor playerColor
        {
            get { return m_color; }
        }

        public Dice dice
        {
            get { return m_dice; }
        }

        public PlayerData playerData
        {
            get { return m_playerData; }
        }

        /// <summary>
        /// Minimal Constructor
        /// </summary>
        /// <param name="color"></param>
        public BasePlayer(string id, PlayerColor color)
        {
            m_playerId = id;
            m_color = color;
            m_dice = new Dice();
        }

        /// <summary>
        /// Extended Data Constructor
        /// </summary>
        /// <param name="color"></param>
        /// <param name="playerData"></param>
        public BasePlayer(string id, PlayerColor color, PlayerData playerData) : this(id, color)
        {
            m_playerData = playerData;
        }

        #region Events
        protected virtual void DiceSetEvent(Dice dice)
        {
            if (OnDiceSet != null)
                OnDiceSet(dice);
        }

        protected virtual void DiceRolledEvent(Dice dice)
        {
            if (OnDiceRolled != null)
                OnDiceRolled(dice);
        }

        protected virtual void OnMovesDoneEvent(params Move[] moves)
        {
            if (OnMoveDone != null)
                OnMoveDone(moves);
        }

        protected virtual void OnStartTurnEvent()
        {
            if (OnStartTurn != null)
                OnStartTurn();
        }

        protected virtual void OnEndTurnEvent()
        {
            if (OnEndTurn != null)
                OnEndTurn();
        }

        protected virtual void OnDoubleRequestedEvent()
        {
            if (OnDoubleRequested != null)
                OnDoubleRequested();
        }

        protected virtual void OnDoubleResponseEvent(DoubleResponse response)
        {
            if (OnDoubleResponse != null)
                OnDoubleResponse(response);
        }

        protected virtual void OnDoubleInitiatedEvent()
        {
            if (OnDoubleInitiated != null)
                OnDoubleInitiated();
        }
        #endregion Events

        #region IPlayer Implementation
        public virtual void Reset()
        {
            if(playerData != null)
                playerData.ResetCurrentTime();
        }

        public virtual void ExecuteMoves(Board board, params Move[] moves)
        {
            board.MakeMove(moves);
            OnMovesDoneEvent(moves);
        }

        public virtual void StartTurn(Board board)
        {
            ClearTurn();

            OnStartTurnEvent();
        }

        public virtual void EndTurn()
        {
            ClearTurn();

            OnEndTurnEvent();
        }

        public virtual void SetDice(int first, int second, Board board)
        {
            if(dice.SetDice(first, second))
            {
                DiceSetEvent(dice);
            }
        }

        public virtual void UpdateSelectedItems(Enums.StoreType type, string[] itemIds)
        {
            m_playerData.UpdateUsedItems(type, itemIds);
        }

        public virtual void RollDice()
        {
            if(dice.RollDice())
                DiceRolledEvent(dice);
        }

        public virtual void RequestedDouble(Board board)
        {
            OnDoubleRequestedEvent();
        }

        public virtual void DoubleResponse(DoubleResponse response)
        {
            OnDoubleResponseEvent(response);
        }
        #endregion IPlayer Implementation

        protected virtual void ClearTurn()
        {
            dice.ResetDice();
        }

        public virtual void EndGame() {}

        public override string ToString()
        {
            return m_color.ToString();
        }
    }
}
