using System.Collections.Generic;
using GT.User;

namespace GT.Websocket
{
    public class UserResponse : ResponseBase
    {
        private Dictionary<string, object> m_userDetails;
        private Dictionary<string, object> m_wallet;
        private Dictionary<string, object> m_dailyBonus;
        private Dictionary<string, object> m_verification;

        public UserResponse(string message) : base(message)
        {
            if (responseCode == WSResponseCode.OK)
                Init();
        }

        /// <summary>
        /// Build response from response data
        /// </summary>
        private void Init()
        {
            object o;
            if (TryGetAPIVariable(APIResponseVariable.UserDetails, out o))
                m_userDetails = (Dictionary<string, object>)o;

            if (TryGetAPIVariable(APIResponseVariable.Wallet, out o))
                m_wallet = (Dictionary<string, object>)o;

            if (TryGetAPIVariable(APIResponseVariable.DailyBonus, out o))
                m_dailyBonus = (Dictionary<string, object>)o;

            if (TryGetAPIVariable(APIResponseVariable.Verification, out o))
                m_verification = (Dictionary<string, object>)o;
        }

        public GTUser CreateGTUser()
        {
            return new GTUser(m_userDetails, m_wallet, m_verification, m_dailyBonus);
        }
    }
}
