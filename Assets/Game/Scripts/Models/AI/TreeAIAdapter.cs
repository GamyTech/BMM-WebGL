using GT.Backgammon.Logic;
using GT.Backgammon.Player;

namespace GT.Backgammon.AI
{
    public class TreeAIAdapter : IBackgammonAI
    {
        private int m_inputNeuronCount;
        private int m_outputNeuronCount;

        public int InputNeuronCount { get { return m_inputNeuronCount; } }
        public int OutputNeuronCount { get { return m_outputNeuronCount; } }

        private TreeNode<Move> m_root;
        private Board m_board;
        private PlayerColor m_currentPlayer;

        public TreeAIAdapter(int inputNeuronCount, int outputNeuronCount, TreeNode<Move> root, Board board, PlayerColor currentPlayer)
        {
            m_inputNeuronCount = inputNeuronCount;
            m_outputNeuronCount = outputNeuronCount;
            m_root = root;
            m_board = board;
            m_currentPlayer = currentPlayer;
        }

        #region Impelementations
        public IPath[] GetAllPaths()
        {
            return TreePath<Move>.GetAllPaths(m_root).ToArray();
        }

        public double[] GetInputAfterSimulation(IPath path)
        {
            Board tempBoard = new Board(m_board);
            tempBoard.MakeMove((path as TreePath<Move>).GetItemsFromPath());

            return GetTDGammonInput(tempBoard, m_currentPlayer);
        }

        public int CompareOutputs(double[] currentOutput, double[] bestOutput)
        {
            double current = GetOutputValue(m_currentPlayer, currentOutput);
            double best = GetOutputValue(m_currentPlayer, bestOutput);
            return current.CompareTo(best);
        }
        #endregion Impelementations

        private static double GetOutputValue(PlayerColor currentTurn, double[] array)
        {
            return currentTurn == PlayerColor.White ? array[0] - array[1] : array[1] - array[0];
        }

        private double[] GetTDGammonInput(Board board, PlayerColor currentTurnColor)
        {
            double[] input = new double[InputNeuronCount];
            for (int i = 0; i < Board.BOARD_SIZE; i++)
            {
                Slot slot = board.GetSlot(i);
                int idxToAdd = slot.SlotColor == SlotColor.White ? 0 : 4;
                input[i * 8 + idxToAdd + 0] = slot.Quantity >= 1 ? 1 : 0;
                input[i * 8 + idxToAdd + 1] = slot.Quantity >= 2 ? 1 : 0;
                input[i * 8 + idxToAdd + 2] = slot.Quantity >= 3 ? 1 : 0;
                input[i * 8 + idxToAdd + 3] = slot.Quantity > 3 ? (double)(slot.Quantity - 3) / 2 : 0;
            }

            // Current moving player
            input[192] = currentTurnColor == PlayerColor.White ? 1 : 0;
            input[193] = currentTurnColor == PlayerColor.Black ? 1 : 0;

            // Chackers on the bar (eaten)
            input[194] = (double)board.GetEatenSlot(PlayerColor.White).Quantity / 2;
            input[195] = (double)board.GetEatenSlot(PlayerColor.Black).Quantity / 2;

            // Checkers borne off
            input[196] = (double)board.GetBearoffSlot(PlayerColor.White).Quantity / 15;
            input[197] = (double)board.GetBearoffSlot(PlayerColor.Black).Quantity / 15;

            return input;
        }
    }
}
