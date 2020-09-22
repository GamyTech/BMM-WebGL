using GT.Backgammon.Player;
using UnityEngine;

namespace GT.Backgammon.Logic
{
    public enum DoubleResponse
    {
        Yes,
        GiveUp,
        DoubleAgain,
    }

    public class Stake
    {
        public const string CASH_AMOUNT_FORMAT = "#,##0.##";

        public delegate void BetChangedHandler(BetChangedEventArgs args);
        public event BetChangedHandler OnBetChanged;

        public Enums.MatchKind Kind { get; private set; }
        public float CurrentBet { get; private set; }
        public float CurrentFee { get; private set; }
        public float MaxBet { get; private set; }
        public string CantDoublePlayerId { get; private set; }
        public int CubeNum { get; private set; }

        public Stake(Enums.MatchKind kind, float currentBet, float currentFee, float maxBet, string cantDoubleId, int amountOfDoubles)
        {
            CubeNum = amountOfDoublesToCubeNum(amountOfDoubles);
            Kind = kind;

            CurrentBet = currentBet;

            CurrentFee = currentFee;
            MaxBet = maxBet;
            CantDoublePlayerId = cantDoubleId;
        }

        public bool CanDouble(string playerId)
        {
            return CurrentBet * 2 <= MaxBet && !playerId.Equals(CantDoublePlayerId);
        }

        public void DoubleDone(IPlayer cantDouble, float currentBet, float currentFee, bool isRequest = false)
        {
            if (isRequest)
                CubeNum *= 2;

            CantDoublePlayerId = cantDouble.playerId;
            CurrentBet = currentBet;
            CurrentFee = currentFee;

            SendUpdateBet(isRequest, cantDouble);
        }

        public void DoubleDone(IPlayer cantDouble, float currentBet, float currentFee, int amountOfDoubles, bool isRequest = false)
        {
            CubeNum = amountOfDoublesToCubeNum(amountOfDoubles);
            DoubleDone(cantDouble, currentBet, currentFee, false);
        }

        public void DoubleDone(IPlayer cantDouble, bool isRequest = false)
        {
            DoubleDone(cantDouble, CurrentBet * 2, CurrentFee * 2, isRequest);
        }

        public void RequestedDouble(IPlayer cantDouble)
        {
            CantDoublePlayerId = cantDouble.playerId;
            SendUpdateBet(true, cantDouble);
        }

        private void SendUpdateBet(bool isRequest, IPlayer cantDoublePlayer)
        {
            if (OnBetChanged != null)
                OnBetChanged(new BetChangedEventArgs(isRequest, Kind, CurrentBet, CurrentFee, MaxBet, CubeNum, cantDoublePlayer));
        }

        private int amountOfDoublesToCubeNum(int amountOfDoubles)
        {
            return 1 << amountOfDoubles;
        }

        private string AmountToString(float value)
        {
            return Wallet.AmountToString(value, Kind);
        }

        public string BetToString()
        {
            return AmountToString(CurrentBet);
        }

        public string FeeToString()
        {
            return AmountToString(CurrentFee);
        }

        public string MaxBetToString()
        {
            return AmountToString(MaxBet);
        }

        public string DoubleBetToString()
        {
            return AmountToString(CurrentBet * 2);
        }

        public string DoubleFeeToString()
        {
            return AmountToString(CurrentFee * 2);
        }

        public string DoubleAgainBetToString()
        {
            return AmountToString(CurrentBet * 4);
        }

        public string DoubleAgainFeeToString()
        {
            return AmountToString(CurrentFee * 4);
        }

        public float DoubleAgainBet()
        {
            return CurrentBet * 4;
        }

        public float DoubleAgainFee()
        {
            return CurrentFee * 4;
        }

        public override string ToString()
        {
            return AmountToString(CurrentBet) + " + " + AmountToString(CurrentFee) + " Max: " + AmountToString(MaxBet);
        }
    }
}
