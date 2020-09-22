using System;

namespace GT.Exceptions
{
    public class MissingKeyException : Exception
    {
        private string m_message;
        public override string Message
        {
            get
            {
                return base.Message + m_message;
            }
        }

        public MissingKeyException(string missingKey) : base(string.Empty)
        {
            m_message = "Missing Key: " + missingKey;
        }
    }
}
