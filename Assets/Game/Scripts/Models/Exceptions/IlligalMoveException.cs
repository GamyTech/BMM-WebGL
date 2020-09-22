using System;

namespace GT.Backgammon.Logic
{
    public class IlligalMoveException : Exception
    {
        private string m_message;
        public override string Message
        {
            get
            {
                return base.Message + m_message;
            }
        }

        public IlligalMoveException(int from, int to, int fromQuantity, int toQuantity) : base(string.Empty)
        {
            m_message = "Move (" + from + "," + to + ") Quantity (" + fromQuantity + "," + toQuantity + ")"; 
        }
    }
}
