using System.Collections.Generic;
using GT.Backgammon.Player;

namespace GT.Backgammon.Logic
{
    public enum MoveCheck
    {
        Success, NotYourTurn, FromBearoffSlot, FromSlotNull, FromSlotEmpty, MovingOpponentsColor, ToSlotNull,
        ToSlotHasMaxCheckers, EatenLocked, BearoffLocked, OpponentBlockedEat, MovingToOpponentsEatenOrBearoff,
        WrongDirection, BearoffPriority
    }

    public class Board 
    {
        #region Constant Variables
        public const string STARTING_BOARD_STRING = "b0000E0C000eE000c0e0000B0000";
        //public const string STARTING_BOARD_STRING = "AA000000000000000000000aMn00";

        public const int MAX_SLOTS = 28;
        public const int BOARD_SIZE = 24;
        public const int MAX_IN_SLOT = 15;
        public const int EATEN_WHITE_INDEX = 26;
        public const int EATEN_BLACK_INDEX = 27;
        public const int BEAROFF_WHITE_INDEX = 24;
        public const int BEAROFF_BLACK_INDEX = 25;
        public const char WHITE_START_CHAR = 'A';
        public const char WHITE_END_CHAR = 'O';
        public const char BLACK_START_CHAR = 'a';
        public const char BLACK_END_CHAR = 'o';
        public const char EMPTY_CHAR = '0';
        #endregion Constant Variables

        // 0-23 game checkers.
        // 24 white beared off,
        // 25 black beared off,
        // 26 white eaten,
        // 27 black eaten,
        // white moves 23 to 0
        // black moves 0 to 23

        private List<Slot> slotList;

        #region Constructors
        /// <summary>
        /// Default constractor
        /// Will create a starting board
        /// </summary>
        public Board() : this(STARTING_BOARD_STRING) { }

        /// <summary>
        /// Constractor using serialized board string
        /// </summary>
        /// <param name="board"></param>
        public Board(string board)
        {
            if(string.IsNullOrEmpty(board))
            {
                board = STARTING_BOARD_STRING;
            }
            slotList = Deserialize(board);
        }

        /// <summary>
        /// Copy constractor
        /// </summary>
        /// <param name="board"></param>
        public Board(Board board)
        {
            slotList = Deserialize(Serialize(board));
        }
        #endregion Constructors

        #region Getters
        public Slot GetSlot(int index)
        {
            if (index < 0 || index >= MAX_SLOTS)
                return null;

            return slotList[index];
        }

        public Slot GetEatenSlot(PlayerColor color)
        {
            switch (color)
            {
                case PlayerColor.White:
                    return slotList[EATEN_WHITE_INDEX];
                case PlayerColor.Black:
                    return slotList[EATEN_BLACK_INDEX];
                default:
                    return null;
            }
        }

        public Slot GetBearoffSlot(PlayerColor color)
        {
            switch (color)
            {
                case PlayerColor.White:
                    return slotList[BEAROFF_WHITE_INDEX];
                case PlayerColor.Black:
                    return slotList[BEAROFF_BLACK_INDEX];
                default:
                    return null;
            }
        }

        public List<Slot> GetSlotsOccupiedByColor(PlayerColor color)
        {
            SlotColor slotColor = GetPlayerToSlotColor(color);
            List<Slot> slots = new List<Slot>();
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                if (slotList[i].SlotColor == slotColor)
                    slots.Add(slotList[i]);
            }
            Slot eatenSlot = GetEatenSlot(color);
            if (eatenSlot != null && eatenSlot.Quantity > 0)
                slots.Add(GetEatenSlot(color));

            return slots;
        }

        public void GetPip(out int whitePip, out int blackPip)
        {
            whitePip = GetEatenSlot(PlayerColor.White).Quantity * 25;
            blackPip = GetEatenSlot(PlayerColor.Black).Quantity * 25;
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                Slot s = GetSlot(i);
                if (s.IsOccupiedBy(PlayerColor.White))
                    whitePip += s.Quantity * (i + 1);
                else if (s.IsOccupiedBy(PlayerColor.Black))
                    blackPip += s.Quantity * (BOARD_SIZE - i);
            }
        }

        public int GetEndGameScoreMultiplier(PlayerColor color)
        {
            PlayerColor opponentColor = GetOpponentsColor(color);
            if (GetBearoffSlot(opponentColor).Quantity > 0)
            {
                // normal win
                return 1;
            }
            else if (GetNumberOfCheckersInStartingQuarter(opponentColor) == 0)
            {
                // gammon win
                return 2;
            }
            else
            {
                // backgammon win
                return 3;
            }
        }
        #endregion Getters

        #region Movement
        public void MakeUndoMove(params Move[] moves)
        {
            for (int i = 0; i < moves.Length; i++)
            {
                MoveChecker(moves[i].to, moves[i].from);
                if (moves[i].isEaten)
                {
                    int opponentEatenIndex = GetIndexOfEatenOpponent(moves[i].color);
                    MoveChecker(opponentEatenIndex, moves[i].to);
                }
            }
        }

        public void MakeMove(params Move[] moves)
        {
            for (int i = 0; i < moves.Length; i++)
            {
                if (moves[i].isEaten)
                {
                    int opponentEatenIndex = GetIndexOfEatenOpponent(moves[i].color);
                    MoveChecker(moves[i].to, opponentEatenIndex);
                }
                MoveChecker(moves[i].from, moves[i].to);
            }
        }

        private void MoveChecker(int from, int to)
        {
            if (from < 0 || from >= MAX_SLOTS || to < 0 || to >= MAX_SLOTS || slotList[from].Quantity <= 0 || slotList[to].Quantity >= MAX_IN_SLOT)
                throw new IlligalMoveException(from, to, slotList[from].Quantity, slotList[to].Quantity);

            slotList[to].Quantity++;
            slotList[to].SlotColor = slotList[from].SlotColor;

            slotList[from].Quantity--;
            if (slotList[from].Quantity == 0)
                slotList[from].SlotColor = SlotColor.Empty;
        }
        #endregion Movement

        #region Is Legal
        public MoveCheck isMoveLegal(int from, int to, PlayerColor color, int diceUsed)
        {
            Slot fromSlot, toSlot, eatenSlot;
            fromSlot = GetSlot(from);
            toSlot = GetSlot(to);

            // from slot null , from slot empty, moving opponents checker, trying to move from eaten with multiple dice when there is still eaten left in slot
            if (fromSlot == null)
                return MoveCheck.FromSlotNull;
            if (fromSlot.Quantity <= 0)
                return MoveCheck.FromSlotEmpty;
            if (fromSlot.SlotColor != GetPlayerToSlotColor(color))
                return MoveCheck.MovingOpponentsColor;

            // to slot null, to slot has more then max
            if (toSlot == null)
                return MoveCheck.ToSlotNull;

            if (toSlot.Quantity >= 15)
                return MoveCheck.ToSlotHasMaxCheckers;

            // there is an eaten checker and you tring to move not from eaten slot
            eatenSlot = GetEatenSlot(color);
            if (eatenSlot.Quantity > 0 && !fromSlot.IsEatenSlot())
                return MoveCheck.EatenLocked;

            // you tring to bearoff but you are not in bearoff state
            if (toSlot.IsBearoffSlot() && !IsInBearingOffState(color))
                return MoveCheck.BearoffLocked;

            int dir = from - to;
            if (color == PlayerColor.White)
            {
                // moving to eaten or bearoff of your opponent
                if (toSlot.GetSlotType() == SlotType.BlackEaten || toSlot.GetSlotType() == SlotType.BlackBearedOff)
                    return MoveCheck.MovingToOpponentsEatenOrBearoff;
                // cant eat when opponent has more then 1 in slot
                if (toSlot.SlotColor == SlotColor.Black && toSlot.Quantity > 1)
                    return MoveCheck.OpponentBlockedEat;
                // moving to wrong direction
                if (dir < 0 && toSlot.GetSlotType() != SlotType.WhiteBearedOff)
                    return MoveCheck.WrongDirection;

                // bearing off white
                if (toSlot.GetSlotType() == SlotType.WhiteBearedOff)
                {
                    if (from < GetLastSlotOfColor(PlayerColor.White) && DistanceSmallerThenDice(from, diceUsed, PlayerColor.White))
                        return MoveCheck.BearoffPriority;
                }
            }
            else
            {
                // moving to eaten or bearoff of your opponent
                if (toSlot.GetSlotType() == SlotType.WhiteEaten || toSlot.GetSlotType() == SlotType.WhiteBearedOff)
                    return MoveCheck.MovingToOpponentsEatenOrBearoff;
                // cant eat when opponent has more then 1 in slot
                if (toSlot.SlotColor == SlotColor.White && toSlot.Quantity > 1)
                    return MoveCheck.OpponentBlockedEat;
                // moving to wrong direction
                if (dir > 0 && fromSlot.GetSlotType() != SlotType.BlackEaten)
                    return MoveCheck.WrongDirection;

                // bearing off black
                if (toSlot.GetSlotType() == SlotType.BlackBearedOff)
                {
                    if (from > GetLastSlotOfColor(PlayerColor.Black) && DistanceSmallerThenDice(from, diceUsed, PlayerColor.Black))
                        return MoveCheck.BearoffPriority;
                }
            }
            return MoveCheck.Success;
        }
        #endregion Is Legal

        #region Aid Functions
        private int GetNumberOfCheckersInStartingQuarter(PlayerColor color)
        {
            int count = GetEatenSlot(color).Quantity;
            Slot[] slots = GetSlotsOccupiedByColorInQuarter(1, color);
            for (int i = 0; i < slots.Length; i++)
            {
                count += slots[i].Quantity;
            }
            return count;
        }

        /// <summary>
        /// Get slots occupied by color in specified quarter
        /// </summary>
        /// <param name="quarter">quarter number 1 - 4</param>
        /// <param name="color"></param>
        /// <returns></returns>
        private Slot[] GetSlotsOccupiedByColorInQuarter(int quarter, PlayerColor color)
        {
            List<Slot> slots = new List<Slot>();
            int from = color == PlayerColor.Black ? (quarter - 1) * 6 : (4 - quarter) * 6;
            int to = color == PlayerColor.Black ? quarter * 6 : (5 - quarter) * 6;

            for (int i = from; i < to; i++)
            {
                Slot slot = GetSlot(i);
                if (slot.IsOccupiedBy(color))
                {
                    slots.Add(slot);
                }
            }
            return slots.ToArray();
        }

        private bool IsInBearingOffState(PlayerColor color)
        {
            int counter = GetBearoffSlot(color).Quantity;
            Slot[] slots = GetSlotsOccupiedByColorInQuarter(4, color);
            for (int i = 0; i < slots.Length; i++)
            {
                counter += slots[i].Quantity;
            }

            if (counter == MAX_IN_SLOT)
                return true;
            return false;
        }

        private bool DistanceSmallerThenDice(int index, int dice, PlayerColor color)
        {
            if (DistanceFromBearoff(index, color) >= dice)
                return false;
            return true;
        }

        private bool DistanceFromBearoffIsEqualToUnusedDice(int index, PlayerColor color, List<int> unusedDice)
        {
            return unusedDice.Contains(DistanceFromBearoff(index, color));
        }

        private int DistanceFromBearoff(int index, PlayerColor color)
        {
            if (color == PlayerColor.Black)
                return BOARD_SIZE - index;
            else
                return index + 1;
        }

        private int GetLastSlotOfColor(PlayerColor color)
        {
            if (color == PlayerColor.Black)
                for (int i = 0; i < BOARD_SIZE; i++)
                    if (GetSlot(i).IsOccupiedBy(PlayerColor.Black))
                        return i;
            if (color == PlayerColor.White)
                for (int i = BOARD_SIZE - 1; i >= 0; i--)
                    if (GetSlot(i).IsOccupiedBy(PlayerColor.White))
                        return i;
            return -1;
        }
        #endregion Aid Functions

        #region Static Aid Function
        public static int DieToIndex(int from, int die, PlayerColor color)
        {
            if (color == PlayerColor.Black)
            {
                if (from == EATEN_BLACK_INDEX)
                    return die - 1;
                else
                {
                    int idx = from + die;
                    if (idx > BOARD_SIZE - 1)
                        return BEAROFF_BLACK_INDEX;
                    else return idx;
                }
            }
            else
            {
                if (from == EATEN_WHITE_INDEX)
                    return BOARD_SIZE - die;
                else
                {
                    int idx = from - die;
                    if (idx < 0)
                        return BEAROFF_WHITE_INDEX;
                    return idx;
                }
            }
        }

        public static int GetIndexOfEaten(PlayerColor color)
        {
            return color == PlayerColor.Black ? EATEN_BLACK_INDEX : EATEN_WHITE_INDEX;
        }
        public static int GetIndexOfEatenOpponent(PlayerColor color)
        {
            return color == PlayerColor.Black ? EATEN_WHITE_INDEX : EATEN_BLACK_INDEX;
        }

        public static PlayerColor GetSlotToPlayerColor(SlotColor color)
        {
            return color == SlotColor.Black ? PlayerColor.Black : PlayerColor.White;
        }

        public static SlotColor GetPlayerToSlotColor(PlayerColor color)
        {
            return color == PlayerColor.White ? SlotColor.White : SlotColor.Black;
        }

        public static SlotColor GetOpponentToSlotColor(PlayerColor color)
        {
            return color == PlayerColor.White ? SlotColor.Black : SlotColor.White;
        }

        public static PlayerColor GetOpponentsColor(PlayerColor color)
        {
            return color == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
        }
        #endregion Static Aid Function

        #region Overrides
        public override string ToString()
        {
            return slotList.Display();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Board b = obj as Board;
            if (b == null)
                return false;

            return Serialize(this).Equals(Serialize(b));
        }

        public override int GetHashCode()
        {
            int hash = 13;
            for (int i = BOARD_SIZE - 1; i < BOARD_SIZE - 6; i++)
            {
                hash = (hash * 7) + slotList[i].Quantity.GetHashCode();
            }
            return hash;
        }
        #endregion Overrides

        #region Operators
        public static bool operator ==(Board a, Board b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return Serialize(a).Equals(Serialize(b));
        }

        public static bool operator !=(Board a, Board b)
        {
            return !(a == b);
        }
        #endregion Operators

        #region Serialization
        /// <summary>
        /// Serialize a slot list to string
        /// </summary>
        /// <param name="slots"></param>
        /// <returns></returns>
        public static string Serialize(List<Slot> slots)
        {
            char[] boardString = new char[MAX_SLOTS];
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                if (slots[i].SlotColor == SlotColor.Empty)
                    boardString[i] = EMPTY_CHAR;
                else if (slots[i].SlotColor == SlotColor.White)
                    boardString[i] = (char)(slots[i].Quantity + WHITE_START_CHAR - 1);
                else if (slots[i].SlotColor == SlotColor.Black)
                    boardString[i] = (char)(slots[i].Quantity + BLACK_START_CHAR - 1);
            }
            //bear off
            int whiteBearedoff = slots[BEAROFF_WHITE_INDEX].Quantity;
            boardString[BEAROFF_WHITE_INDEX] = whiteBearedoff == 0 ? EMPTY_CHAR : (char)(whiteBearedoff + WHITE_START_CHAR - 1);
            int blackBearedoff = slots[BEAROFF_BLACK_INDEX].Quantity;
            boardString[BEAROFF_BLACK_INDEX] = blackBearedoff == 0 ? EMPTY_CHAR : (char)(blackBearedoff + BLACK_START_CHAR - 1);

            //eaten
            int whiteEaten = slots[EATEN_WHITE_INDEX].Quantity;
            boardString[EATEN_WHITE_INDEX] = whiteEaten == 0 ? EMPTY_CHAR : (char)(whiteEaten + WHITE_START_CHAR - 1);
            int blackEaten = slots[EATEN_BLACK_INDEX].Quantity;
            boardString[EATEN_BLACK_INDEX] = blackEaten == 0 ? EMPTY_CHAR : (char)(blackEaten + BLACK_START_CHAR - 1);

            return new string(boardString);
        }

        /// <summary>
        /// Serialize a board to string
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static string Serialize(Board board)
        {
            return Serialize(board.slotList);
        }

        /// <summary>
        /// Deserialize string to list of slots,
        /// Capital letters represent white,
        /// Lower case represent black</summary>
        /// <param name="boardString"></param>
        /// <returns></returns>
        public static List<Slot> Deserialize(string boardString)
        {
            List<Slot> slots = new List<Slot>();
            char[] array = boardString.ToCharArray();
            for (int i = 0; i < MAX_SLOTS; i++)
            {
                if (array[i] >= WHITE_START_CHAR && array[i] <= WHITE_END_CHAR)
                    slots.Add(new Slot(i, array[i] - WHITE_START_CHAR + 1, SlotColor.White));
                else if (array[i] >= BLACK_START_CHAR && array[i] <= BLACK_END_CHAR)
                    slots.Add(new Slot(i, array[i] - BLACK_START_CHAR + 1, SlotColor.Black));
                else if (array[i] == EMPTY_CHAR)
                    slots.Add(new Slot(i, 0, SlotColor.Empty));
            }
            return slots;
        }
        #endregion Serialization
    }
}
