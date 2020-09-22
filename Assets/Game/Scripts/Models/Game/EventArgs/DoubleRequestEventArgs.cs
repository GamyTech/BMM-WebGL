using GT.Backgammon.Player;
using UnityEngine;

namespace GT.Backgammon.Logic
{
    public class DoubleRequestEventArgs
    {
        public virtual IPlayer RequestedPlayer { get; protected set; }
        public virtual Enums.MatchKind Kind { get; protected set; }
        public virtual float CurrentBet { get; protected set; }
        public virtual float CurrentFee { get; protected set; }
        public virtual string NewBet { get; protected set; }
        public virtual string NewFee { get; protected set; }
        public virtual string DoubleAgainBet { get; protected set; }
        public virtual string DoubleAgainFee { get; protected set; }
        public virtual bool CanDoubleAgain { get; protected set; }

        public DoubleRequestEventArgs(IPlayer requestedPlayer, Stake stake)
        {
            RequestedPlayer = requestedPlayer;
            Kind = stake.Kind;
            CurrentBet = stake.CurrentBet;
            CurrentFee = stake.CurrentFee;
            NewBet = stake.DoubleBetToString();
            NewFee = stake.DoubleFeeToString();
            DoubleAgainBet = stake.DoubleAgainBetToString();
            DoubleAgainFee = stake.DoubleAgainFeeToString();
            if (requestedPlayer is ILocalPlayer)
                CanDoubleAgain = stake.DoubleAgainBet() <= stake.MaxBet;
            else
                CanDoubleAgain = false;
        }
    }
}
