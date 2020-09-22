using GT.Backgammon.Logic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GT.Backgammon.Player
{
    public abstract class LocalPlayer : BasePlayer, ILocalPlayer
    {
        private bool canDouble;

        // Tree of all possible moves this turn
        protected TreeNode<Move> m_possibleMoves;
        public TreeNode<Move> possibleMoves
        {
            get { return m_possibleMoves; }
        }

        public abstract Move[] movesLastTurn { get; }
        public abstract int eatenLastTurn { get; }

        public LocalPlayer(string id, PlayerColor color) : base(id, color)
        {
            canDouble = false;
            m_possibleMoves = null;
        }

        public LocalPlayer(string id, PlayerColor color, PlayerData data) : base(id, color, data)
        {
            m_possibleMoves = null;
        }

        protected override void ClearTurn()
        {
            base.ClearTurn();
            m_possibleMoves = null;
        }

        public override void SetDice(int first, int second, Board board)
        {
            dice.SetDice(first, second);
            CalculatePossibleMoves(board);

            DiceSetEvent(dice);
        }

        public virtual void SetCanDouble(bool isDouble)
        {
            canDouble = isDouble;
        }

        public virtual bool CanDouble()
        {
            if (dice.IsRolled)
                return false;
            return canDouble;
        }

        protected virtual void CalculatePossibleMoves(Board board)
        {
            m_possibleMoves = GetAllMovesTree(board, m_color, dice.GetListOfDice());
        }

        #region Possible Movement
        private TreeNode<Move> GetAllMovesTree(Board board, PlayerColor color, int[] dice)
        {
            TreeNode<Move> root = new TreeNode<Move>(new Move(0, 0, color, 0, false));
            GetMovesTreeFromSlot(root, board, color, dice);

            //remove illigal moves ( play one number in such a way as to avoid playing the other, must play the higher one )
            RemoveIlligalMoves(root);

            return root;
        }

        private void GetMovesTreeFromSlot(TreeNode<Move> rootNode, Board board, PlayerColor color, int[] dice)
        {
            if (dice.Length == 0) return;
  
            List<Slot> slots = board.GetSlotsOccupiedByColor(color);
            for (int i = 0; i < slots.Count; i++)
            {
                int fromIndex = slots[i].Index;
                int[] uniqueDice = ((int[])dice.Clone()).Distinct().ToArray();
                for (int j = 0; j < uniqueDice.Length; j++)
                {
                    int toIndex = Board.DieToIndex(fromIndex, uniqueDice[j], color);
                    if (board.isMoveLegal(fromIndex, toIndex, color, uniqueDice[j]) == MoveCheck.Success)
                    {
                        Move move = CreateMove(board, fromIndex, toIndex, color, uniqueDice[j]);
                        TreeNode<Move> newNode = new TreeNode<Move>(move);
                        rootNode.AddChild(newNode);

                        Board newBoard = new Board(board);
                        newBoard.MakeMove(move);
                        List<int> unusedDice = new List<int>(dice);
                        unusedDice.Remove(uniqueDice[j]);
                        GetMovesTreeFromSlot(newNode, newBoard, color, unusedDice.ToArray());
                    }
                }
            }
        }

        private void RemoveIlligalMoves(TreeNode<Move> root)
        {
            int maxDepth = root.MaxDepth();
            root.RemoveShallowBranches(maxDepth);

            if(maxDepth == 1)
            {
                int maxDice = 0;
                for (int i = 0; i < root.children.Count; i++)
                {
                    maxDice = Mathf.Max(maxDice, root.children[i].Item.dice);
                }
                for (int i = root.children.Count-1; i >=0; i--)
                {
                    if(root.children[i].Item.dice < maxDice)
                    {
                        root.RemoveChild(root.children[i]);
                    }
                }
            }
        }

        private Move CreateMove(Board board, int from, int to, PlayerColor color, int die)
        {
            return new Move(from, to, color, die, board.GetSlot(to).IsOccupiedBy(Board.GetOpponentsColor(color)));
        }

        protected int GetEatenOnPath(TreePath<Move> path)
        {
            int count = 0;
            for (int i = 0; i < path.nodeList.Count; i++)
            {
                if (path.nodeList[i].Item.isEaten)
                    count++;
            }
            return count;
        }
        #endregion Possible Movement
    }
}
