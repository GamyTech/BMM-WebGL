using System.Collections;
using GT.Backgammon.Logic;
using System.Collections.Generic;

namespace GT.Backgammon.View
{
    public delegate void View_ExecuteMoveHandler(int from, int to);

    public interface IBoardView 
    {
        event View_ExecuteMoveHandler OnView_ExecuteMove;

        ISlotView[] viewSlots { get; }
        void InitView(float moveDuration, bool isWhiteBottom);
        void ShowBoardState(string boardString);
        IEnumerator Move(Move move);
        IEnumerator Undo(Move move);
        void SetMovesDict(Dictionary<int, HashSet<int>> movesDict);
        void ShowMovesIndicators();
        void TurnStarted(int turnCount, int whitePip, int blackPip);
    }
}
