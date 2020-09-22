using GT.Backgammon.Player;

namespace GT.Backgammon.Logic
{
    public class GameStopEventArgs
    {
        public virtual IPlayer Winner { get; protected set; }

        public GameStopEventArgs(IPlayer winner)
        {
            Winner = winner;
        }

    }
}
