using GT.Backgammon.Player;

namespace GT.Backgammon.Logic
{
    public class LiveGameStopEventArgs : GameStopEventArgs
    {
        public virtual Enums.MatchKind Kind { get; protected set; }
        public virtual float Bet { get; protected set; }
        public virtual float Fee { get; protected set; }

        public LiveGameStopEventArgs(IPlayer winner, Enums.MatchKind kind, float bet, float fee) : base(winner)
        {
            Kind = kind;
            Bet = bet;
            Fee = fee;
        }
    }
}
