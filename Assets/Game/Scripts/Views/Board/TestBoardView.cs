using UnityEngine;
using GT.Backgammon.Logic;
using UnityEngine.EventSystems;
using System;
using GT.Backgammon.Player;
using System.Collections;
using UnityEngine.UI;

namespace GT.Backgammon.View
{
    public delegate void EditBoardHandler(bool inEdit);

    public enum PlayDirection
    {
        CounterClockwiseRight = 0,
        CounterClockwiseLeft = 1,
        ClockwiseRight = 2,
        ClockwiseLeft = 3,
    }

    public class TestBoardView : BaseBoardView
    {
        public event EditBoardHandler OnEditBoard;

        public GameObject[] viewSlotObjects;
        public GameObject topBearoffObject;
        public GameObject bottomBearoffObject;
        public GameObject topEatenObject;
        public GameObject bottomEatenObject;

        public Text TopPipText;
        public Image TopPipImage;
        public Text BottomPipText;
        public Image BottomPipImage;
        public Text TurnCountText;

        public Transform bearoffContainer;

        protected WaitForSeconds waitBetweenMoves;

        private Func<int, int> convertIndexFunc;
        private Func<int, int> convertColorIndexFunc;
        private Func<PlayerColor, int> convertEatenIndexFunc;

        private ISlotView selectedSlot;
        private bool whitePerspective;

        private bool m_editMode;
        public bool editMode
        {
            get { return m_editMode; }
            set
            {
                m_editMode = value;
                if (OnEditBoard != null) OnEditBoard(m_editMode);
            }
        }

        #region Implementations
        public void InitView(float moveDuration, bool isWhiteBottom, PlayDirection dir)
        {
            InitIndexFunctions(dir, isWhiteBottom);
            InitView(moveDuration, isWhiteBottom);

        }

        public override void InitView(float moveDuration, bool isWhiteBottom)
        {
            m_editMode = false;
            waitBetweenMoves = new WaitForSeconds(moveDuration);
            whitePerspective = isWhiteBottom;

            Color topColor = whitePerspective ? Color.black : Color.white;
            TopPipImage.color = topColor;
            TopPipText.color = topColor.OppositeColor();
            BottomPipImage.color = topColor.OppositeColor();
            BottomPipText.color = topColor;

            m_viewSlots = new ISlotView[28];
            for (int i = 0; i < viewSlotObjects.Length; i++)
            {
                int index = i;
                m_viewSlots[i] = viewSlotObjects[i].GetComponent<ISlotView>();
                m_viewSlots[i].InitSlot(new TestSlotViewData(b => SlotClicked(b, index), i, i < 12));
            }

            m_viewSlots[24] = topBearoffObject.GetComponent<ISlotView>();
            m_viewSlots[24].InitSlot(new TestSlotViewData(b => SlotClicked(b, 24), 24, true));

            m_viewSlots[25] = bottomBearoffObject.GetComponent<ISlotView>();
            m_viewSlots[25].InitSlot(new TestSlotViewData(b => SlotClicked(b, 25), 25, false));

            m_viewSlots[26] = topEatenObject.GetComponent<ISlotView>();
            m_viewSlots[26].InitSlot(new TestSlotViewData(b => SlotClicked(b, 26), 26, true));

            m_viewSlots[27] = bottomEatenObject.GetComponent<ISlotView>();
            m_viewSlots[27].InitSlot(new TestSlotViewData(b => SlotClicked(b, 27), 27, false));

            //ClearBoard();
        }

        public override IEnumerator Move(Move move)
        {
            ClearIndicators();
            selectedSlot = null;

            int fromIndex = ConvertLogicToViewIndex(move.from);
            int toIndex = ConvertLogicToViewIndex(move.to);

            if (move.isEaten)
            {
                yield return waitBetweenMoves;
                int toEatenIndex = GetEatenIndex(Board.GetOpponentsColor(move.color));
                m_viewSlots[toIndex].RemoveChecker();
                m_viewSlots[toEatenIndex].AddChecker(Board.GetOpponentToSlotColor(move.color));
            }
            yield return waitBetweenMoves;
            m_viewSlots[fromIndex].RemoveChecker();
            m_viewSlots[toIndex].AddChecker(Board.GetPlayerToSlotColor(move.color));
        }

        public override IEnumerator Undo(Move move)
        {
            ClearIndicators();
            selectedSlot = null;

            int fromIndex = ConvertLogicToViewIndex(move.from);
            int toIndex = ConvertLogicToViewIndex(move.to);

            yield return waitBetweenMoves;
            m_viewSlots[toIndex].RemoveChecker();
            m_viewSlots[fromIndex].AddChecker(Board.GetPlayerToSlotColor(move.color));
            if (move.isEaten)
            {
                yield return waitBetweenMoves;
                int fromEatenIndex = GetEatenIndex(Board.GetOpponentsColor(move.color));
                m_viewSlots[fromEatenIndex].RemoveChecker();
                m_viewSlots[toIndex].AddChecker(Board.GetOpponentToSlotColor(move.color));
            }
        }

        public override void TurnStarted(int turnCount, int whitePip, int blackPip)
        {
            base.TurnStarted(turnCount, whitePip, blackPip);
            selectedSlot = null;

            BottomPipText.text = whitePerspective ? whitePip.ToString() : blackPip.ToString();
            TopPipText.text = whitePerspective ? blackPip.ToString() : whitePip.ToString();

            int pipDif = whitePip - blackPip;
            TurnCountText.text = "Turn: " + turnCount + " | " + pipDif;
        }

        protected override int ConvertLogicToViewIndex(int logicIndex)
        {
            return convertIndexFunc(convertColorIndexFunc(logicIndex));
        }

        protected override int ConvertViewToLogicIndex(int viewIndex)
        {
            return convertColorIndexFunc(convertIndexFunc(viewIndex));
        }

        protected override int GetEatenIndex(PlayerColor color)
        {
            return convertIndexFunc(convertEatenIndexFunc(color));
        }
        #endregion Implementations

        #region Private Methods
        public void InitIndexFunctions(PlayDirection dir, bool whitePerspective)
        {
            SetColor(whitePerspective);
            SetEaten(whitePerspective);
            SetDirection((int)dir);
        }

        private void ClearBoard()
        {
            for (int i = 0; i < m_viewSlots.Length; i++)
            {
                m_viewSlots[i].ResetSlotView();
            }
        }

        private void SetColor(bool whitePerspective)
        {
            if (whitePerspective)
                convertColorIndexFunc = ConvertToWhiteIndex;
            else
                convertColorIndexFunc = ConvertToBlackIndex;
        }

        private void SetDirection(int dir)
        {
            SetBearoffSide(dir == 0 || dir == 3);
            switch (dir)
            {
                case 1:
                    convertIndexFunc = ConvertIndexToCCWL;
                    break;
                case 2:
                    convertIndexFunc = ConvertIndexToCWL;
                    break;
                case 3:
                    convertIndexFunc = ConvertIndexToCWR;
                    break;
                default:
                    convertIndexFunc = ConvertIndexToCCWR;
                    break;
            }
        }

        private void SetEaten(bool whitePerspective)
        {
            if (whitePerspective)
                convertEatenIndexFunc = c => { return Board.GetIndexOfEaten(c); };
            else
                convertEatenIndexFunc = c => { return Board.GetIndexOfEatenOpponent(c); };
        }

        private int GetDir(bool clockWise, bool bearoffLeft)
        {
            int dir;
            if (clockWise)
            {
                if (bearoffLeft)
                {
                    dir = 2;
                }
                else
                {
                    dir = 3;
                }
            }
            else
            {
                if (bearoffLeft)
                {
                    dir = 1;
                }
                else
                {
                    dir = 0;
                }
            }
            return dir;
        }

        private void SetBearoffSide(bool right)
        {
            if (right)
            {
                bearoffContainer.SetAsLastSibling();
            }
            else
            {
                bearoffContainer.SetAsFirstSibling();
            }
        }

        private SlotColor GetNextColor(SlotColor color)
        {
            switch (color)
            {
                case SlotColor.White:
                    return SlotColor.Black;
                case SlotColor.Black:
                    return SlotColor.Empty;
                default:
                    return SlotColor.White;
            }
        }

        private void EditInputHandling(ISlotView slot, PointerEventData button)
        {
            if (button.scrollDelta.y > 0)
            {
                // add checker to index
                slot.Quantity++;
            }
            else if (button.scrollDelta.y < 0)
            {
                // remove checker from index
                slot.Quantity--;
            }
            else
            {
                switch (button.button)
                {
                    case PointerEventData.InputButton.Left:
                        // change color to black
                        if (slot.SlotColor != SlotColor.Black)
                        {
                            slot.SlotColor = SlotColor.Black;
                        }
                        else if (slot.SlotColor == SlotColor.Black)
                        {
                            slot.SlotColor = SlotColor.Empty;
                        }
                        break;
                    case PointerEventData.InputButton.Right:
                        // change color to white
                        if (slot.SlotColor != SlotColor.White)
                        {
                            slot.SlotColor = SlotColor.White;
                        }
                        else if (slot.SlotColor == SlotColor.White)
                        {
                            slot.SlotColor = SlotColor.Empty;
                        }
                        if (slot.Quantity == 0) slot.Quantity = 1;
                        break;
                    case PointerEventData.InputButton.Middle:
                        // clear slot
                        slot.Quantity = 0;
                        break;
                }
            }

            if (slot.Quantity == 0)
            {
                slot.SlotColor = SlotColor.Empty;
            }
            else if (slot.SlotColor == SlotColor.Empty)
            {
                slot.SlotColor = SlotColor.Black;
            }
        }
        #endregion Private Methods

        #region Convert From Logic Index To View Index Functions
        private int ConvertToBlackIndex(int index)
        {
            switch (index)
            {
                case 24: return 25;
                case 25: return 24;
                case 26: return 27;
                case 27: return 26;
                default: return index;
            }
        }

        private int ConvertToWhiteIndex(int index)
        {
            switch (index)
            {
                case 24: return 24;
                case 25: return 25;
                case 26: return 26;
                case 27: return 27;
                default: return 23 - index;
            }
        }

        /// <summary>
        /// Clockwise + Right Bearoff
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int ConvertIndexToCWR(int index)
        {
            switch (index)
            {
                case 24: return 24;
                case 25: return 25;
                case 26: return 27;
                case 27: return 26;
                default: return 23 - index;
            }
        }

        /// <summary>
        /// Counter Clockwise + Right Bearoff
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int ConvertIndexToCCWR(int index)
        {
            switch (index)
            {
                case 24: return 25;
                case 25: return 24;
                case 26: return 26;
                case 27: return 27;
                default: return index;
            }
        }

        /// <summary>
        /// Clockwise + Left Bearoff
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int ConvertIndexToCWL(int index)
        {
            if (index < 6)
            {
                return index + 12;
            }
            else if (index < 12)
            {
                return index + 12;
            }
            else if (index < 18)
            {
                return index - 12;
            }
            else if (index < 24)
            {
                return index - 12;
            }
            switch (index)
            {
                case 24: return 24;
                case 25: return 25;
                case 26: return 27;
                case 27: return 26;
            }
            return -1;
        }

        /// <summary>
        /// Counter Clockwise + Left Bearoff
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int ConvertIndexToCCWL(int index)
        {
            if (index < 6)
            {
                return 11 - index;
            }
            else if (index < 12)
            {
                return 11 - index;
            }
            else if (index < 18)
            {
                return 35 - index;
            }
            else if (index < 24)
            {
                return 35 - index;
            }
            switch (index)
            {
                case 24: return 25;
                case 25: return 24;
                case 26: return 26;
                case 27: return 27;
            }
            return -1;
        }
        #endregion Convert From Logic Index To View Index Functions

        #region Input
        public void SlotClicked(BaseEventData button, int index)
        {
            ISlotView slot = m_viewSlots[index];
            if (editMode)
            {
                if (button is PointerEventData)
                {
                    EditInputHandling(slot, button as PointerEventData);
                }
            }
            else
            {
                if (selectedSlot == null)
                {
                    if (possibleMovesDict.ContainsKey(slot.LogicIndex))
                    {
                        ClearIndicators();
                        selectedSlot = slot;
                        selectedSlot.SetIndicator(Color.cyan);

                        ActivateIndicators(possibleMovesDict[selectedSlot.LogicIndex], Color.blue);
                    }
                }
                else
                {
                    ClearIndicators();
                    if (possibleMovesDict[selectedSlot.LogicIndex].Contains(slot.LogicIndex))
                    {
                        possibleMovesDict = null;

                        TriggerExecuteMove(selectedSlot.LogicIndex, slot.LogicIndex);
                    }
                    else
                    {
                        ActivateIndicators(possibleMovesDict.Keys, Color.green);
                    }
                    selectedSlot = null;
                }
            }
        }
        #endregion Input

        #region Static Methods
        public static PlayDirection NextPlayDirection(PlayDirection playDirection)
        {
            switch (playDirection)
            {
                case PlayDirection.CounterClockwiseRight:
                    return PlayDirection.ClockwiseLeft;
                case PlayDirection.ClockwiseLeft:
                    return PlayDirection.CounterClockwiseLeft;
                case PlayDirection.CounterClockwiseLeft:
                    return PlayDirection.ClockwiseRight;
                case PlayDirection.ClockwiseRight:
                    return PlayDirection.CounterClockwiseRight;
                default:
                    return PlayDirection.CounterClockwiseRight;
            }
        }
        #endregion Static Methods
    }
}
