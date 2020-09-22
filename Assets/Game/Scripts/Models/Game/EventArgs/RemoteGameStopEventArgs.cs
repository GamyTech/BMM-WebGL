using GT.Backgammon.Player;

namespace GT.Backgammon.Logic
{
    public class RemoteGameStopEventArgs : LiveGameStopEventArgs
    {
        public PlayerStats Player1Stats { get; protected set; }
        public PlayerStats Player2Stats { get; protected set; }
        public float Loyalty { get; protected set; }
        public bool IsCanceled { get; protected set; }

        public RemoteGameStopEventArgs(bool isCanceled, IPlayer winner, Enums.MatchKind kind, float bet, float fee, int loyalty, PlayerStats player1Stats, PlayerStats player2Stats)
            : base(winner, kind, bet, fee)
        {
            IsCanceled = isCanceled;
            Player1Stats = player1Stats;
            Player2Stats = player2Stats;
            Loyalty = loyalty;
        }
    }
}
