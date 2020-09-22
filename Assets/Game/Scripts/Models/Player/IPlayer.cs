using GT.Backgammon.Logic;

namespace GT.Backgammon.Player
{
    public enum PlayerColor { White = 0, Black = 1 }

    public delegate void TurnHandler();
    public delegate void MoveDoneHandler(params Move[] moves);
    public delegate void DiceSetHandler(Dice dice);
    public delegate void DiceRolledHandler(Dice dice);
    public delegate void DoubleRequestedHandler();
    public delegate void DoubleResponseHandler(DoubleResponse response);
    public delegate void DoubleInitiatedHandler();

    public interface IPlayer
    {
        event TurnHandler OnStartTurn;
        event TurnHandler OnEndTurn;
        event MoveDoneHandler OnMoveDone;
        event DiceSetHandler OnDiceSet;
        event DiceRolledHandler OnDiceRolled;
        event DoubleRequestedHandler OnDoubleRequested;
        event DoubleResponseHandler OnDoubleResponse;
        event DoubleInitiatedHandler OnDoubleInitiated;

        string playerId { get; }
        PlayerColor playerColor { get; }
        Dice dice { get; }
        PlayerData playerData { get; }

        void Reset();
        void ExecuteMoves(Board board, params Move[] moves);
        void StartTurn(Board board);
        void EndTurn();
        void EndGame();
        void SetDice(int first, int second, Board board);
        void RollDice();
        void UpdateSelectedItems(Enums.StoreType type, string[] itemIds);
        void RequestedDouble(Board board);
        void DoubleResponse(DoubleResponse response);
    }
}
