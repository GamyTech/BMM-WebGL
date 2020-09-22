using GT.Backgammon.Player;

namespace GT.Backgammon.Logic
{
    public class GameStartEventArgs
    {
        public virtual IPlayer CurrentPlayer { get; protected set; }

        public GameStartEventArgs(IPlayer currentPlayer)
        {
            CurrentPlayer = currentPlayer;
        }
    }
}
