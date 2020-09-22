using GT.Backgammon.Logic;

namespace GT.Backgammon.Player
{
    public class RemotePlayer : BasePlayer, IRemotePlayer
    {
        public RemotePlayer(string id, PlayerColor color, PlayerData playerData) : base(id, color, playerData) { }

        public virtual void ReceivedMove(Board board, string receivedData)
        {
            // deserialize to dice and moves,
            Move[] moves = Move.DeserializeMoves(receivedData);

            // roll dice
            RollDice();

            // execute moves
            ExecuteMoves(board, moves);

            // end turn
            EndTurn();
        }
    }
}
