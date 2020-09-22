using System.Collections.Generic;
using UnityEngine;

namespace GT.User
{
    public class TimelyBonus
    {

        private System.DateTime m_lastCollectedDate;
        private int m_collectTimeMinutes;

        public System.DateTime NextUnlockTime { get; private set; }
        public float MinuteToWait { get { return (float)NextUnlockTime.Subtract(System.DateTime.UtcNow).TotalMinutes; } }
        public float Progress { get { return ((float)m_collectTimeMinutes - MinuteToWait) / (float)m_collectTimeMinutes; } }
        public bool IsReady { get { return MinuteToWait <= 0; } }

        public TimelyBonus(Dictionary<string, object> dict)
        {
            object o;
            if (dict.TryGetValue("LastCollectedDate", out o))
                System.DateTime.TryParse(o.ToString(), out m_lastCollectedDate);
            else
                Debug.LogError("LastCollectedDate missing");

            if (dict.TryGetValue("CollectTimeMinutes", out o))
                m_collectTimeMinutes = o.ParseInt();
            else
                Debug.LogError("CollectTimeMinutes missing");

            NextUnlockTime = m_lastCollectedDate.AddMinutes(m_collectTimeMinutes);
        }
    }
}
