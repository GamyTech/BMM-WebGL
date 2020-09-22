using UnityEngine;
using GT.Backgammon.Logic;
using System.Collections;

namespace GT.Backgammon.Player
{
    public class TDBotPlayer : TDGammonPlayer, IRemotePlayer
    {
        private static int MAX_DOUBLES_COUNT = 0;

        private Board tempBoard;

        public override string AIDescription { get { return "TDBot"; } }

        private int doublesInGame;

        public TDBotPlayer(string id, PlayerColor color, int strength) : base(id, color, strength)
        {
        }

        public TDBotPlayer(string id, PlayerColor color, PlayerData data, float minWaitTime, float maxWaitTime, int strength)
            : base(id, color, data, minWaitTime, maxWaitTime, strength)
        {
        }

        public override void SetDice(int first, int second, Board board)
        {
            dice.SetDice(first, second);
            CalculatePossibleMoves(board);

            if (RemoteGameController.Instance != null)
                RemoteGameController.Instance.StartCoroutine(StartTurnRoutine(board));
            else
                StartTurnInstant(board);
        }

        public override void DoubleResponse(DoubleResponse response)
        {
            SendDoubleResponse(response);
        }

        public override void RequestedDouble(Board board)
        {
            tempBoard = board;
            if (RemoteGameController.Instance != null)
                RemoteGameController.Instance.StartCoroutine(RequestedDoubleRoutine(board));
            else
                RequestedDoubleInstant(board);
        }

        public virtual void ReceivedMove(Board board, string receivedData)
        {
            // deserialize to dice and moves,
            Move[] moves = Move.DeserializeMoves(receivedData);

            // roll dice
            //RollDice();

            // execute moves
            ExecuteMoves(board, moves);

            // end turn
            EndTurn();
        }

        private void StartTurnInstant(Board board)
        {
            // choose best move
            m_movesLastTurn = GetBestTurn(board, out m_eatenLastTurn);

            // if should double initialize double else roll dice
            if (CanDouble() && ShouldDouble(board) && IsAllowedToDouble())
            {
                SendDoubleRequest();
                return;
            }
            else
            {
                // Roll dice
                SendRoll();
            }

            // Send Turn
            SendTurn(board, m_eatenLastTurn, m_movesLastTurn);
        }

        private IEnumerator StartTurnRoutine(Board board)
        {
            // choose best move
            m_movesLastTurn = GetBestTurn(board, out m_eatenLastTurn);

            // ---- Put time delay here ----
            yield return WaitRandomTime();

            // if should double initialize double else roll dice
            if (CanDouble() && ShouldDouble(board) && IsAllowedToDouble())
            {
                GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.LastMatchBotDoublesCount, ++doublesInGame);
                SendDoubleRequest();
                yield break;
            }
            else
            {
                // Roll dice
                SendRoll();
            }

            // ---- Put time delay here ----
            yield return WaitRandomTime();

            // Send Turn
            SendTurn(board, m_eatenLastTurn, m_movesLastTurn);
        }

        private bool IsAllowedToDouble()
        {
            doublesInGame = GTDataManagementKit.GetIntFromPrefs(Enums.PlayerPrefsVariable.LastMatchBotDoublesCount, 0);
            return doublesInGame < MAX_DOUBLES_COUNT;
        }

        #region Sending Data
        private void SendRoll()
        {
            RequestInGameController.Instance.SendRoll(playerData.RollingItems);
        }

        private void SendTurn(Board board, int eaten, params Move[] moves)
        {
            Board newBoard = new Board(board);
            newBoard.MakeMove(moves);
            RequestInGameController.Instance.SendMove(moves, eaten, newBoard, true);
        }

        private void SendDoubleRequest()
        {
            RequestInGameController.Instance.SendDoubleRequest(playerColor.ToString(), true);
        }

        private void SendDoubleResponse(DoubleResponse response)
        {
            RequestInGameController.Instance.SendDoubleResponse(response, playerColor, tempBoard, true);
        }
        #endregion Sending Data
    }
}
