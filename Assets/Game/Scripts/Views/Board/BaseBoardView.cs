using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GT.Backgammon.Logic;
using GT.Backgammon.Player;

namespace GT.Backgammon.View
{
    public abstract class BaseBoardView : MonoBehaviour, IBoardView
    {
        #region Delegates And Events
        public event View_ExecuteMoveHandler OnView_ExecuteMove;
        #endregion Delegates And Events

        protected Dictionary<int, HashSet<int>> possibleMovesDict;

        protected ISlotView[] m_viewSlots;
        public ISlotView[] viewSlots
        {
            get { return m_viewSlots; }
        }

        protected abstract int ConvertLogicToViewIndex(int index);
        protected abstract int ConvertViewToLogicIndex(int index);
        protected abstract int GetEatenIndex(PlayerColor color);

        #region IBoardView Implementations
        public abstract void InitView(float moveDuration, bool isWhiteBottom);
        public abstract IEnumerator Move(Move move);
        public abstract IEnumerator Undo(Move move);

        public virtual void ShowBoardState(string boardString)
        {
            List<Slot> slots = Board.Deserialize(boardString);
            for (int i = 0; i < slots.Count; i++)
            {
                int viewIndex = ConvertLogicToViewIndex(i);
                m_viewSlots[viewIndex].SetSlotView(i, slots[i].SlotColor, slots[i].Quantity);
            }
        }

        public virtual void SetMovesDict(Dictionary<int, HashSet<int>> movesDict)
        {
            possibleMovesDict = movesDict;
            if (movesDict == null)
                return;
            
        }

        public virtual void ShowMovesIndicators()
        {
            if (possibleMovesDict != null)
                ActivateIndicators(possibleMovesDict.Keys, Color.green);
        }

        public virtual void TurnStarted(int turnCount, int whitePip, int blackPip)
        {
            ClearIndicators();
        }

        #endregion IBoardView Implementations

        #region Protected Methods
        protected virtual void ClearIndicators()
        {
            for (int i = 0; i < m_viewSlots.Length; i++)
                m_viewSlots[i].ResetIndicator();
        }

        /// <summary>
        /// Activate indicators
        /// </summary>
        /// <param name="collection">Collection of logic indexes</param>
        /// <param name="color"></param>
        protected virtual void ActivateIndicators(IEnumerable<int> collection, Color color)
        {
            foreach (var item in collection)
                m_viewSlots[ConvertLogicToViewIndex(item)].SetIndicator(color);
        } 
        #endregion Protected Methods

        #region Trigger View Events
        protected void TriggerExecuteMove(int from, int to)
        {
            if (OnView_ExecuteMove != null)
                OnView_ExecuteMove(from, to);
        }
        #endregion Trigger View Events

        public string GetBoardString()
        {
            Slot[] slots = new Slot[Board.MAX_SLOTS];
            for (int i = 0; i < viewSlots.Length; i++)
                slots[viewSlots[i].LogicIndex] = new Slot(viewSlots[i].LogicIndex, viewSlots[i].Quantity, viewSlots[i].SlotColor);
            return Board.Serialize(new List<Slot>(slots));
        }
    }
}
