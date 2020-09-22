using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using GT.User;

namespace GT.Notifications
{
    public class BonusNotification : Notification, IUserNotification
    {
        private bool timely = false;
        private TimelyBonus timelyBonus;
        private UnityAction unregisterAction;
        
        public BonusNotification(MonoBehaviour mono) : base()
        {
            this.timely = false;
        }

        ~BonusNotification()
        {
            unregisterAction();
        }

        public override void ForceUpdate()
        {
            SetTimelyBonus(timelyBonus);
            UpdateNotification();
        }

        public void SetUser(GTUser user)
        {
            if (user != null)
            {
                timelyBonus = user.TimelyBonusData;
                user.OnTimelyBonusChanged += GtUser_OnTimelyBonusChanged;
                unregisterAction = () =>
                {
                    user.OnTimelyBonusChanged -= GtUser_OnTimelyBonusChanged;
                };
                SetTimelyBonus(timelyBonus);
                UpdateNotification();
            }
            else
                Reset();
        }

        protected override void UpdateNotification()
        {
            int tempCount = 0;
            if (timely)
                tempCount++;
            notificationCount = tempCount;
        }

        protected override void Reset()
        {
            base.Reset();
            timely = false;
        }

        #region Aid Functions
        private void SetTimelyBonus(TimelyBonus timelyBonus)
        {
            this.timelyBonus = timelyBonus;
            timely = timelyBonus == null? false : timelyBonus.IsReady;
        }
        #endregion Aid Functions

        #region Events
        private void GtUser_OnTimelyBonusChanged(TimelyBonus newValue)
        {
            SetTimelyBonus(newValue);
            UpdateNotification();
        }
        #endregion Events
    }
}
