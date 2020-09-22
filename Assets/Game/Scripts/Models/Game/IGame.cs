using GT.Backgammon.Player;

namespace GT.Backgammon.Logic
{
    public enum GameStatus { PreGame, Started, Stopped }

    public delegate void GameInitializedHandler(GameInitializedEventArgs eventArgs);
    public delegate void GameStartedHandler(GameStartEventArgs eventArgs);
    public delegate void GameResumedHandler(GameStartEventArgs eventArgs);
    public delegate void GameStoppedHandler(GameStopEventArgs eventArgs);
    public delegate void GameBoardResetHandler(Board newBoard);

    public delegate void DiceHandler(IPlayer player, Dice dice);
    public delegate void StartTurnHandler(IPlayer player);
    public delegate void EndTurnHandler(IPlayer currentPlayer, IPlayer nextPlayer);
    public delegate void MoveDoneHandler(IPlayer player, params Move[] moves);
    public interface IGame 
    {
        event GameStartedHandler OnGameStarted;
        event GameResumedHandler OnGameResumed;
        event GameStoppedHandler OnGameStopped;
        event GameBoardResetHandler OnGameBoardReset;

        event DiceHandler OnDiceSet;
        event DiceHandler OnDiceRolled;
        event StartTurnHandler OnStartTurn;
        event EndTurnHandler OnEndTurn;
        event MoveDoneHandler OnMoveDone;

        Board GameBoard { get; set; }

        IPlayer CurrentTurnPlayer { get; }

        bool IsRolled { get; }

        IPlayer[] players { get; }

        int CurrentTurnIndex { get; }

        GameStatus CurrentGameState { get; }

        void StartGame();

        void StartGame(IPlayer currentTurnPlayer);

        void ResumeMatch(IPlayer currentTurnPlayer, int turnIndex, string currentBoard, bool isRolled);

        void StopGame(IPlayer winner);

        IPlayer NextPlayer(IPlayer player);
    }
}
