using UnityEngine;
using GT.Backgammon;

namespace GT.Database
{
    public class GetServerMatchHistoryMoves : GlobalServerResponseBase
    {
        public ReplayMatchData MatchData { get; private set; }

        public GetServerMatchHistoryMoves(WWW www) : base(www)
        {
            if(ResponseDict != null)
                MatchData = new ReplayMatchData(ResponseDict);
        }
    }
}
