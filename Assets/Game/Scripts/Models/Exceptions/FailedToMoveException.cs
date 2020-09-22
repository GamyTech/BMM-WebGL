using System;

namespace GT.Backgammon.Logic
{
    public class FailedToMoveException : Exception
    {
        private string m_message;
        public override string Message
        {
            get
            {
                return base.Message + m_message;
            }
        }

        public FailedToMoveException(int from, int to) : base(string.Empty)
        {
            m_message = "Move Failed (" + from + "," + to + ")";
        }
    }
}
