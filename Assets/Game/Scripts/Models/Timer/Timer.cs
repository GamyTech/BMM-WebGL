using UnityEngine;
using System.Collections;

namespace GT.Backgammon.Logic
{
    public class Timer 
    {
        public delegate void TimerChangedHandler(float currentTime);
        public delegate void TimerEndedHandler();

        public event TimerChangedHandler OnTimerChanged;
        public event TimerEndedHandler OnTimerEnded;

        private bool m_enabled;

        private float m_currentTime;
        public float CurrentTime
        {
            get { return m_currentTime; }
            private set
            {
                m_currentTime = value;
                if (m_currentTime < 0)
                    m_currentTime = 0;
            }
        }


        public Timer()
        {
            CurrentTime = 0;
            m_enabled = false;
        }

        public void SetTimer(float time)
        {
            CurrentTime = time;
        }

        public void SetEnabled(bool enabled)
        {
            m_enabled = enabled;
        }

        public void UpdateTimer(float deltaTime)
        {
            if (!m_enabled) return;

            CurrentTime -= deltaTime;

            if (OnTimerChanged != null)
                OnTimerChanged(CurrentTime);

            if (CurrentTime == 0)
            {
                if (OnTimerEnded != null)
                    OnTimerEnded();
            }
        }
    }
}
