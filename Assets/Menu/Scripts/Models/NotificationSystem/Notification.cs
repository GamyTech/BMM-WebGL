using UnityEngine;
using System.Collections;
using System;

namespace GT.Notifications
{
    public delegate void NotificationCountChanged(int count);

    public abstract class Notification
    {
        public event NotificationCountChanged OnNotificationCountChanged;

        private int m_notificationCount;
        public int notificationCount
        {
            get { return m_notificationCount; }
            set
            {
                if (Utils.SetProperty(ref m_notificationCount, value) && OnNotificationCountChanged != null)
                    OnNotificationCountChanged(m_notificationCount);
            }
        }

        protected Notification()
        {
            Reset();
        }

        protected virtual void Reset()
        {
            notificationCount = 0;
        }

        protected abstract void UpdateNotification();

        public abstract void ForceUpdate();
    }
}
