using GT.Backgammon.Player;

namespace GT.Backgammon.View
{
    public interface ITimersView
    {
        void InitView(bool isWhiteBottom);

        void SetTimer(PlayerColor color, PlayerData data);
    }
}
