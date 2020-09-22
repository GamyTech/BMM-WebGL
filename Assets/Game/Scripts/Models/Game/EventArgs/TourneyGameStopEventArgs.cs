using GT.Backgammon.Player;

namespace GT.Backgammon.Logic
{
    public class TourneyGameStopEventArgs : RemoteGameStopEventArgs
    {
        public int UserScores { get; protected set; }
        public int OpponentScores { get; protected set; }

        public TourneyGameStopEventArgs(bool isCanceled, IPlayer winner, Enums.MatchKind kind, float bet, PlayerStats player1Stats, 
                PlayerStats player2Stats, int userScores, int opponentScores)
                : base(isCanceled, winner, kind, bet, 0, 0, player1Stats, player2Stats)
        {
            UserScores = userScores;
            OpponentScores = opponentScores;
        }
    }
}
