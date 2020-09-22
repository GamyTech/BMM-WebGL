using UnityEngine;
using System.Collections;
using GT.Backgammon.Player;

namespace GT.Backgammon.Logic
{
    public class BetChangedEventArgs
    {
        public bool IsRequest { get; protected set; }
        public Enums.MatchKind Kind { get; protected set; }
        public string CurrentPostFix { get; protected set; }
        public float CurrentBet { get; protected set; }
        public float CurrentFee { get; protected set; }
        public float MaxBet { get; protected set; }
        public int CubeNum { get; protected set; }
        public IPlayer CantDoublePlayer { get; protected set; }

        public BetChangedEventArgs(bool isRequest, Enums.MatchKind kind, float currentBet, float currentFee, float maxBet, int cubeNum, IPlayer cantDoublePlayer)
        {
            Kind = kind;
            CurrentPostFix = Wallet.MatchKindToPrefix(kind);

            IsRequest = isRequest;
            CurrentBet = currentBet;
            CurrentFee = currentFee;
            MaxBet = maxBet;
            CubeNum = cubeNum;
            CantDoublePlayer = cantDoublePlayer;
        }
    }
}
