using UnityEngine;
using System.Collections;
using GT.Backgammon.Logic;
using System;

namespace GT.Backgammon.Player
{
    public abstract class BaseAIPlayer : LocalPlayer, IAIPlayer
    {
        public abstract string AIDescription { get; }

        protected float minWaitTime;
        protected float maxWaitTime;

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

        public BaseAIPlayer(string id, PlayerColor color) : base(id, color) { }

        /// <summary>
        /// Constructor that allows ai to wait between actions
        /// </summary>
        /// <param name="id"></param>
        /// <param name="color"></param>
        /// <param name="data"></param>
        /// <param name="mono"></param>
        public BaseAIPlayer(string id, PlayerColor color, PlayerData data, float minWaitTime, float maxWaitTime) : base(id, color, data)
        {
            this.minWaitTime = minWaitTime;
            this.maxWaitTime = maxWaitTime;
        }

        public override void SetDice(int first, int second, Board board)
        {
            base.SetDice(first, second, board);
            if (ContentController.Instance != null)
                ContentController.Instance.StartCoroutine(StartTurnRoutine(board));
            else
                StartTurnInstant(board);
        }

        protected abstract Move[] GetBestTurn(Board board, out int eaten);
        protected abstract bool ShouldDouble(Board board);

        public override void DoubleResponse(DoubleResponse response)
        {
            OnDoubleResponseEvent(response);
        }

        private void StartTurnInstant(Board board)
        {
            // choose best move
            m_movesLastTurn = GetBestTurn(board, out m_eatenLastTurn);

            // if should double initialize double else roll dice
            if (CanDouble() && ShouldDouble(board))
            {
                OnDoubleInitiatedEvent();
                return;
            }
            else
            {
                // Roll dice
                RollDice();
            }

            // execute moves
            ExecuteMoves(board, m_movesLastTurn);

            // end turn
            EndTurn();
        }

        private IEnumerator StartTurnRoutine(Board board)
        {
            // choose best move
            m_movesLastTurn = GetBestTurn(board, out m_eatenLastTurn);

            // ---- Put time delay here ----
            yield return WaitRandomTime();

            // if should double initialize double else roll dice
            if (CanDouble() && ShouldDouble(board))
            {
                OnDoubleInitiatedEvent();
                yield break;
            }
            else
            {
                // Roll dice
                RollDice();
            }

            // ---- Put time delay here ----
            yield return WaitRandomTime();

            // execute moves
            ExecuteMoves(board, m_movesLastTurn);

            // end turn
            EndTurn();
        }

        protected IEnumerator WaitRandomTime()
        {
            float waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
