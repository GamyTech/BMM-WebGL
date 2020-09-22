using System;
using System.Collections.Generic;
using GT.Backgammon.Logic;
using GT.Store;

namespace GT.Backgammon.Player
{
    public class HumanPlayer : LocalPlayer, IHumanPlayer
    {
        public event MoveDoneHandler OnUndoDone;

        // Previous moves this turn
        private Stack<TreeNode<Move>> undoNodes;
        // Current node on the tree of possible moves
        private TreeNode<Move> currentNode;

        protected Move[] m_movesLastTurn;
        public override Move[] movesLastTurn
        {
            get { return m_movesLastTurn; }
        }

        protected int m_eatenLastTurn;
        public override int eatenLastTurn
        {
            get { return m_eatenLastTurn; }
        }

        public HumanPlayer(string id, PlayerColor color, PlayerData data) : base(id, color, data)
        {
            undoNodes = new Stack<TreeNode<Move>>();
            ClearTurn();
        }

        #region Overrides
        public override void EndTurn()
        {
            TreePath<Move> path = new TreePath<Move>(possibleMoves, currentNode);
            m_movesLastTurn = path.GetReversedItemsFromPath();
            m_eatenLastTurn = GetEatenOnPath(path);

            ClearTurn();
            base.EndTurn();
        }

        protected override void CalculatePossibleMoves(Board board)
        {
            base.CalculatePossibleMoves(board);
            currentNode = m_possibleMoves;
        }

        protected override void ClearTurn()
        {
            currentNode = null;
            undoNodes.Clear();
            base.ClearTurn();
        }
        #endregion Overrides

        #region IHumanPlayer Implementation
        public bool CanRoll()
        {
            return dice.IsRolled == false;
        }

        public bool CanUndo()
        {
            return currentNode != null && currentNode.Parent != null;
        }

        public bool IsFinishedMoving()
        {
            return currentNode != null && currentNode.children.Count == 0;
        }

        public Dictionary<int, HashSet<int>> GetPossibleMoves()
        {
            Dictionary<int, HashSet<int>> dict = new Dictionary<int, HashSet<int>>();
            for (int i = 0; i < currentNode.children.Count; i++)
            {
                TreeNode<Move> child = currentNode.children[i];
                HashSet<int> destSet = GetAllDestinationsFromNode(child);
                if (dict.ContainsKey(child.Item.from))
                {
                    dict[child.Item.from].UnionWith(destSet);
                }
                else
                {
                    dict.Add(child.Item.from, destSet);
                }
            }
            return dict;
        }


        public List<Move> GetPossibleMovesFrom(int from)
        {
            List<Move> list = new List<Move>();

            for (int i = 0; i < currentNode.children.Count; i++)
            {
                TreeNode<Move> child = currentNode.children[i];

                if(child.Item.from == from)
                {
                    list.Add(child.Item);
                }
            }
            return list;
        }

        public void MakeBestMove(Board board, int from)
        {
            MakeMove(board, from, GetBestDestinationIndexFrom(from));
        }

        public void MakeMove(Board board, int from, int to)
        {
            if (currentNode == null)
                throw new FailedToMoveException(from, to);

            List<TreePath<Move>> paths = new List<TreePath<Move>>();
            for (int i = 0; i < currentNode.children.Count; i++)
                if (currentNode.children[i].Item.from == from)
                    paths.AddRange(GetAllPathsToDestination(currentNode.children[i], to));

            if (paths.Count == 0)
            {
                //throw new FailedToMoveException(from, to);
                return;
            }

            // get best path (min steps) then (highest eaten)
            TreePath<Move> chosenPath = paths[0];
            int maxEaten = GetEatenOnPath(chosenPath);
            for (int i = 1; i < paths.Count; i++)
            {
                int tempEaten = GetEatenOnPath(paths[i]);
                if(chosenPath.nodeList.Count > paths[i].nodeList.Count)
                {
                    chosenPath = paths[i];
                    maxEaten = tempEaten;
                }
                else if (maxEaten < tempEaten)
                {
                    chosenPath = paths[i];
                    maxEaten = tempEaten;
                }
            }
            undoNodes.Push(currentNode);
            currentNode = chosenPath.GetLastNode();

            Move[] moves = chosenPath.GetItemsFromPath();
            ExecuteMoves(board, moves);
        }

        public void MakeUndo(Board board)
        {
            if (undoNodes.Count > 0)
            {
                TreeNode<Move> destNode = undoNodes.Pop();
                TreePath<Move> undoPath = new TreePath<Move>(destNode, currentNode);
                Move[] moves = undoPath.GetItemsFromPath();
                board.MakeUndoMove(moves);

                currentNode = destNode;

                if (OnUndoDone != null)
                    OnUndoDone(moves);
            }
        }
        #endregion IHumanPlayer Implementation

        #region Private Methods
        protected int GetBestDestinationIndexFrom(int from)
        {
            int toIndex = 0;
            int maxDice = 0;

            for (int i = 0; i < currentNode.children.Count; i++)
            {
                TreeNode<Move> child = currentNode.children[i];
                if (child.Item.from == from)
                {
                    if (child.Item.dice > maxDice)
                    {
                        maxDice = child.Item.dice;
                        toIndex = child.Item.to;
                    }
                }
            }
            return toIndex;
        }

        protected HashSet<int> GetAllDestinationsFromNode(TreeNode<Move> node)
        {
            HashSet<int> set = new HashSet<int>();
            set.Add(node.Item.to);
            if (node.children.Count > 0)
            {
                for (int i = 0; i < node.children.Count; i++)
                {
                    if (node.children[i].Item.from == node.Item.to)
                    {
                        set.UnionWith(GetAllDestinationsFromNode(node.children[i]));
                    }
                }
            }
            return set;
        }

        protected List<TreePath<Move>> GetAllPathsToDestination(TreeNode<Move> startNode, int destIndex)
        {
            List<TreePath<Move>> paths = new List<TreePath<Move>>();

            if (startNode.Item.to == destIndex)
            {
                paths.Add(new TreePath<Move>(startNode));
            }
            else
            {
                List<TreeNode<Move>> destinationNodes = GetAllDestinationNodes(startNode, destIndex);
                for (int i = 0; i < destinationNodes.Count; i++)
                {
                    TreePath<Move> path = new TreePath<Move>(startNode, destinationNodes[i]);
                    path.nodeList.Add(startNode);
                    path.nodeList.Reverse();
                    paths.Add(path);
                }
            }
            return paths;
        }

        protected List<TreeNode<Move>> GetAllDestinationNodes(TreeNode<Move> startNode, int destIndex)
        {
            List<TreeNode<Move>> list = new List<TreeNode<Move>>();

            if (startNode.children.Count > 0)
            {
                for (int i = 0; i < startNode.children.Count; i++)
                {
                    if (startNode.children[i].Item.from == startNode.Item.to)
                    {
                        if (startNode.children[i].Item.to == destIndex)
                        {
                            list.Add(startNode.children[i]);
                            break;
                        }
                        else
                        {
                            list.AddRange(GetAllDestinationNodes(startNode.children[i], destIndex));
                        }
                    }
                }
            }

            return list;
        }
        #endregion Private Methods
    }
}
