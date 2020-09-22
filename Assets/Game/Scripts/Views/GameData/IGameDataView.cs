using GT.Backgammon.Player;

namespace GT.Backgammon.View
{
    public interface IGameDataView
    {
        void InitGameData(params IPlayer[] players);

        void SetBetData(string bet, string fee, string maxBet, int cubeNum, string cubeHolder);

        void SetPlayersData(params IPlayer[] players);

        void ResetView();
    }
}
