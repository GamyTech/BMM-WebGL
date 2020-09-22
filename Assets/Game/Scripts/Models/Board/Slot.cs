using GT.Backgammon.Player;

namespace GT.Backgammon.Logic
{
    public enum SlotColor { Empty, White, Black }
    public enum SlotType { Empty, WhiteOccupied, BlackOccupied, WhiteEaten, BlackEaten, WhiteBearedOff, BlackBearedOff }

    public class Slot
    {
        private int m_index;
        public int Index
        {
            get { return m_index; }
        }

        private int m_quantity;
        public int Quantity
        {
            get { return m_quantity; }
            internal set { m_quantity = value; }
        }

        private SlotColor m_slotColor;
        public SlotColor SlotColor
        {
            get { return m_slotColor; }
            internal set { m_slotColor = value; }
        }

        public Slot(int index, int quantity, SlotColor color)
        {
            m_index = index;
            m_quantity = quantity;
            m_slotColor = color;
        }

        #region Slot Aid Functions
        public SlotType GetSlotType()
        {
            if (Index == Board.EATEN_BLACK_INDEX)
                return SlotType.BlackEaten;
            else if (Index == Board.EATEN_WHITE_INDEX)
                return SlotType.WhiteEaten;
            else if (Index == Board.BEAROFF_BLACK_INDEX)
                return SlotType.BlackBearedOff;
            else if (Index == Board.BEAROFF_WHITE_INDEX)
                return SlotType.WhiteBearedOff;
            else if (SlotColor == SlotColor.Black)
                return SlotType.BlackOccupied;
            else if (SlotColor == SlotColor.White)
                return SlotType.WhiteOccupied;
            else
                return SlotType.Empty;
        }

        public bool IsOccupiedBy(PlayerColor color)
        {
            if (m_slotColor == SlotColor.Black && color == PlayerColor.Black ||
                m_slotColor == SlotColor.White && color == PlayerColor.White)
                return true;
            return false;
        }

        public bool IsEatenSlot()
        {
            if (Index == Board.EATEN_WHITE_INDEX || Index == Board.EATEN_BLACK_INDEX)
                return true;
            return false;
        }

        public bool IsBearoffSlot()
        {
            if (Index == Board.BEAROFF_WHITE_INDEX || Index == Board.BEAROFF_BLACK_INDEX)
                return true;
            return false;
        }
        #endregion Slot Aid Functions

        #region Overrides
        public override string ToString()
        {
            return Quantity.ToString() + SlotColor.ToString()[0];
        }
        #endregion Overrides
    }
}
