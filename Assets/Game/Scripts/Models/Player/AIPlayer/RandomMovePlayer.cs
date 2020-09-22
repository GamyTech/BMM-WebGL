using GT.Backgammon.Logic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GT.Backgammon.Player
{
    public class RandomMovePlayer : BaseAIPlayer
    {
        public override string AIDescription
        {
            get
            {
                return "Random Move AI";
            }
        }

        public RandomMovePlayer(string id, PlayerColor color) : base(id, color) { }

        public RandomMovePlayer(string id, PlayerColor color, PlayerData data, float minWaitTime, float maxWaitTime) 
            : base(id, color, data, minWaitTime, maxWaitTime) { }


        protected override Move[] GetBestTurn(Board board, out int eaten)
        {
            List<TreePath<Move>> paths = TreePath<Move>.GetAllPaths(possibleMoves);
            TreePath<Move> randomPath = paths[Random.Range(0, paths.Count)];
            eaten = GetEatenOnPath(randomPath);
            return randomPath.GetItemsFromPath();
        }

        protected override bool ShouldDouble(Board board)
        {
            return false;
        }

        public override void RequestedDouble(Board board)
        {
            base.RequestedDouble(board);
            DoubleResponse(Logic.DoubleResponse.GiveUp);
        }
    }
}
