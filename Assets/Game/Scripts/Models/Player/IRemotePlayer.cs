using GT.Backgammon.Logic;

namespace GT.Backgammon.Player
{
    public interface IRemotePlayer 
    {
        void ReceivedMove(Board board, string receivedData);
    }
}
