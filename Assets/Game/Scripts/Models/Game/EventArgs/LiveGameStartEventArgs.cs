using GT.Backgammon.Player;

namespace GT.Backgammon.Logic
{
    public class LiveGameStartEventArgs : GameStartEventArgs
    {

        public virtual Enums.MatchKind Kind { get; protected set; }
        public virtual float BetValue { get; protected set; }
        public virtual float FeeValue { get; protected set; }
        public virtual float MaxBetValue { get; protected set; }
        public virtual string CurrentBet { get; protected set; }
        public virtual string CurrentFee { get; protected set; }
        public virtual string MaxBet { get; protected set; }
        public virtual int CubeNum { get; protected set; }
        public virtual string CantDoublePlayerId { get; protected set; }

        public LiveGameStartEventArgs(IPlayer currentPlayer, Stake stake) : base(currentPlayer)
        {
            Kind = stake.Kind;
            CurrentBet = stake.BetToString();
            CurrentFee = stake.FeeToString();
            BetValue = stake.CurrentBet;
            FeeValue = stake.CurrentFee;
            MaxBet = stake.MaxBetToString();
            MaxBetValue = stake.MaxBet;
            CubeNum = stake.CubeNum;
            CantDoublePlayerId = stake.CantDoublePlayerId;
        }
    }
}
