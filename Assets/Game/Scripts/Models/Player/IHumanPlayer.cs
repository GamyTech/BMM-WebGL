using System.Collections.Generic;
using GT.Backgammon.Logic;

namespace GT.Backgammon.Player
{
    public interface IHumanPlayer
    {
        event MoveDoneHandler OnUndoDone;

        bool CanDouble();
        bool CanRoll();
        bool CanUndo();
        bool IsFinishedMoving();

        void RollDice();
        Dictionary<int, HashSet<int>> GetPossibleMoves();
        void MakeMove(Board board, int from, int to);
        void MakeBestMove(Board board, int from);
        void MakeUndo(Board board);
    }
}
