using System.Collections.Generic;
using GT.Backgammon.Player;

namespace GT.Backgammon.Logic
{
    public static class GameFactory
    {
        enum GameType { Local, Remote, Replay }

        public static LocalGame CreateLocalGame(Stake stake, params IPlayer[] players)
        {
            LocalGame game = new LocalGame(stake, players);
            game.InitializationComplete();
            return game;
        }

        public static RemoteGame CreateRemoteGame(Dictionary<string, PlayerData> players, Enums.MatchKind kind, float bet, float fee, float maxBet, string cantDoubleId, int doublesCount, int matchId)
        {
            List<IPlayer> playersList = new List<IPlayer>();
            foreach (var player in players)
                playersList.Add(CreatePlayerFromData(player.Key, player.Value));

            Stake stake = new Stake(kind, bet, fee, maxBet, cantDoubleId, doublesCount);
            RemoteGame game = new RemoteGame(stake, matchId, playersList.ToArray());
            game.InitializationComplete();

            return game;
        }

        public static TourneyGame CreateTourneyGame(Dictionary<string, PlayerData> players, int maxDoubles, string cantDoubleId, int doublesCount, int matchId)
        {
            List<IPlayer> playersList = new List<IPlayer>();
            foreach (var player in players)
                playersList.Add(CreatePlayerFromData(player.Key, player.Value));

            Stake stake = new Stake(Enums.MatchKind.Virtual, 1 << doublesCount, 0, 1 << maxDoubles, cantDoubleId, doublesCount);
            TourneyGame game = new TourneyGame(stake, matchId, playersList.ToArray());
            game.InitializationComplete();

            return game;
        }

        private static IPlayer CreatePlayerFromData(string pId, PlayerData p)
        {
            IPlayer player;

            if (UserController.Instance != null && 
                UserController.Instance.gtUser != null && 
                pId.Equals(UserController.Instance.gtUser.Id))
                player = new HumanPlayer(pId, p.Color, p);
            else if (p.IsBot)
                player = new TDBotPlayer(pId, p.Color, p, 1f, 5f, 3000000);
            else
                player = new RemotePlayer(pId, p.Color, p);

            return player;
        }
    }
}
