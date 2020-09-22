using GT.Backgammon.Logic;

namespace GT.Backgammon.Player
{
    public interface ILocalPlayer
    {
        TreeNode<Move> possibleMoves { get; }

        Move[] movesLastTurn { get; }
        int eatenLastTurn { get; }

        void SetCanDouble(bool canDouble);
        bool CanDouble();
    }
}
