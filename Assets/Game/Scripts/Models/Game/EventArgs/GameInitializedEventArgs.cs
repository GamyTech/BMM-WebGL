using GT.Backgammon.Player;

namespace GT.Backgammon.Logic
{
    public class GameInitializedEventArgs
    {
        private IGame m_game;
        public virtual IGame game
        {
            get { return m_game; }
        }

        private IPlayer[] m_players;
        public virtual IPlayer[] players
        {
            get { return m_players; }
        }

        public GameInitializedEventArgs(IGame game, IPlayer[] players)
        {
            m_game = game;
            m_players = players;
        }
    }
}
